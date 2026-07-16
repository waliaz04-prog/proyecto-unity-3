using UnityEngine;
using UnityEngine.Serialization;

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
    [SerializeField] private float velocidadBala = 80f;
    [SerializeField] private float tiempoVidaBala = 5f;
    [SerializeField] private bool atravesarEnemigos;

    [Header("Munición")]
    [SerializeField] private bool usarMunicion = true;
    [Tooltip("Tipo de balas que usa esta arma. Los ítems de munición de las máquinas venden por tipo.")]
    [SerializeField] private TipoMunicion tipoMunicion = TipoMunicion.Pistola;
    [Tooltip("Balas dentro del arma (cargador)")]
    [FormerlySerializedAs("municionActual")]
    [SerializeField] private int municionEnCargador = 30;
    [Tooltip("Capacidad máxima del cargador")]
    [FormerlySerializedAs("municionMaxima")]
    [SerializeField] private int tamanoCargador = 30;
    [Tooltip("Balas en la mochila (reserva de esta arma)")]
    [SerializeField] private int municionReserva = 90;
    [Tooltip("Máximo de balas que caben en la mochila")]
    [SerializeField] private int reservaMaxima = 300;
    [Tooltip("Segundos que tarda la recarga con la tecla R")]
    [SerializeField] private float tiempoRecarga = 1.5f;
    [Tooltip("Al comprar mejora de capacidad: reserva máxima ganada por cada bala de cargador ganada. Ej: mejora de 10 balas × 3 = +30 de reserva máxima")]
    [SerializeField] private float multiplicadorMejoraReserva = 3f;

    [Header("Pool")]
    [SerializeField] private string idPoolBala = "bala";

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs;

    private float siguienteAtaque;
    private bool recargando;
    private float finRecarga;
    private Camera camaraPrincipal;
    private readonly Vector3 centroViewport = new Vector3(0.5f, 0.5f, 0f);

    public bool EstaRecargando => recargando;
    // True solo si el arma es de fuego y consume balas (para HUD y compras).
    public bool UsaMunicion => tipoArma == WeaponType.Firearm && usarMunicion;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        camaraPrincipal = Camera.main;
    }

    private void OnDisable()
    {
        // Cambiar de arma cancela la recarga en curso.
        // No se pierden balas: la transferencia ocurre solo al completarse.
        recargando = false;
    }

    private void Update()
    {
        switch (tipoArma)
        {
            case WeaponType.Melee:
                if (Time.time >= siguienteAtaque && Input.GetMouseButtonDown(0))
                    AtaqueMelee();
                break;

            case WeaponType.Firearm:
                ActualizarRecarga();

                if (Input.GetKeyDown(KeyCode.R))
                    IniciarRecarga();

                if (recargando || Time.time < siguienteAtaque) break;

                bool disparar = armaAutomatica ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);
                if (disparar) Disparar();
                break;
        }
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
        if (usarMunicion && municionEnCargador <= 0) return;

        siguienteAtaque = Time.time + tiempoEntreAtaques;

        if (usarMunicion)
            municionEnCargador--;

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

    // Inicia la recarga si hace falta y hay balas en reserva. Dura tiempoRecarga segundos.
    public void IniciarRecarga()
    {
        if (!usarMunicion || recargando) return;
        if (municionEnCargador >= tamanoCargador) return;
        if (municionReserva <= 0) return;

        recargando = true;
        finRecarga = Time.time + tiempoRecarga;

        if (animator != null)
            animator.SetTrigger("Recargar");

        if (mostrarLogs) Debug.Log(gameObject.name + " recargando...");
    }

    private void ActualizarRecarga()
    {
        if (recargando && Time.time >= finRecarga)
            CompletarRecarga();
    }

    // Transfiere de la reserva solo lo que falta para llenar el cargador.
    private void CompletarRecarga()
    {
        recargando = false;
        int faltante = tamanoCargador - municionEnCargador;
        int transferido = Mathf.Min(faltante, municionReserva);
        municionEnCargador += transferido;
        municionReserva -= transferido;
    }

    // Suma balas a la mochila de esta arma (compras en máquinas), respetando el máximo.
    public void AgregarMunicionReserva(int cantidad)
    {
        municionReserva = Mathf.Min(municionReserva + cantidad, reservaMaxima);
    }

    public float ObtenerDanio() => danio;
    public WeaponType ObtenerTipoArma() => tipoArma;
    public TipoMunicion ObtenerTipoMunicion() => tipoMunicion;
    public int ObtenerMunicionCargador() => municionEnCargador;
    public int ObtenerTamanoCargador() => tamanoCargador;
    public int ObtenerMunicionReserva() => municionReserva;
    public int ObtenerReservaMaxima() => reservaMaxima;
    public float ObtenerTiempoRecarga() => tiempoRecarga;

    public void SubirDano(float cantidad) => danio += cantidad;

    public void MejorarCadencia(float reduccion)
    {
        tiempoEntreAtaques = Mathf.Max(0.05f, tiempoEntreAtaques - reduccion);
    }

    // La mejora de capacidad agranda el cargador (y regala esas balas)
    // y también sube la reserva máxima según multiplicadorMejoraReserva.
    public void SubirCapacidadMunicion(int cantidad)
    {
        tamanoCargador += cantidad;
        municionEnCargador += cantidad;
        reservaMaxima += Mathf.RoundToInt(cantidad * multiplicadorMejoraReserva);
    }

    public void MejorarVelocidadRecarga(float reduccion)
    {
        tiempoRecarga = Mathf.Max(0.1f, tiempoRecarga - reduccion);
    }
}
