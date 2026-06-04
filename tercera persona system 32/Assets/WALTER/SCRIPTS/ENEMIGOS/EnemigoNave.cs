using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemigoNave : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField]
    private Transform jugador;

    [Header("Zona de Vuelo")]
    [SerializeField]
    private ZonaVueloNaves zonaVuelo;

    [Header("Movimiento")]
    [SerializeField]
    private float velocidadMovimiento = 10f;

    [SerializeField]
    private float aceleracion = 20f;

    [SerializeField]
    private float velocidadRotacion = 5f;

    [SerializeField]
    private float distanciaDeteccion = 40f;

    [SerializeField]
    private float distanciaAtaque = 20f;

    [SerializeField]
    private float distanciaNuevoDestino = 5f;

    [SerializeField]
    private float tiempoMaximoDestino = 5f;

    [Header("Altura")]
    [SerializeField]
    private Transform modeloVisual;

    [SerializeField]
    private float alturaVisual = 10f;

    [Header("Spawn Enemigos")]
    [SerializeField]
    private string idPoolAlien = "alien";

    [SerializeField]
    private Transform puntoSpawn;

    [SerializeField]
    private float tiempoAntesGenerar = 10f;

    [SerializeField]
    private float tiempoEntreSpawns = 5f;

    [SerializeField]
    private int maximoAliensPorNave = 10;

    [SerializeField]
    private int maximoAliensGlobales = 100;

    [Header("NavMesh")]
    [SerializeField]
    private float radioBusquedaNavMesh = 10f;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    [SerializeField]
    private bool mostrarGizmos = true;

    private NavMeshAgent agent;

    private Vector3 destinoActual;

    private float timerDestino;

    private int aliensGenerados;

    private static int aliensActivos;

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();

        ConfigurarAgente();

        BuscarJugador();
    }

    private void OnEnable()
    {
        aliensGenerados = 0;

        VerificarNavMesh();

        GenerarNuevoDestino();

        StartCoroutine(
            RutinaSpawn()
        );
    }

    private void OnDisable()
    {
        StopAllCoroutines();
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

        float distanciaJugador =
            Vector3.Distance(
                transform.position,
                jugador.position
            );

        if (distanciaJugador <=
            distanciaDeteccion)
        {
            SeguirJugador();

            if (distanciaJugador <=
                distanciaAtaque)
            {
                AtacarJugador();
            }
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
        agent.speed =
            velocidadMovimiento;

        agent.acceleration =
            aceleracion;

        agent.updateRotation =
            false;

        agent.updateUpAxis =
            false;
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
        }
    }

    private void VerificarNavMesh()
    {
        if (agent.isOnNavMesh)
            return;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            transform.position,
            out hit,
            20f,
            NavMesh.AllAreas))
        {
            agent.Warp(
                hit.position
            );
        }
    }

    #region Movimiento

    private void Patrullar()
    {
        if (zonaVuelo == null)
            return;

        timerDestino +=
            Time.deltaTime;

        float distancia =
            Vector3.Distance(
                transform.position,
                destinoActual
            );

        if (distancia <=
            distanciaNuevoDestino ||
            timerDestino >=
            tiempoMaximoDestino)
        {
            GenerarNuevoDestino();
        }

        agent.SetDestination(
            destinoActual
        );
    }

    private void GenerarNuevoDestino()
    {
        timerDestino = 0f;

        Vector3 punto =
            zonaVuelo.ObtenerPuntoAleatorio();

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            punto,
            out hit,
            radioBusquedaNavMesh,
            NavMesh.AllAreas))
        {
            destinoActual =
                hit.position;
        }
    }

    private void SeguirJugador()
    {
        agent.SetDestination(
            jugador.position
        );
    }

    private void Rotar()
    {
        Vector3 direccion =
            agent.velocity;

        direccion.y = 0f;

        if (direccion.sqrMagnitude <
            0.01f)
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

    private void ActualizarAlturaVisual()
    {
        if (modeloVisual == null)
            return;

        Vector3 posicion =
            modeloVisual.localPosition;

        posicion.y =
            alturaVisual;

        modeloVisual.localPosition =
            posicion;
    }

    #endregion

    #region Ataque

    private void AtacarJugador()
    {
        // Aquí irán los proyectiles
        // cuando los creemos.
    }

    #endregion

    #region Spawn

    private IEnumerator RutinaSpawn()
    {
        yield return new WaitForSeconds(
            tiempoAntesGenerar
        );

        while (true)
        {
            if (aliensGenerados <
                maximoAliensPorNave)
            {
                if (aliensActivos <
                    maximoAliensGlobales)
                {
                    CrearAlien();
                }
            }

            yield return new WaitForSeconds(
                tiempoEntreSpawns
            );
        }
    }

    private void CrearAlien()
    {
        if (PoolManager.Instance == null)
            return;

        if (puntoSpawn == null)
            return;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            puntoSpawn.position,
            out hit,
            radioBusquedaNavMesh,
            NavMesh.AllAreas))
        {
            GameObject alien =
                PoolManager.Instance
                .ObtenerObjeto(
                    idPoolAlien,
                    hit.position,
                    Quaternion.identity
                );

            if (alien == null)
                return;

            aliensGenerados++;
            aliensActivos++;

            ControladorEnemigo controlador =
                alien.GetComponent
                <ControladorEnemigo>();

            if (controlador != null)
            {
                controlador.OnEnemyDeath -=
                    ReducirContador;

                controlador.OnEnemyDeath +=
                    ReducirContador;
            }

            if (mostrarLogs)
            {
                Debug.Log(
                    "Alien generado por nave"
                );
            }
        }
    }

    private void ReducirContador()
    {
        aliensActivos--;

        if (aliensActivos < 0)
        {
            aliensActivos = 0;
        }
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos)
            return;

        Gizmos.color =
            Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaDeteccion
        );

        Gizmos.color =
            Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            distanciaAtaque
        );
    }
}