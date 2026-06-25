using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Tipo")]
    [SerializeField] private WeaponType tipoArma;

    [Header("General")]
    [SerializeField] private float danio = 20f;
    [SerializeField] private float tiempoEntreAtaques = 0.5f;

    [Header("Melee")]
    [SerializeField] private WeaponMeleeTrigger meleeTrigger;
    [SerializeField] private Animator animator;
    [SerializeField] private float tiempoHitbox = 0.2f;

    [Header("Disparo")]
    [SerializeField] private bool armaAutomatica;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private ParticleSystem efectoDisparo;
    [SerializeField] private int balasPorDisparo = 1;
    [SerializeField] private float dispersion;
    [SerializeField] private bool usarMunicion = true;
    [SerializeField] private int municionActual = 30;
    [SerializeField] private int municionMaxima = 30;
    [SerializeField] private float velocidadBala = 80f;
    [SerializeField] private float tiempoVidaBala = 5f;
    [SerializeField] private bool atravesarEnemigos;
    [SerializeField] private float tiempoRecarga = 1.5f;

    [Header("Pool")]
    [SerializeField] private string idPoolBala = "bala";

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs;

    private float siguienteAtaque;
    private Camera camaraPrincipal;
    private readonly Vector3 centroViewport = new Vector3(0.5f, 0.5f, 0f);

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        camaraPrincipal = Camera.main;
    }

    private void Update()
    {
        if (Time.time < siguienteAtaque) return;

        switch (tipoArma)
        {
            case WeaponType.Melee:
                if (Input.GetMouseButtonDown(0))
                    AtaqueMelee();
                break;

            case WeaponType.Firearm:
                bool disparar = armaAutomatica ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
                if (disparar) Disparar();
                break;
        }

        // Recarga manual con tecla R
        if (Input.GetKeyDown(KeyCode.R))
            Recargar();
    }

    private void AtaqueMelee()
    {
        siguienteAtaque = Time.time + tiempoEntreAtaques;

        if (animator != null)
        {
            animator.SetTrigger("Atacar");
        }
        else
        {
            ActivarMelee();
            Invoke(nameof(DesactivarMelee), tiempoHitbox);
        }
    }

    public void ActivarMelee()
    {
        if (meleeTrigger != null)
            meleeTrigger.ActivarTrigger();
    }

    public void DesactivarMelee()
    {
        if (meleeTrigger != null)
            meleeTrigger.DesactivarTrigger();
    }

    private void Disparar()
    {
        if (usarMunicion && municionActual <= 0) return;

        siguienteAtaque = Time.time + tiempoEntreAtaques;

        if (usarMunicion)
            municionActual--;

        if (efectoDisparo != null)
            efectoDisparo.Play();

        for (int i = 0; i < balasPorDisparo; i++)
            CrearBala();
    }

    private void CrearBala()
    {
        if (puntoDisparo == null || PoolManager.Instance == null) return;

        if (camaraPrincipal == null)
            camaraPrincipal = Camera.main;

        if (camaraPrincipal == null) return;

        Ray ray = camaraPrincipal.ViewportPointToRay(centroViewport);
        Vector3 objetivo = Physics.Raycast(ray, out RaycastHit hit, 1000f)
            ? hit.point
            : ray.origin + ray.direction * 1000f;

        Vector3 direccion = (objetivo - puntoDisparo.position).normalized;

        if (dispersion > 0f)
        {
            direccion += new Vector3(
                Random.Range(-dispersion, dispersion),
                Random.Range(-dispersion, dispersion),
                Random.Range(-dispersion, dispersion));
            direccion.Normalize();
        }

        GameObject balaObj = PoolManager.Instance.ObtenerObjeto(
            idPoolBala, puntoDisparo.position, Quaternion.LookRotation(direccion));

        if (balaObj == null) return;

        if (balaObj.TryGetComponent(out Bala bala))
            bala.Configurar(danio, velocidadBala, tiempoVidaBala, atravesarEnemigos, disparadoPorJugador: true);
    }

    public void Recargar()
    {
        municionActual = municionMaxima;
        if (mostrarLogs) Debug.Log("Recargando: " + gameObject.name);
    }

    public float ObtenerDanio() => danio;
    public WeaponType ObtenerTipoArma() => tipoArma;
    public int ObtenerMunicion() => municionActual;
    public int ObtenerMunicionMaxima() => municionMaxima;

    public void SubirDano(float cantidad) => danio += cantidad;

    public void MejorarCadencia(float reduccion)
    {
        tiempoEntreAtaques = Mathf.Max(0.05f, tiempoEntreAtaques - reduccion);
    }

    public void SubirCapacidadMunicion(int cantidad)
    {
        municionMaxima += cantidad;
        municionActual += cantidad;
    }

    public void MejorarVelocidadRecarga(float reduccion)
    {
        tiempoRecarga = Mathf.Max(0.1f, tiempoRecarga - reduccion);
    }

    public float ObtenerTiempoRecarga() => tiempoRecarga;
}
