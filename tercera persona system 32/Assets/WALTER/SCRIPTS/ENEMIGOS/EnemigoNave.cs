using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoNave : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Transform jugador;

    [Header("Visual")]
    [SerializeField] private Transform modeloVisual;
    [SerializeField] private float alturaVisual = 10f;

    [Header("Zona de Vuelo")]
    [SerializeField] private ZonaVueloNaves zonaVuelo;

    [SerializeField]
    private float distanciaParaNuevoDestino = 5f;

    [SerializeField]
    private float tiempoMaximoDestino = 5f;

    [Header("Movimiento")]
    [SerializeField]
    private float distanciaDeteccion = 30f;

    [SerializeField]
    private float distanciaAtaque = 15f;

    [SerializeField]
    private float velocidadMovimiento = 8f;

    [SerializeField]
    private float aceleracion = 20f;

    [SerializeField]
    private float velocidadRotacion = 5f;

    [Header("NavMesh")]
    [SerializeField]
    private float radioAgente = 2f;

    [SerializeField]
    private ObstacleAvoidanceType obstacleAvoidance =
        ObstacleAvoidanceType.HighQualityObstacleAvoidance;

    [Header("Ataque")]
    [SerializeField]
    private GameObject proyectilPrefab;

    [SerializeField]
    private Transform puntoDisparo;

    [SerializeField]
    private float tiempoEntreDisparos = 2f;

    [Header("Spawn Enemigos")]
    [SerializeField]
    private GameObject enemigoPrefab;

    [SerializeField]
    private Transform puntoSpawn;

    [SerializeField]
    private float tiempoAntesGenerar = 10f;

    [SerializeField]
    private float tiempoEntreSpawns = 5f;

    [SerializeField]
    private int maximoPorNave = 10;

    [SerializeField]
    private int maximoPorHorda = 30;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarGizmos = true;

    private NavMeshAgent agent;

    private Vector3 destinoActual;

    private float timerDestino;
    private float timerDisparo;

    private int enemigosGenerados;

    private static int enemigosTotalesVivos;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        ConfigurarAgente();

        BuscarJugador();
    }

    private void Start()
    {
        VerificarNavMesh();

        GenerarNuevoDestino();

        StartCoroutine(IniciarSpawn());
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

        if (distancia <= distanciaDeteccion)
        {
            SeguirJugador();

            if (distancia <= distanciaAtaque)
            {
                AtacarJugador();
            }
        }
        else
        {
            Patrullar();
        }

        AjustarAlturaVisual();

        RotarHaciaMovimiento();
    }

    #region Jugador

    private void BuscarJugador()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            jugador = player.transform;
        }
    }

    #endregion

    #region Configuración

    private void ConfigurarAgente()
    {
        agent.speed = velocidadMovimiento;

        agent.acceleration = aceleracion;

        agent.radius = radioAgente;

        agent.updateRotation = false;

        agent.updateUpAxis = false;

        agent.obstacleAvoidanceType =
            obstacleAvoidance;
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

    #endregion

    #region Movimiento

    private void SeguirJugador()
    {
        agent.SetDestination(
            jugador.position
        );
    }

    private void Patrullar()
    {
        if (zonaVuelo == null)
            return;

        timerDestino += Time.deltaTime;

        float distancia =
            Vector3.Distance(
                transform.position,
                destinoActual
            );

        if (distancia <= distanciaParaNuevoDestino ||
            timerDestino >= tiempoMaximoDestino)
        {
            GenerarNuevoDestino();
        }

        agent.SetDestination(destinoActual);
    }

    private void GenerarNuevoDestino()
    {
        timerDestino = 0f;

        Vector3 puntoAleatorio =
            zonaVuelo.ObtenerPuntoAleatorio();

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            puntoAleatorio,
            out hit,
            20f,
            NavMesh.AllAreas))
        {
            destinoActual = hit.position;
        }
    }

    private void AjustarAlturaVisual()
    {
        if (modeloVisual == null)
            return;

        Vector3 posicion =
            modeloVisual.localPosition;

        posicion.y = alturaVisual;

        modeloVisual.localPosition =
            posicion;
    }

    private void RotarHaciaMovimiento()
    {
        Vector3 direccion =
            agent.velocity;

        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.01f)
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

    #endregion

    #region Ataque

    private void AtacarJugador()
    {
        timerDisparo += Time.deltaTime;

        if (timerDisparo >= tiempoEntreDisparos)
        {
            timerDisparo = 0f;

            if (proyectilPrefab != null &&
                puntoDisparo != null)
            {
                Instantiate(
                    proyectilPrefab,
                    puntoDisparo.position,
                    puntoDisparo.rotation
                );
            }
        }
    }

    #endregion

    #region Spawn

    private IEnumerator IniciarSpawn()
    {
        yield return new WaitForSeconds(
            tiempoAntesGenerar
        );

        while (enemigosGenerados <
               maximoPorNave)
        {
            if (enemigosTotalesVivos <
                maximoPorHorda)
            {
                CrearEnemigo();
            }

            yield return new WaitForSeconds(
                tiempoEntreSpawns
            );
        }
    }

    private void CrearEnemigo()
    {
        if (enemigoPrefab == null)
            return;

        if (puntoSpawn == null)
            return;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            puntoSpawn.position,
            out hit,
            10f,
            NavMesh.AllAreas))
        {
            GameObject enemigo =
                Instantiate(
                    enemigoPrefab,
                    hit.position,
                    puntoSpawn.rotation
                );

            enemigosGenerados++;

            enemigosTotalesVivos++;

            ControladorEnemigo controlador =
                enemigo.GetComponent<ControladorEnemigo>();

            if (controlador != null)
            {
                controlador.OnEnemyDeath +=
                    ReducirContador;
            }
        }
    }

    private void ReducirContador()
    {
        enemigosTotalesVivos--;

        if (enemigosTotalesVivos < 0)
        {
            enemigosTotalesVivos = 0;
        }
    }

    #endregion

    #region Gizmos

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos)
            return;

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaDeteccion
        );

        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaAtaque
        );
    }

    #endregion
}