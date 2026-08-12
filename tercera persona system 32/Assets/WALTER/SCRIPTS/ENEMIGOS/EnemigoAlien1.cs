using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoAlien1 : EnemyBase
{
    [Header("Movimiento")]
    [SerializeField] private float distanciaIdeal = 3f;
    [SerializeField] private float toleranciaDistancia = 0.5f;
    [SerializeField] private float frecuenciaActualizacionRuta = 0.2f;
    [Tooltip("Multiplica la velocidad del agente solo al retroceder. Útil para aliens a distancia que huyen rápido.")]
    [SerializeField] private float multiplicadorVelocidadRetirada = 1f;

    [Header("Rotación")]
    [SerializeField] private float velocidadRotacion = 8f;

    [Header("NavMesh")]
    [SerializeField] private float radioAgente = 0.6f;
    [SerializeField] private float distanciaWarpNavMesh = 20f;
    [SerializeField] private float radioBusquedaNavMesh = 5f;

    [Header("Debug")]
    [SerializeField] private bool mostrarGizmos = true;

    private NavMeshAgent agent;
    private AtaqueEnemigo ataqueEnemigo;
    private float timerRuta;
    private bool estaRetrocediendo;

    protected override void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ataqueEnemigo = GetComponent<AtaqueEnemigo>();
        ConfigurarAgente();
        base.Awake(); // busca el jugador en caché estático
        if (jugador != null)
            ataqueEnemigo?.ConfigurarObjetivo(jugador);
    }

    private void OnEnable()
    {
        timerRuta = 0f;
        estaRetrocediendo = false;
        if (agent != null) agent.enabled = true;
        VerificarNavMesh();
    }

    private void Update()
    {
        if (!TieneJugador()) return;

        if (!agent.isOnNavMesh) return;

        ActualizarMovimiento();
        RotarHaciaJugador();

        if (ataqueEnemigo != null)
            ataqueEnemigo.IntentarAtacar();
    }

    private void ConfigurarAgente()
    {
        // speed y acceleration los controla StatsEnemigo
        agent.radius = radioAgente;
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void ActualizarMovimiento()
    {
        timerRuta += Time.deltaTime;
        if (timerRuta < frecuenciaActualizacionRuta) return;
        timerRuta = 0f;

        // Restaurar velocidad base antes de recalcular para evitar acumulación
        if (estaRetrocediendo && multiplicadorVelocidadRetirada != 1f)
            agent.speed /= multiplicadorVelocidadRetirada;
        estaRetrocediendo = false;

        float distancia = Vector3.Distance(transform.position, jugador.position);

        if (distancia > distanciaIdeal + toleranciaDistancia)
        {
            agent.isStopped = false;
            agent.SetDestination(jugador.position);
        }
        else if (distancia < distanciaIdeal - toleranciaDistancia)
        {
            Vector3 direccion = (transform.position - jugador.position).normalized;
            Vector3 posicionRetirada = jugador.position + direccion * distanciaIdeal;

            if (NavMesh.SamplePosition(posicionRetirada, out NavMeshHit hit, radioBusquedaNavMesh, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                estaRetrocediendo = true;
                if (multiplicadorVelocidadRetirada != 1f)
                    agent.speed *= multiplicadorVelocidadRetirada;
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void VerificarNavMesh()
    {
        if (!agent.isOnNavMesh && NavMesh.SamplePosition(transform.position, out NavMeshHit hit, distanciaWarpNavMesh, NavMesh.AllAreas))
            agent.Warp(hit.position);
    }

    private void RotarHaciaJugador()
    {
        if (jugador == null) return;

        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.01f) return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
    }

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaIdeal);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaIdeal + toleranciaDistancia);
    }
}
