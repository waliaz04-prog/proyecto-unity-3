using UnityEngine;

[RequireComponent(typeof(PoolObject))]
[RequireComponent(typeof(Collider))]
public class Bala : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = false;

    private float danio;
    private float velocidad;
    private float tiempoVida;
    private bool atravesarEnemigos;
    private float tiempoActual;
    private bool esDelJugador;

    private PoolObject poolObject;

    private void Awake()
    {
        poolObject = GetComponent<PoolObject>();
    }

    private void OnEnable()
    {
        tiempoActual = 0f;
    }

    public void Configurar(float nuevoDanio, float nuevaVelocidad, float nuevoTiempoVida, bool puedeAtravesar, bool disparadoPorJugador = true)
    {
        danio = nuevoDanio;
        velocidad = nuevaVelocidad;
        tiempoVida = nuevoTiempoVida;
        atravesarEnemigos = puedeAtravesar;
        esDelJugador = disparadoPorJugador;
        tiempoActual = 0f;
    }

    private void Update()
    {
        transform.position += transform.forward * velocidad * Time.deltaTime;
        tiempoActual += Time.deltaTime;
        if (tiempoActual >= tiempoVida)
            DesactivarBala();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger) return;

        // Si la bala es del jugador, ignorar colisión con el jugador (filtro por owner)
        if (esDelJugador && other.CompareTag("Player"))
            return;

        // Si es bala enemiga, ignorar colisión con enemigos
        if (!esDelJugador && !other.CompareTag("Player"))
        {
            StatsEnemigo enemigoPropio = other.GetComponentInParent<StatsEnemigo>();
            if (enemigoPropio != null) return;
        }

        StatsEnemigo enemigo = other.GetComponentInParent<StatsEnemigo>();
        if (enemigo != null)
        {
            enemigo.RecibirDanio(danio);
            if (mostrarLogs) Debug.Log("Impacto enemigo");
            if (!atravesarEnemigos) DesactivarBala();
            return;
        }

        VidaPlayer jugador = other.GetComponentInParent<VidaPlayer>();
        if (jugador != null)
        {
            jugador.RecibirDanio(danio);
            if (mostrarLogs) Debug.Log("Impacto jugador");
            DesactivarBala();
            return;
        }

        DesactivarBala();
    }

    private void DesactivarBala()
    {
        if (poolObject != null)
            poolObject.RegresarAlPool();
        else
            gameObject.SetActive(false);
    }
}
