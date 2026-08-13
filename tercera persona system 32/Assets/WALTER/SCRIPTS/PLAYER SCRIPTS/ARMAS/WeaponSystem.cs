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

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoDisparo;
    [SerializeField] private AudioClip sonidoRecarga;
    [SerializeField] private AudioClip sonidoSinMunicion;
    [SerializeField] private AudioClip sonidoMelee;

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

    public bool UsaMunicion =>
        tipoArma == WeaponType.Firearm && usarMunicion;

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        camaraPrincipal = Camera.main;

        ValidarConfiguracion();
    }

    private void OnDisable()
    {
        recargando = false;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
            return;

        switch (tipoArma)
        {
            case WeaponType.Melee:
                ActualizarMelee();
                break;

            case WeaponType.Firearm:
                ActualizarArmaDeFuego();
                break;
        }
    }

    private void ActualizarMelee()
    {
        if (Time.time < siguienteAtaque)
            return;

        if (Input.GetMouseButtonDown(0))
            AtaqueMelee();
    }

    private void ActualizarArmaDeFuego()
    {
        ActualizarRecarga();

        if (Input.GetKeyDown(KeyCode.R))
            IniciarRecarga();

        if (recargando)
            return;

        if (Time.time < siguienteAtaque)
            return;

        bool disparar = armaAutomatica
            ? Input.GetMouseButton(0)
            : Input.GetMouseButtonDown(0);

        if (disparar)
            Disparar();
    }

    private void AtaqueMelee()
    {
        siguienteAtaque = Time.time + tiempoEntreAtaques;

        if (animator != null)
            animator.SetTrigger("Atacar");

        ReproducirSonidoMelee();

        ActivarMelee();

        CancelInvoke(nameof(DesactivarMelee));
        Invoke(nameof(DesactivarMelee), tiempoHitbox);
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
        if (usarMunicion && municionEnCargador <= 0)
        {
            ReproducirSonidoSinMunicion();

            if (mostrarLogs)
                Debug.Log($"{gameObject.name}: No hay munición.");

            return;
        }

        siguienteAtaque = Time.time + tiempoEntreAtaques;

        if (usarMunicion)
            municionEnCargador--;

        if (efectoDisparo != null)
            efectoDisparo.Play();

        ReproducirSonidoDisparo();

        for (int i = 0; i < balasPorDisparo; i++)
            CrearBala();
    }

    private void CrearBala()
    {
        if (puntoDisparo == null)
        {
            if (mostrarLogs)
                Debug.LogWarning($"{gameObject.name}: No hay PuntoDisparo asignado.");

            return;
        }

        if (PoolManager.Instance == null)
        {
            if (mostrarLogs)
                Debug.LogWarning($"{gameObject.name}: No existe PoolManager.");

            return;
        }

        if (camaraPrincipal == null)
            camaraPrincipal = Camera.main;

        if (camaraPrincipal == null)
        {
            if (mostrarLogs)
                Debug.LogWarning($"{gameObject.name}: No se encontró la cámara principal.");

            return;
        }

        Ray ray = camaraPrincipal.ViewportPointToRay(centroViewport);

        Vector3 objetivo;

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            objetivo = hit.point;
        }
        else
        {
            objetivo = ray.origin + ray.direction * 1000f;
        }

        Vector3 direccion = objetivo - puntoDisparo.position;

        if (direccion.sqrMagnitude <= 0.0001f)
            return;

        direccion.Normalize();

        if (dispersion > 0f)
        {
            direccion += new Vector3(
                Random.Range(-dispersion, dispersion),
                Random.Range(-dispersion, dispersion),
                Random.Range(-dispersion, dispersion)
            );

            direccion.Normalize();
        }

        Quaternion rotacion = Quaternion.LookRotation(direccion);

        GameObject balaObj = PoolManager.Instance.ObtenerObjeto(
            idPoolBala,
            puntoDisparo.position,
            rotacion
        );

        if (balaObj == null)
        {
            if (mostrarLogs)
                Debug.LogWarning(
                    $"{gameObject.name}: No se pudo obtener una bala del pool '{idPoolBala}'."
                );

            return;
        }

        if (balaObj.TryGetComponent(out Bala bala))
        {
            bala.Configurar(
                danio,
                velocidadBala,
                tiempoVidaBala,
                atravesarEnemigos,
                disparadoPorJugador: true
            );
        }
        else if (mostrarLogs)
        {
            Debug.LogWarning(
                $"{balaObj.name}: El objeto obtenido del pool no tiene componente Bala."
            );
        }
    }

    public void IniciarRecarga()
    {
        if (!usarMunicion)
            return;

        if (recargando)
            return;

        if (municionEnCargador >= tamanoCargador)
            return;

        if (municionReserva <= 0)
            return;

        recargando = true;
        finRecarga = Time.time + tiempoRecarga;

        if (animator != null)
            animator.SetTrigger("Recargar");

        ReproducirSonidoRecarga();

        if (mostrarLogs)
            Debug.Log($"{gameObject.name}: Recargando...");
    }

    private void ActualizarRecarga()
    {
        if (!recargando)
            return;

        if (Time.time >= finRecarga)
            CompletarRecarga();
    }

    private void CompletarRecarga()
    {
        recargando = false;

        int faltante = tamanoCargador - municionEnCargador;

        int transferido = Mathf.Min(
            faltante,
            municionReserva
        );

        municionEnCargador += transferido;
        municionReserva -= transferido;

        if (mostrarLogs)
        {
            Debug.Log(
                $"{gameObject.name}: Recarga completada. " +
                $"Cargador: {municionEnCargador}/{tamanoCargador}. " +
                $"Reserva: {municionReserva}/{reservaMaxima}."
            );
        }
    }

    private void ReproducirSonidoDisparo()
    {
        if (sonidoDisparo == null)
            return;

        if (AudioManager.Instance == null)
            return;

        Vector3 posicion = puntoDisparo != null
            ? puntoDisparo.position
            : transform.position;

        AudioManager.Instance.ReproducirClip3D(
            sonidoDisparo,
            posicion
        );
    }

    private void ReproducirSonidoRecarga()
    {
        if (sonidoRecarga == null)
            return;

        if (AudioManager.Instance == null)
            return;

        Vector3 posicion = transform.position;

        AudioManager.Instance.ReproducirClip3D(
            sonidoRecarga,
            posicion
        );
    }

    private void ReproducirSonidoSinMunicion()
    {
        if (sonidoSinMunicion == null)
            return;

        if (AudioManager.Instance == null)
            return;

        Vector3 posicion = transform.position;

        AudioManager.Instance.ReproducirClip3D(
            sonidoSinMunicion,
            posicion
        );
    }

    private void ReproducirSonidoMelee()
    {
        if (sonidoMelee == null)
            return;

        if (AudioManager.Instance == null)
            return;

        Vector3 posicion = transform.position;

        AudioManager.Instance.ReproducirClip3D(
            sonidoMelee,
            posicion
        );
    }

    public void AgregarMunicionReserva(int cantidad)
    {
        if (cantidad <= 0)
            return;

        municionReserva = Mathf.Min(
            municionReserva + cantidad,
            reservaMaxima
        );
    }

    public float ObtenerDanio()
    {
        return danio;
    }

    public WeaponType ObtenerTipoArma()
    {
        return tipoArma;
    }

    public TipoMunicion ObtenerTipoMunicion()
    {
        return tipoMunicion;
    }

    public int ObtenerMunicionCargador()
    {
        return municionEnCargador;
    }

    public int ObtenerTamanoCargador()
    {
        return tamanoCargador;
    }

    public int ObtenerMunicionReserva()
    {
        return municionReserva;
    }

    public int ObtenerReservaMaxima()
    {
        return reservaMaxima;
    }

    public float ObtenerTiempoRecarga()
    {
        return tiempoRecarga;
    }

    public void SubirDano(float cantidad)
    {
        if (cantidad <= 0f)
            return;

        danio += cantidad;
    }

    public void MejorarCadencia(float reduccion)
    {
        if (reduccion <= 0f)
            return;

        tiempoEntreAtaques = Mathf.Max(
            0.05f,
            tiempoEntreAtaques - reduccion
        );
    }

    public void SubirCapacidadMunicion(int cantidad)
    {
        if (cantidad <= 0)
            return;

        tamanoCargador += cantidad;
        municionEnCargador += cantidad;

        reservaMaxima += Mathf.RoundToInt(
            cantidad * multiplicadorMejoraReserva
        );

        municionEnCargador = Mathf.Min(
            municionEnCargador,
            tamanoCargador
        );

        municionReserva = Mathf.Min(
            municionReserva,
            reservaMaxima
        );
    }

    public void MejorarVelocidadRecarga(float reduccion)
    {
        if (reduccion <= 0f)
            return;

        tiempoRecarga = Mathf.Max(
            0.1f,
            tiempoRecarga - reduccion
        );
    }

    private void ValidarConfiguracion()
    {
        if (tipoArma == WeaponType.Firearm)
        {
            if (puntoDisparo == null && mostrarLogs)
            {
                Debug.LogWarning(
                    $"{gameObject.name}: Falta asignar PuntoDisparo."
                );
            }

            if (string.IsNullOrWhiteSpace(idPoolBala) && mostrarLogs)
            {
                Debug.LogWarning(
                    $"{gameObject.name}: El ID del pool de bala está vacío."
                );
            }

            if (balasPorDisparo < 1)
                balasPorDisparo = 1;

            if (tamanoCargador < 1)
                tamanoCargador = 1;

            if (municionEnCargador < 0)
                municionEnCargador = 0;

            if (municionEnCargador > tamanoCargador)
                municionEnCargador = tamanoCargador;

            if (municionReserva < 0)
                municionReserva = 0;

            if (reservaMaxima < 0)
                reservaMaxima = 0;
        }
    }
}