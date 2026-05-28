using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OleadasManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject[] enemigosPrefabs;

    [Header("Áreas Spawn")]
    [SerializeField]
    private AreaSpawn[] areasSpawn;

    [Header("Jugador")]
    [SerializeField]
    private Transform jugador;

    [Header("Oleadas")]
    [SerializeField]
    private int enemigosIniciales = 5;

    [SerializeField]
    private int incrementoPorOleada = 3;

    [SerializeField]
    private float tiempoEntreOleadas = 10f;

    [SerializeField]
    private int oleadaActual = 0;

    [Header("Distancias Seguridad")]
    [SerializeField]
    private float distanciaMinimaJugador = 20f;

    [SerializeField]
    private float radioBusquedaNavMesh = 10f;

    [SerializeField]
    private int intentosBusqueda = 20;

    [Header("Límites")]
    [SerializeField]
    private int maximoEnemigosSimultaneos = 100;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = true;

    private readonly List<GameObject> enemigosVivos =
        new List<GameObject>();

    private bool generandoOleada;

    public int OleadaActual =>
        oleadaActual;

    private void Start()
    {
        StartCoroutine(
            BucleOleadas()
        );
    }

    private IEnumerator BucleOleadas()
    {
        yield return new WaitForSeconds(2f);

        while (true)
        {
            yield return StartCoroutine(
                IniciarOleada()
            );

            yield return StartCoroutine(
                EsperarFinOleada()
            );

            if (mostrarLogs)
            {
                Debug.Log(
                    "Oleada completada: " +
                    oleadaActual
                );
            }

            yield return new WaitForSeconds(
                tiempoEntreOleadas
            );
        }
    }

    private IEnumerator IniciarOleada()
    {
        generandoOleada = true;

        oleadaActual++;

        int cantidadEnemigos =
            enemigosIniciales +
            ((oleadaActual - 1) *
             incrementoPorOleada);

        cantidadEnemigos =
            Mathf.Min(
                cantidadEnemigos,
                maximoEnemigosSimultaneos
            );

        if (mostrarLogs)
        {
            Debug.Log(
                "Iniciando Oleada: " +
                oleadaActual +
                " | Enemigos: " +
                cantidadEnemigos
            );
        }

        for (int i = 0; i < cantidadEnemigos; i++)
        {
            CrearEnemigo();

            yield return new WaitForSeconds(
                0.2f
            );
        }

        generandoOleada = false;
    }

    private void CrearEnemigo()
    {
        if (enemigosPrefabs.Length == 0)
            return;

        if (areasSpawn.Length == 0)
            return;

        Vector3 posicionSpawn;

        if (!ObtenerPosicionValida(
            out posicionSpawn))
        {
            return;
        }

        GameObject prefab =
            enemigosPrefabs[
                Random.Range(
                    0,
                    enemigosPrefabs.Length
                )
            ];

        GameObject enemigo =
            Instantiate(
                prefab,
                posicionSpawn,
                Quaternion.identity
            );

        enemigosVivos.Add(
            enemigo
        );

        StatsEnemigo stats =
            enemigo.GetComponent<StatsEnemigo>();

        if (stats != null)
        {
            stats.ConfigurarPorOleada(
                oleadaActual
            );
        }

        ControladorEnemigo controlador =
            enemigo.GetComponent<ControladorEnemigo>();

        if (controlador != null)
        {
            controlador.OnEnemyDeath +=
                () =>
                {
                    enemigosVivos.Remove(
                        enemigo
                    );
                };
        }
    }

    private bool ObtenerPosicionValida(
        out Vector3 posicionFinal)
    {
        posicionFinal =
            Vector3.zero;

        for (int i = 0;
             i < intentosBusqueda;
             i++)
        {
            AreaSpawn area =
                areasSpawn[
                    Random.Range(
                        0,
                        areasSpawn.Length
                    )
                ];

            Vector3 puntoAleatorio =
                area.ObtenerPuntoAleatorio();

            if (jugador != null)
            {
                float distanciaJugador =
                    Vector3.Distance(
                        jugador.position,
                        puntoAleatorio
                    );

                if (distanciaJugador <
                    distanciaMinimaJugador)
                {
                    continue;
                }
            }

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                puntoAleatorio,
                out hit,
                radioBusquedaNavMesh,
                NavMesh.AllAreas))
            {
                Collider[] colisiones =
                    Physics.OverlapSphere(
                        hit.position,
                        1f
                    );

                bool posicionBloqueada =
                    false;

                foreach (Collider colision
                    in colisiones)
                {
                    if (!colision.isTrigger)
                    {
                        posicionBloqueada =
                            true;

                        break;
                    }
                }

                if (!posicionBloqueada)
                {
                    posicionFinal =
                        hit.position;

                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator EsperarFinOleada()
    {
        while (enemigosVivos.Count > 0 ||
               generandoOleada)
        {
            enemigosVivos.RemoveAll(
                enemigo => enemigo == null
            );

            yield return null;
        }
    }
}