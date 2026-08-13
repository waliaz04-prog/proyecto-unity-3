using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 4f;
    [SerializeField] private float velocidadCorrer = 7f;

    [Header("Salto")]
    [Tooltip("Activa el salto si el personaje puede saltar.")]
    [SerializeField] private bool permitirSalto = false;

    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private float gravedad = -20f;

    [Header("Estamina")]
    [SerializeField] private float estaminaMax = 100f;
    [SerializeField] private float consumoEstamina = 15f;
    [SerializeField] private float regeneracionEstamina = 10f;

    [Header("Estado")]
    [SerializeField] private bool controlesBloqueados;

    [Header("Animación")]
    [SerializeField] private Animator animator;

    [Tooltip("Nombre exacto del estado Idle dentro del Animator.")]
    [SerializeField] private string estadoIdle = "Idle";

    [Tooltip("Nombre exacto del estado de caminar hacia adelante.")]
    [SerializeField] private string estadoCaminarAdelante = "CaminarAdelante";

    [Tooltip("Nombre exacto del estado de caminar hacia la izquierda.")]
    [SerializeField] private string estadoCaminarIzquierda = "CaminarIzquierda";

    [Tooltip("Velocidad de reproducción de la animación de caminar hacia adelante.")]
    [SerializeField] private float velocidadAnimacionAdelante = 1f;

    [Tooltip("Velocidad de reproducción de la animación lateral.")]
    [SerializeField] private float velocidadAnimacionLateral = 1f;

    [Tooltip("Tiempo utilizado para cambiar suavemente entre estados de animación.")]
    [SerializeField] private float duracionTransicionAnimacion = 0.15f;

    [Header("Orientación")]
    [Tooltip("Si está activado, el personaje gira automáticamente hacia la dirección del movimiento.")]
    [SerializeField] private bool girarConMovimiento = false;

    [Tooltip("Velocidad de giro del personaje.")]
    [SerializeField] private float velocidadGiro = 10f;

    public bool EstaMuerto { get; private set; }

    private CharacterController controller;

    private Vector3 velocidadVertical;

    private bool isGrounded;
    private bool corriendo;

    private float estaminaActual;

    private int hashEstadoActual;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();

        if (animator == null)
            animator = GetComponent<Animator>();

        estaminaActual = estaminaMax;

        ValidarConfiguracion();
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        if (EstaMuerto)
        {
            AplicarGravedad();
            return;
        }

        if (controlesBloqueados)
        {
            AplicarGravedad();
            return;
        }

        RevisarSuelo();

        ManejarCorrer();

        Movimiento();

        ManejarSalto();

        AplicarGravedad();
    }

    private void Movimiento()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        Vector3 direccion =
            transform.right * x +
            transform.forward * z;

        if (direccion.sqrMagnitude > 1f)
            direccion.Normalize();

        float velocidad =
            corriendo
                ? velocidadCorrer
                : velocidadCaminar;

        controller.Move(
            direccion *
            velocidad *
            Time.deltaTime
        );

        ActualizarAnimacion(x, z);

        if (girarConMovimiento && direccion.sqrMagnitude > 0.001f)
            GirarHaciaMovimiento(direccion);
    }

    private void ActualizarAnimacion(float x, float z)
    {
        if (animator == null)
            return;

        bool seEstaMoviendo =
            Mathf.Abs(x) > 0.01f ||
            Mathf.Abs(z) > 0.01f;

        if (!seEstaMoviendo)
        {
            ReproducirEstado(
                estadoIdle,
                1f
            );

            return;
        }

        if (Mathf.Abs(z) >= Mathf.Abs(x))
        {
            ReproducirEstado(
                estadoCaminarAdelante,
                z >= 0f
                    ? velocidadAnimacionAdelante
                    : velocidadAnimacionAdelante
            );
        }
        else
        {
            ReproducirEstado(
                estadoCaminarIzquierda,
                velocidadAnimacionLateral
            );
        }
    }

    private void ReproducirEstado(
        string nombreEstado,
        float velocidadAnimacion)
    {
        if (string.IsNullOrEmpty(nombreEstado))
            return;

        int hashEstado =
            Animator.StringToHash(nombreEstado);

        if (hashEstadoActual != hashEstado)
        {
            animator.CrossFade(
                hashEstado,
                duracionTransicionAnimacion
            );

            hashEstadoActual = hashEstado;
        }

        animator.speed = Mathf.Max(
            0.01f,
            velocidadAnimacion
        );
    }

    private void GirarHaciaMovimiento(Vector3 direccion)
    {
        if (direccion.sqrMagnitude <= 0.001f)
            return;

        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(direccion);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadGiro * Time.deltaTime
            );
    }

    private void RevisarSuelo()
    {
        isGrounded = controller.isGrounded;

        if (isGrounded && velocidadVertical.y < 0f)
            velocidadVertical.y = -2f;
    }

    private void ManejarSalto()
    {
        if (!permitirSalto)
            return;

        if (!isGrounded)
            return;

        if (!Input.GetKeyDown(KeyCode.Space))
            return;

        velocidadVertical.y =
            Mathf.Sqrt(
                fuerzaSalto *
                -2f *
                gravedad
            );

        if (animator != null)
        {
            animator.SetTrigger(
                "Saltar"
            );
        }
    }

    private void AplicarGravedad()
    {
        velocidadVertical.y +=
            gravedad *
            Time.deltaTime;

        controller.Move(
            velocidadVertical *
            Time.deltaTime
        );
    }

    private void ManejarCorrer()
    {
        bool intentaCorrer =
            Input.GetKey(KeyCode.LeftShift) &&
            estaminaActual > 0f &&
            Input.GetAxisRaw("Vertical") > 0f;

        corriendo = intentaCorrer;

        if (intentaCorrer)
        {
            estaminaActual -=
                consumoEstamina *
                Time.deltaTime;
        }
        else
        {
            estaminaActual +=
                regeneracionEstamina *
                Time.deltaTime;
        }

        estaminaActual =
            Mathf.Clamp(
                estaminaActual,
                0f,
                estaminaMax
            );

        if (estaminaActual <= 0f)
            corriendo = false;
    }

    public float ObtenerPorcentajeEstamina()
    {
        if (estaminaMax <= 0f)
            return 0f;

        return estaminaActual /
               estaminaMax;
    }

    public void SubirVelocidad(float cantidad)
    {
        if (cantidad <= 0f)
            return;

        velocidadCaminar += cantidad;
        velocidadCorrer += cantidad;
    }

    public void BloquearControles(bool estado)
    {
        controlesBloqueados = estado;

        if (estado)
        {
            corriendo = false;

            if (animator != null)
            {
                ReproducirEstado(
                    estadoIdle,
                    1f
                );
            }
        }
    }

    public void Morir()
    {
        if (EstaMuerto)
            return;

        EstaMuerto = true;
        controlesBloqueados = true;
        corriendo = false;

        if (animator != null)
        {
            animator.speed = 1f;

            animator.SetTrigger(
                "Muerte"
            );
        }
    }

    public bool EstaCorriendo()
    {
        return corriendo;
    }

    public float ObtenerEstaminaActual()
    {
        return estaminaActual;
    }

    public float ObtenerEstaminaMaxima()
    {
        return estaminaMax;
    }

    private void ValidarConfiguracion()
    {
        if (estaminaMax < 0f)
            estaminaMax = 0f;

        if (estaminaActual > estaminaMax)
            estaminaActual = estaminaMax;

        if (velocidadCaminar < 0f)
            velocidadCaminar = 0f;

        if (velocidadCorrer < 0f)
            velocidadCorrer = 0f;

        if (velocidadGiro < 0f)
            velocidadGiro = 0f;

        if (duracionTransicionAnimacion < 0f)
            duracionTransicionAnimacion = 0f;

        if (velocidadAnimacionAdelante <= 0f)
            velocidadAnimacionAdelante = 1f;

        if (velocidadAnimacionLateral <= 0f)
            velocidadAnimacionLateral = 1f;
    }
}