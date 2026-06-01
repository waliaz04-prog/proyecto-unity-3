using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoAlien1 : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Transform jugador;

    [Header("Movimiento")]
    [SerializeField] private float distanciaMinima = 3f;
    [SerializeField] private float velocidadMovimiento = 4f;
    [SerializeField] private float aceleracion = 8f;

    [Header("Rotación")]
    [SerializeField] private float velocidadRotacion = 8f;

    [Header("NavMesh")]
    [SerializeField] private float radioAgente = 0.6f;

    [Header("Debug")]
    [SerializeField] private bool mostrarGizmos = true;

    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = velocidadMovimiento;
        agent.acceleration = aceleracion;
        agent.radius = radioAgente;
        agent.updateRotation = false;
        agent.obstacleAvoidanceType =
            ObstacleAvoidanceType.HighQualityObstacleAvoidance;

        BuscarJugador();
    }

    private void Start()
    {
        VerificarNavMesh();
    }

    private void Update()
    {
        if (jugador == null)
        {
            BuscarJugador();
            return;
        }

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
            agent.SetDestination(jugador.position);
        }
        else
        {
            agent.isStopped = true;
        }

        RotarHaciaJugador();
    }

    private void BuscarJugador()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            jugador = player.transform;
        }
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

        if (direccion.sqrMagnitude < 0.01f)
            return;

        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(direccion);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
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