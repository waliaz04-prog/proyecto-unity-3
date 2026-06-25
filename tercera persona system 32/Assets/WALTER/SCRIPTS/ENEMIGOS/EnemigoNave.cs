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

    [Header("Spawn Enemigos")]
    [SerializeField] private string idPoolAlien = "alien";
    [SerializeField] private Transform puntoSpawn;
    [SerializeField] private float tiempoAntesGenerar = 10f;
    [SerializeField] private float tiempoEntreSpawns = 5f;
    [SerializeField] private int maximoAliensPorNave = 10;
    [SerializeField] private int maximoAliensGlobales = 100;

    [Header("NavMesh")]
    [SerializeField] private float radioBusquedaNavMesh = 10f;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = false;
    [SerializeField] private bool mostrarGizmos = true;

    private NavMeshAgent agent;
    private AtaqueEnemigo ataqueEnemigo;
    private Vector3 destinoActual;
    private float timerDestino;
    private int aliensGenerados;

    // Contador de instancia en lugar de static para evitar bugs al reusar desde pool.
    // El conteo global se gestiona a través de suscripción/desuscripción en CrearAlien.
    private static int aliensActivosGlobal;

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
        aliensGenerados = 0;
        VerificarNavMesh();
        GenerarNuevoDestino();
        StartCoroutine(RutinaSpawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        if (!TieneJugador()) return;

        if (!agent.isOnNavMesh) return;

        float distanciaJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaJugador <= distanciaDeteccion)
        {
            SeguirJugador();
            if (distanciaJugador <= distanciaAtaque)
                AtacarJugador();
        }
        else
        {
            Patrullar();
        }

        Rotar();
        ActualizarAlturaVisual();
    }

    private void ConfigurarAgente()
    {
        // speed y acceleration los controla StatsEnemigo
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    // Llamar desde SceneInitializer al cargar una escena nueva para limpiar el pool de naves.
    public static void ResetearContadorGlobal()
    {
        aliensActivosGlobal = 0;
    }

    private void VerificarNavMesh()
    {
        if (agent.isOnNavMesh) return;
        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 20f, NavMesh.AllAreas))
            agent.Warp(hit.position);
    }

    private void Patrullar()
    {
        if (zonaVuelo == null) return;

        timerDestino += Time.deltaTime;
        float distancia = Vector3.Distance(transform.position, destinoActual);

        if (distancia <= distanciaNuevoDestino || timerDestino >= tiempoMaximoDestino)
            GenerarNuevoDestino();

        agent.SetDestination(destinoActual);
    }

    private void GenerarNuevoDestino()
    {
        timerDestino = 0f;
        Vector3 punto = zonaVuelo.ObtenerPuntoAleatorio();
        if (NavMesh.SamplePosition(punto, out NavMeshHit hit, radioBusquedaNavMesh, NavMesh.AllAreas))
            destinoActual = hit.position;
    }

    private void SeguirJugador()
    {
        agent.SetDestination(jugador.position);
    }

    private void Rotar()
    {
        Vector3 direccion = agent.velocity;
        direccion.y = 0f;
        if (direccion.sqrMagnitude < 0.01f) return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
    }

    private void ActualizarAlturaVisual()
    {
        if (modeloVisual == null) return;
        Vector3 posicion = modeloVisual.localPosition;
        posicion.y = alturaVisual;
        modeloVisual.localPosition = posicion;
    }

    private void AtacarJugador()
    {
        if (ataqueEnemigo != null)
            ataqueEnemigo.IntentarAtacar();
    }

    private IEnumerator RutinaSpawn()
    {
        yield return new WaitForSeconds(tiempoAntesGenerar);

        while (true)
        {
            if (aliensGenerados < maximoAliensPorNave && aliensActivosGlobal < maximoAliensGlobales)
                CrearAlien();

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

        ControladorEnemigo controlador = alien.GetComponent<ControladorEnemigo>();
        if (controlador != null)
        {
            controlador.OnEnemyDeath -= ReducirContador;
            controlador.OnEnemyDeath += ReducirContador;
        }

        if (mostrarLogs) Debug.Log("Alien generado por nave");
    }

    private void ReducirContador()
    {
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
