using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovimiento : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 4f;
    [SerializeField] private float velocidadCorrer = 7f;

    [Header("Salto")]
    [Tooltip("Desmarca esto si el juego no necesita saltar. La gravedad sigue aplicándose igual (para caídas), solo se bloquea la tecla de salto.")]
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
    [Tooltip("Suaviza la transición entre valores de movimiento en el Blend Tree (segundos). 0 = respuesta instantánea, más alto = más suave pero menos preciso.")]
    [SerializeField] private float suavizadoAnimacion = 0.1f;

    public bool EstaMuerto { get; private set; }

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocidadVertical;
    private bool isGrounded;
    private bool corriendo;
    private float estaminaActual;

    // VelocidadX = movimiento lateral (izquierda/derecha), VelocidadZ = adelante/atrás.
    // Separarlos en dos ejes permite que el Blend Tree del Animator reutilice el
    // mismo clip para adelante y atrás (reproducido al revés) y otro distinto
    // para los lados, en vez de un solo valor de velocidad "sin dirección".
    private static readonly int AnimVelocidadX = Animator.StringToHash("VelocidadX");
    private static readonly int AnimVelocidadZ = Animator.StringToHash("VelocidadZ");
    private static readonly int AnimCorriendo = Animator.StringToHash("Corriendo");
    private static readonly int AnimSaltar = Animator.StringToHash("Saltar");
    private static readonly int AnimMuerte = Animator.StringToHash("Muerte");

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        estaminaActual = estaminaMax;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || controlesBloqueados || EstaMuerto) return;

        RevisarSuelo();
        Movimiento();
        ManejarSalto();
        AplicarGravedad();
        ManejarCorrer();
    }

    private void Movimiento()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float velocidad = corriendo ? velocidadCorrer : velocidadCaminar;
        Vector3 direccion = transform.right * x + transform.forward * z;
        controller.Move(direccion.normalized * velocidad * Time.deltaTime);

        if (animator != null)
        {
            // SetFloat con dampTime suaviza la mezcla del Blend Tree en vez de
            // saltar bruscamente entre direcciones cada vez que cambia el input.
            animator.SetFloat(AnimVelocidadX, x, suavizadoAnimacion, Time.deltaTime);
            animator.SetFloat(AnimVelocidadZ, z, suavizadoAnimacion, Time.deltaTime);
            animator.SetBool(AnimCorriendo, corriendo);
        }
    }

    private void RevisarSuelo()
    {
        // CharacterController ya calcula el contacto con el suelo al llamar a Move().
        // Se lee aquí, antes de mover en este frame, para saber si al terminar el
        // frame anterior el jugador estaba tocando el suelo (usado para saltar y
        // para frenar la caída). Evita depender de un Transform + LayerMask aparte
        // que puede quedar sin asignar o romperse si se borra el objeto.
        isGrounded = controller.isGrounded;
        if (isGrounded && velocidadVertical.y < 0)
            velocidadVertical.y = -2f;
    }

    private void ManejarSalto()
    {
        if (!permitirSalto) return;

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocidadVertical.y = Mathf.Sqrt(fuerzaSalto * -2f * gravedad);
            if (animator != null) animator.SetTrigger(AnimSaltar);
        }
    }

    private void AplicarGravedad()
    {
        velocidadVertical.y += gravedad * Time.deltaTime;
        controller.Move(velocidadVertical * Time.deltaTime);
    }

    private void ManejarCorrer()
    {
        bool intentaCorrer = Input.GetKey(KeyCode.LeftShift)
            && estaminaActual > 0
            && Input.GetAxisRaw("Vertical") > 0;

        corriendo = intentaCorrer;

        estaminaActual += intentaCorrer
            ? -consumoEstamina * Time.deltaTime
            : regeneracionEstamina * Time.deltaTime;

        estaminaActual = Mathf.Clamp(estaminaActual, 0f, estaminaMax);
    }

    public float ObtenerPorcentajeEstamina() => estaminaActual / estaminaMax;

    public void SubirVelocidad(float cantidad)
    {
        velocidadCaminar += cantidad;
        velocidadCorrer += cantidad;
    }

    public void BloquearControles(bool estado)
    {
        controlesBloqueados = estado;
    }

    public void Morir()
    {
        if (EstaMuerto) return;
        EstaMuerto = true;
        controlesBloqueados = true;
        if (animator != null) animator.SetTrigger(AnimMuerte);
    }
}
