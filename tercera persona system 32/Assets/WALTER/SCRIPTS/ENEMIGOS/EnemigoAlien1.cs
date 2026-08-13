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
        base.Awake();
    }

    private void ConfigurarAgente()
    {
        if (agent == null) return;
        agent.radius = radioAgente;
        agent.updateRotation = false;
    }

    private void OnEnable()
    {
        VerificarNavMesh();
        timerRuta = 0f;
        estaRetrocediendo = false;
    }

    private void Update()
    {
        if (!TieneJugador()) return;

        RotarHaciaJugador();

        timerRuta += Time.deltaTime;
        if (timerRuta >= frecuenciaActualizacionRuta)
        {
            timerRuta = 0f;
            ActualizarDestino();
        }
    }

    private void ActualizarDestino()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        float distanciaAlJugadorSqr = (jugador.position - transform.position).sqrMagnitude;
        float distMinSqr = (distanciaIdeal - toleranciaDistancia) * (distanciaIdeal - toleranciaDistancia);
        float distMaxSqr = (distanciaIdeal + toleranciaDistancia) * (distanciaIdeal + toleranciaDistancia);

        if (distanciaAlJugadorSqr > distMaxSqr)
        {
            if (estaRetrocediendo)
            {
                estaRetrocediendo = false;
                if (multiplicadorVelocidadRetirada != 1f)
                    agent.speed /= multiplicadorVelocidadRetirada;
            }
            agent.isStopped = false;
            agent.SetDestination(jugador.position);
        }
        else if (distanciaAlJugadorSqr < distMinSqr)
        {
            Vector3 direccionRetirada = (transform.position - jugador.position).normalized;
            Vector3 posicionRetirada = transform.position + direccionRetirada * distanciaIdeal;

            if (NavMesh.SamplePosition(posicionRetirada, out NavMeshHit hit, radioBusquedaNavMesh, NavMesh.AllAreas))
            {
                agent.isStopped = false;
                if (!estaRetrocediendo)
                {
                    estaRetrocediendo = true;
                    if (multiplicadorVelocidadRetirada != 1f)
                        agent.speed *= multiplicadorVelocidadRetirada;
                }
                agent.SetDestination(hit.position);
            }
        }
        else
        {
            if (estaRetrocediendo)
            {
                estaRetrocediendo = false;
                if (multiplicadorVelocidadRetirada != 1f)
                    agent.speed /= multiplicadorVelocidadRetirada;
            }
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
    }
}