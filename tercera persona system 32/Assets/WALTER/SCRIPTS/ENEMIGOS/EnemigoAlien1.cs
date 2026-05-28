using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoAlien1 : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField]
    private Transform jugador;

    [Header("Movimiento")]
    [SerializeField]
    private float distanciaMinima = 3f;

    [Header("Rotación")]
    [SerializeField]
    private float velocidadRotacion = 8f;

    [Header("NavMesh")]
    [SerializeField]
    private float radioAgente = 0.6f;

    [SerializeField]
    private float distanciaFrenado = 0f;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarGizmos = true;

    private NavMeshAgent agent;

    private StatsEnemigo stats;

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();

        stats =
            GetComponent<StatsEnemigo>();

        ConfigurarAgente();
    }

    private void Start()
    {
        VerificarNavMesh();
    }

    private void Update()
    {
        if (jugador == null)
            return;

        if (!agent.isOnNavMesh)
            return;

        float distancia =
            Vector3.Distance(
                transform.position,
                jugador.position
            );

        if (distancia > distanciaMinima)
        {
            agent.isStopped = false;

            agent.SetDestination(
                jugador.position
            );
        }
        else
        {
            agent.isStopped = true;
        }

        RotarHaciaJugador();
    }

    private void ConfigurarAgente()
    {
        if (stats != null)
        {
            agent.speed =
                stats.VelocidadMovimiento;

            agent.acceleration =
                stats.Aceleracion;
        }

        agent.radius =
            radioAgente;

        agent.stoppingDistance =
            distanciaFrenado;

        agent.updateRotation = false;

        agent.obstacleAvoidanceType =
            ObstacleAvoidanceType.HighQualityObstacleAvoidance;
    }

    private void VerificarNavMesh()
    {
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                transform.position,
                out hit,
                20f,
                NavMesh.AllAreas))
            {
                agent.Warp(hit.position);
            }
        }
    }

    private void RotarHaciaJugador()
    {
        Vector3 direccion =
            jugador.position - transform.position;

        direccion.y = 0f;

        if (direccion == Vector3.zero)
            return;

        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(
                direccion
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion *
                Time.deltaTime
            );
    }

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaMinima
        );
    }
}