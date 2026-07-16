using UnityEngine;

// Bala con detección por raycast: lanza un rayo entre la posición actual y la
// siguiente en cada frame, así nunca atraviesa colliders sin detectarlos
// (evita el "tunneling" que ocurre con triggers a alta velocidad).
[RequireComponent(typeof(PoolObject))]
public class Bala : MonoBehaviour
{
    [Header("Colisión")]
    [Tooltip("Capas con las que la bala puede impactar. Excluye aquí capas como efectos o pickups.")]
    [SerializeField] private LayerMask capasImpacto = ~0;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = false;

    private float danio;
    private float velocidad;
    private float tiempoVida;
    private bool atravesarEnemigos;
    private float tiempoActual;
    private bool esDelJugador;

    // Evita dañar dos veces al mismo enemigo cuando la bala lo atraviesa
    // y ese enemigo tiene varios colliders.
    private StatsEnemigo ultimoEnemigoDanado;

    private PoolObject poolObject;

    private void Awake()
    {
        poolObject = GetComponent<PoolObject>();
    }

    private void OnEnable()
    {
        tiempoActual = 0f;
        ultimoEnemigoDanado = null;
    }

    public void Configurar(float nuevoDanio, float nuevaVelocidad, float nuevoTiempoVida, bool puedeAtravesar, bool disparadoPorJugador = true)
    {
        danio = nuevoDanio;
        velocidad = nuevaVelocidad;
        tiempoVida = nuevoTiempoVida;
        atravesarEnemigos = puedeAtravesar;
        esDelJugador = disparadoPorJugador;
        tiempoActual = 0f;
        ultimoEnemigoDanado = null;
    }

    private void Update()
    {
        float distanciaFrame = velocidad * Time.deltaTime;
        Vector3 origen = transform.position;

        // Rayo que cubre exactamente el trayecto de este frame.
        if (Physics.Raycast(origen, transform.forward, out RaycastHit impacto, distanciaFrame, capasImpacto, QueryTriggerInteraction.Ignore))
        {
            bool balaTermino = ProcesarImpacto(impacto);
            if (balaTermino) return;
        }

        transform.position = origen + transform.forward * distanciaFrame;

        tiempoActual += Time.deltaTime;
        if (tiempoActual >= tiempoVida)
            DesactivarBala();
    }

    // Retorna true si la bala terminó su recorrido (volvió al pool).
    private bool ProcesarImpacto(RaycastHit impacto)
    {
        Collider col = impacto.collider;

        // La bala del jugador nunca daña al jugador.
        if (esDelJugador && col.CompareTag("Player"))
            return false;

        StatsEnemigo enemigo = col.GetComponentInParent<StatsEnemigo>();
        if (enemigo != null)
        {
            // Las balas enemigas atraviesan a otros enemigos sin dañarlos.
            if (!esDelJugador) return false;

            if (enemigo != ultimoEnemigoDanado)
            {
                enemigo.RecibirDanio(danio);
                ultimoEnemigoDanado = enemigo;
                if (mostrarLogs) Debug.Log("Impacto enemigo");
            }

            if (atravesarEnemigos) return false;

            DesactivarBala();
            return true;
        }

        VidaPlayer jugador = col.GetComponentInParent<VidaPlayer>();
        if (jugador != null)
        {
            if (!esDelJugador)
            {
                jugador.RecibirDanio(danio);
                if (mostrarLogs) Debug.Log("Impacto jugador");
            }
            DesactivarBala();
            return true;
        }

        // Pared, suelo u otro obstáculo del escenario.
        DesactivarBala();
        return true;
    }

    private void DesactivarBala()
    {
        if (poolObject != null)
            poolObject.RegresarAlPool();
        else
            gameObject.SetActive(false);
    }
}
