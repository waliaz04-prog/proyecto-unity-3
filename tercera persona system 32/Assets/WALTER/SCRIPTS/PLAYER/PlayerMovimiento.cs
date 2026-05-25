using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovimiento : MonoBehaviour
{
    private CharacterController controller;
    private Animator animator;

    [Header("Movimiento")]
    [SerializeField] private float velocidadCaminar = 4f;
    [SerializeField] private float velocidadCorrer = 7f;

    [Header("Salto")]
    [SerializeField] private float fuerzaSalto = 5f;
    [SerializeField] private float gravedad = -20f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float radioSuelo = 0.3f;
    [SerializeField] private LayerMask capaSuelo;

    [Header("Estamina")]
    [SerializeField] private float estaminaMax = 100f;
    [SerializeField] private float consumoEstamina = 15f;
    [SerializeField] private float regeneracionEstamina = 10f;

    private Vector3 velocidadVertical;

    private bool isGrounded;
    private bool corriendo;

    private float estaminaActual;

    [Header("Estado")]
    [SerializeField] private bool controlesBloqueados;

    public bool estaMuerto { get; private set; }

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        estaminaActual = estaminaMax;
    }

    private void Update()
    {
        if (Time.timeScale == 0 || controlesBloqueados || estaMuerto)
            return;

        RevisarSuelo();

        Movimiento();

        ManejarSalto();

        AplicarGravedad();

        ManejarCorrer();
    }

    // MOVIMIENTO
    private void Movimiento()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        float velocidadActual =
            corriendo ? velocidadCorrer : velocidadCaminar;

        Vector3 direccion =
            transform.right * x +
            transform.forward * z;

        controller.Move(
            direccion.normalized *
            velocidadActual *
            Time.deltaTime
        );

        if (animator != null)
        {
            animator.SetFloat(
                "Velocidad",
                direccion.magnitude
            );

            animator.SetBool(
                "Corriendo",
                corriendo
            );
        }
    }

    // DETECTAR SUELO
    private void RevisarSuelo()
    {
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            radioSuelo,
            capaSuelo
        );

        if (isGrounded && velocidadVertical.y < 0)
        {
            velocidadVertical.y = -2f;
        }
    }

    // SALTO
    private void ManejarSalto()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            velocidadVertical.y =
                Mathf.Sqrt(
                    fuerzaSalto * -2f * gravedad
                );

            if (animator != null)
            {
                animator.SetTrigger("Saltar");
            }
        }
    }

    // GRAVEDAD
    private void AplicarGravedad()
    {
        velocidadVertical.y += gravedad * Time.deltaTime;

        controller.Move(
            velocidadVertical * Time.deltaTime
        );
    }

    // CORRER
    private void ManejarCorrer()
    {
        bool intentandoCorrer =
            Input.GetKey(KeyCode.LeftShift) &&
            estaminaActual > 0 &&
            Input.GetAxisRaw("Vertical") > 0;

        if (intentandoCorrer)
        {
            corriendo = true;

            estaminaActual -=
                consumoEstamina * Time.deltaTime;
        }
        else
        {
            corriendo = false;

            estaminaActual +=
                regeneracionEstamina * Time.deltaTime;
        }

        estaminaActual = Mathf.Clamp(
            estaminaActual,
            0,
            estaminaMax
        );
    }

    // ESTAMINA
    public float ObtenerPorcentajeEstamina()
    {
        return estaminaActual / estaminaMax;
    }

    // BLOQUEAR CONTROLES
    public void BloquearControles(bool estado)
    {
        controlesBloqueados = estado;
    }

    // MUERTE
    public void Morir()
    {
        if (estaMuerto)
            return;

        estaMuerto = true;

        controlesBloqueados = true;

        if (animator != null)
        {
            animator.SetTrigger("Muerte");
        }
    }
}