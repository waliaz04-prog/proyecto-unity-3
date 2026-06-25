using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovimiento : MonoBehaviour
{
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

    [Header("Estado")]
    [SerializeField] private bool controlesBloqueados;

    public bool EstaMuerto { get; private set; }

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocidadVertical;
    private bool isGrounded;
    private bool corriendo;
    private float estaminaActual;

    private static readonly int AnimVelocidad = Animator.StringToHash("Velocidad");
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
            // Usar Mathf.Clamp01 para evitar que la magnitud diagonal supere 1 en el Animator
            animator.SetFloat(AnimVelocidad, Mathf.Clamp01(direccion.magnitude));
            animator.SetBool(AnimCorriendo, corriendo);
        }
    }

    private void RevisarSuelo()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, radioSuelo, capaSuelo);
        if (isGrounded && velocidadVertical.y < 0)
            velocidadVertical.y = -2f;
    }

    private void ManejarSalto()
    {
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
