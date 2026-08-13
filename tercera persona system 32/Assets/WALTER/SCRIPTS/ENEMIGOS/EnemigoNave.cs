using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoNave : EnemyBase
{
    [Header("Zona de Vuelo")]
    [SerializeField] private ZonaVueloNaves zonaVuelo;

    [Header("Movimiento")]
    [SerializeField] private float velocidadRotacion = 5f;
    [SerializeField] private float distanciaDeteccion = 40f;
    [SerializeField] private float distanciaAtaque = 20f;
    [SerializeField] private float distanciaNuevoDestino = 5f;
    [SerializeField] private float tiempoMaximoDestino = 5f;

    [Header("Altura")]
    [SerializeField] private Transform modeloVisual;
    [SerializeField] private float alturaVisual = 10f;

    [Header("Giro Visual (Efecto Platillo Volador)")]
    [Tooltip("Grados por segundo. El modelo gira todo el tiempo sobre su propio eje.")]
    [SerializeField] private float velocidadGiroVisual = 90f;
    [SerializeField] private Vector3 ejeGiroVisual = Vector3.up;

    [Header("Spawn Enemigos")]
    [SerializeField] private string idPoolAlien = "alien";
    [SerializeField] private Transform puntoSpawn;
    [SerializeField] private float tiempoAntesGenerar = 10f;
    [SerializeField] private float tiempoEntreSpawns = 2f;
    [SerializeField] private int maxAliensPorNave = 5;
    [SerializeField] private int maxAliensGlobales = 20;
    [SerializeField] private float radioBusquedaNavMesh = 5f;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = false;
    [SerializeField] private bool mostrarGizmos = true;

    private static int aliensActivosGlobal = 0;

    private NavMeshAgent agent;
    private AtaqueEnemigo ataqueEnemigo;

    private Vector3 destinoActual;
    private float timerDestino;
    private int aliensGenerados;
    private Coroutine rutinaSpawn;

    public static void ResetearContadorGlobal()
    {
        aliensActivosGlobal = 0;
    }

    protected override void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        ataqueEnemigo = GetComponent<AtaqueEnemigo>();

        if (agent != null)
        {
            agent.updateRotation = false;
        }

        base.Awake();
    }

    private void OnEnable()
    {
        aliensGenerados = 0;
        timerDestino = 0f;

        if (zonaVuelo != null)
        {
            destinoActual = zonaVuelo.ObtenerPuntoAleatorio();
        }

        rutinaSpawn = StartCoroutine(RutinaSpawnAliens());
    }

    private void OnDisable()
    {
        if (rutinaSpawn != null)
        {
            StopCoroutine(rutinaSpawn);
            rutinaSpawn = null;
        }
    }

    private void Update()
    {
        RotarModeloVisual();

        if (!TieneJugador()) return;

        ManejarMovimiento();
        RotarHaciaTarget();
    }

    private void RotarModeloVisual()
    {
        if (modeloVisual != null)
        {
            modeloVisual.Rotate(ejeGiroVisual * (velocidadGiroVisual * Time.deltaTime), Space.Self);
        }
    }

    private void ManejarMovimiento()
    {
        if (agent == null || !agent.isOnNavMesh) return;

        timerDestino += Time.deltaTime;

        float distSqr = (destinoActual - transform.position).sqrMagnitude;
        float distDestinoSqr = distanciaNuevoDestino * distanciaNuevoDestino;

        if (distSqr <= distDestinoSqr || timerDestino >= tiempoMaximoDestino)
        {
            ObtenerNuevoDestino();
        }

        agent.SetDestination(destinoActual);
    }

    private void ObtenerNuevoDestino()
    {
        timerDestino = 0f;

        if (zonaVuelo != null)
        {
            destinoActual = zonaVuelo.ObtenerPuntoAleatorio();
        }
        else if (jugador != null)
        {
            Vector3 offsetAleatorio = Random.insideUnitSphere * distanciaAtaque;
            offsetAleatorio.y = 0f;
            destinoActual = jugador.position + offsetAleatorio;
        }
    }

    private void RotarHaciaTarget()
    {
        if (jugador == null) return;

        Vector3 direccion = jugador.position - transform.position;
        direccion.y = 0f;

        if (direccion.sqrMagnitude < 0.01f) return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
    }

    private IEnumerator RutinaSpawnAliens()
    {
        yield return new WaitForSeconds(tiempoAntesGenerar);

        while (true)
        {
            if (aliensGenerados < maxAliensPorNave && aliensActivosGlobal < maxAliensGlobales)
            {
                CrearAlien();
            }

            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }

    private void CrearAlien()
    {
        if (PoolManager.Instance == null || puntoSpawn == null) return;

        if (!NavMesh.SamplePosition(puntoSpawn.position, out NavMeshHit hit, radioBusquedaNavMesh, NavMesh.AllAreas))
            return;

        GameObject alien = PoolManager.Instance.ObtenerObjeto(idPoolAlien, hit.position, Quaternion.identity);
        if (alien == null) return;

        aliensGenerados++;
        aliensActivosGlobal++;

        if (alien.TryGetComponent(out ControladorEnemigo controlador))
        {
            controlador.OnEnemyDeath += ReducirContador;
        }

        if (mostrarLogs) Debug.Log("Alien generado por nave.");
    }

    private void ReducirContador(ControladorEnemigo controlador)
    {
        if (controlador != null)
        {
            controlador.OnEnemyDeath -= ReducirContador;
        }

        aliensActivosGlobal--;
        if (aliensActivosGlobal < 0) aliensActivosGlobal = 0;
    }

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}