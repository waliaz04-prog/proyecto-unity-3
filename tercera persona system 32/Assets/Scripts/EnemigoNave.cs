using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoNave : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Transform jugador;

    [Header("Movimiento")]
    [SerializeField] private float distanciaDeteccion = 30f;
    [SerializeField] private float distanciaAtaque = 15f;

    [SerializeField] private float velocidadMovimiento = 10f;
    [SerializeField] private float aceleracion = 20f;
    [SerializeField] private float velocidadRotacion = 5f;

    [SerializeField] private float alturaVuelo = 12f;

    [Header("NavMesh")]
    [SerializeField] private float radioAgente = 2f;

    [SerializeField]
    private ObstacleAvoidanceType calidadAvoidance =
        ObstacleAvoidanceType.HighQualityObstacleAvoidance;

    [Header("Patrulla")]
    [SerializeField] private Transform[] puntosPatrulla;

    [SerializeField]
    private float distanciaCambioPatrulla = 3f;

    [Header("Ataque")]
    [SerializeField] private GameObject proyectilPrefab;

    [SerializeField] private Transform puntoDisparo;

    [SerializeField] private float tiempoEntreDisparos = 2f;

    [Header("Spawn Enemigos")]
    [SerializeField] private GameObject enemigoPrefab;

    [SerializeField] private Transform puntoSpawn;

    [SerializeField]
    private float tiempoAntesDeGenerar = 10f;

    [SerializeField]
    private float tiempoEntreSpawns = 5f;

    [SerializeField]
    private int maximoPorNave = 10;

    [SerializeField]
    private int maximoPorHorda = 30;

    [Header("Debug")]
    [SerializeField] private bool mostrarGizmos = true;

    private NavMeshAgent agent;

    private int indicePatrulla;
    private int enemigosGenerados;

    private static int enemigosTotalesVivos;

    private float timerDisparo;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        ConfigurarAgente();
    }

    private void Start()
    {
        StartCoroutine(IniciarSpawn());
    }

    private void Update()
    {
        if (jugador == null)
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

        AjustarAltura();

        RotarHaciaMovimiento();
    }

    #region Configuración

    private void ConfigurarAgente()
    {
        agent.speed = velocidadMovimiento;

        agent.acceleration = aceleracion;

        agent.radius = radioAgente;

        agent.updateRotation = false;

        agent.updateUpAxis = false;

        agent.obstacleAvoidanceType =
            calidadAvoidance;
    }

    #endregion

    #region Movimiento

    private void SeguirJugador()
    {
        agent.SetDestination(jugador.position);
    }

    private void Patrullar()
    {
        if (puntosPatrulla.Length == 0)
            return;

        Transform puntoActual =
            puntosPatrulla[indicePatrulla];

        agent.SetDestination(
            puntoActual.position
        );

        float distancia =
            Vector3.Distance(
                transform.position,
                puntoActual.position
            );

        if (distancia <= distanciaCambioPatrulla)
        {
            indicePatrulla++;

            if (indicePatrulla >=
                puntosPatrulla.Length)
            {
                indicePatrulla = 0;
            }
        }
    }

    private void AjustarAltura()
    {
        Vector3 posicion =
            transform.position;

        posicion.y = alturaVuelo;

        transform.position = posicion;
    }

    private void RotarHaciaMovimiento()
    {
        Vector3 direccion =
            agent.velocity;

        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.1f)
            return;

        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(
                direccion
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
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
            tiempoAntesDeGenerar
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
        if (enemigoPrefab == null ||
            puntoSpawn == null)
            return;

        GameObject enemigo =
            Instantiate(
                enemigoPrefab,
                puntoSpawn.position,
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

    private void ReducirContador()
    {
        enemigosTotalesVivos--;
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