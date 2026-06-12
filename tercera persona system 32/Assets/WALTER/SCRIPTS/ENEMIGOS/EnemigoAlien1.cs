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
    private float distanciaIdeal = 3f;

    [SerializeField]
    private float toleranciaDistancia = 0.5f;

    [SerializeField]
    private float frecuenciaActualizacionRuta = 0.2f;

    [SerializeField]
    private float velocidadMovimiento = 4f;

    [SerializeField]
    private float aceleracion = 8f;

    [Header("Rotación")]
    [SerializeField]
    private float velocidadRotacion = 8f;

    [Header("NavMesh")]
    [SerializeField]
    private float radioAgente = 0.6f;

    [SerializeField]
    private float distanciaWarpNavMesh = 20f;

    [SerializeField]
    private float radioBusquedaNavMesh = 5f;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarGizmos = true;

    private NavMeshAgent agent;
    private DisparadorEnemigo disparadorEnemigo;

    private float timerRuta;

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();

        disparadorEnemigo =
            GetComponent<DisparadorEnemigo>();

        ConfigurarAgente();

        BuscarJugador();
    }

    private void OnEnable()
    {
        timerRuta = 0f;

        if (agent != null)
        {
            agent.enabled = true;
        }

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

        ActualizarMovimiento();

        RotarHaciaJugador();

        if (disparadorEnemigo != null)
        {
            disparadorEnemigo.IntentarDisparar();
        }
    }

    private void ConfigurarAgente()
    {
        agent.speed =
            velocidadMovimiento;

        agent.acceleration =
            aceleracion;

        agent.radius =
            radioAgente;

        agent.updateRotation =
            false;

        agent.updateUpAxis =
            false;
    }

    private void ActualizarMovimiento()
    {
        timerRuta +=
            Time.deltaTime;

        if (timerRuta <
            frecuenciaActualizacionRuta)
        {
            return;
        }

        timerRuta = 0f;

        float distancia =
            Vector3.Distance(
                transform.position,
                jugador.position
            );

        if (distancia >
            distanciaIdeal +
            toleranciaDistancia)
        {
            agent.isStopped =
                false;

            agent.SetDestination(
                jugador.position
            );
        }
        else if (
            distancia <
            distanciaIdeal -
            toleranciaDistancia)
        {
            Vector3 direccion =
                (
                    transform.position -
                    jugador.position
                ).normalized;

            Vector3 posicionRetirada =
                jugador.position +
                (
                    direccion *
                    distanciaIdeal
                );

            NavMeshHit hit;

            if (
                NavMesh.SamplePosition(
                    posicionRetirada,
                    out hit,
                    radioBusquedaNavMesh,
                    NavMesh.AllAreas
                )
            )
            {
                agent.isStopped =
                    false;

                agent.SetDestination(
                    hit.position
                );
            }
        }
        else
        {
            agent.isStopped =
                true;
        }
    }

    private void BuscarJugador()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (player != null)
        {
            jugador =
                player.transform;

            if (disparadorEnemigo != null)
            {
                disparadorEnemigo
                    .ConfigurarObjetivo(
                        jugador
                    );
            }
        }
    }

    private void VerificarNavMesh()
    {
        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;

            if (
                NavMesh.SamplePosition(
                    transform.position,
                    out hit,
                    distanciaWarpNavMesh,
                    NavMesh.AllAreas
                )
            )
            {
                agent.Warp(
                    hit.position
                );
            }
        }
    }

    private void RotarHaciaJugador()
    {
        if (jugador == null)
            return;

        Vector3 direccion =
            jugador.position -
            transform.position;

        direccion.y = 0f;

        if (
            direccion.sqrMagnitude <
            0.01f
        )
        {
            return;
        }

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

        Gizmos.color =
            Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaIdeal
        );

        Gizmos.color =
            Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaIdeal +
            toleranciaDistancia
        );
    }
}