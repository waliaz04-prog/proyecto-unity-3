using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OleadasManager : MonoBehaviour
{
    [System.Serializable]
    public class ConfiguracionEnemigo
    {
        [Header("Pool")]
        public string idPool = "alien";

        [Header("Aparición")]
        [Range(1, 100)]
        public int probabilidad = 100;

        public int oleadaMinima = 1;

        public int oleadaMaxima = 0;

        [Header("Límite")]
        public int maximoSimultaneos = 999;
    }

    [Header("Jugador")]
    [SerializeField]
    private Transform jugador;

    [Header("Áreas Spawn")]
    [SerializeField]
    private AreaSpawn[] areasSpawn;

    [Header("Tipos de Enemigos")]
    [SerializeField]
    private ConfiguracionEnemigo[] enemigosDisponibles;

    [Header("Oleadas")]
    [SerializeField]
    private int enemigosPrimeraOleada = 10;

    [SerializeField]
    private int incrementoPorOleada = 3;

    [SerializeField]
    private float esperaPrimeraOleada = 10f;

    [SerializeField]
    private float tiempoEntreOleadas = 15f;

    [SerializeField]
    private float tiempoEntreSpawns = 0.15f;

    [Header("Spawn")]
    [SerializeField]
    private float distanciaMinimaJugador = 20f;

    [SerializeField]
    private float radioBusquedaNavMesh = 10f;

    [SerializeField]
    private int intentosBusqueda = 30;

    [SerializeField]
    private float distanciaMinimaEntreEnemigos = 2f;

    [Header("Límites")]
    [SerializeField]
    private int maximoEnemigosSimultaneos = 250;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    private readonly List<GameObject>
        enemigosVivos =
        new List<GameObject>();

    private readonly Dictionary<string, int>
        enemigosActivos =
        new Dictionary<string, int>();

    private bool generandoOleada;

    private int oleadaActual;

    public int OleadaActual
    {
        get
        {
            return oleadaActual;
        }
    }

    private void Start()
    {
        StartCoroutine(
            BucleOleadas()
        );
    }

    private IEnumerator BucleOleadas()
    {
        yield return new WaitForSeconds(
            esperaPrimeraOleada
        );

        while (true)
        {
            yield return StartCoroutine(
                IniciarOleada()
            );

            yield return StartCoroutine(
                EsperarFinOleada()
            );

            yield return new WaitForSeconds(
                tiempoEntreOleadas
            );
        }
    }

    private IEnumerator IniciarOleada()
    {
        generandoOleada = true;

        oleadaActual++;

        if (GameManager.Instance != null)
        {
            GameManager.Instance
                .CambiarOleada(
                    oleadaActual
                );
        }

        int cantidadEnemigos =
            enemigosPrimeraOleada +
            (
                (oleadaActual - 1)
                * incrementoPorOleada
            );

        if (mostrarLogs)
        {
            Debug.Log(
                "Oleada "
                + oleadaActual
                + " - Enemigos "
                + cantidadEnemigos
            );
        }

        for (
            int i = 0;
            i < cantidadEnemigos;
            i++
        )
        {
            while (
                enemigosVivos.Count >=
                maximoEnemigosSimultaneos
            )
            {
                LimpiarLista();

                yield return null;
            }

            CrearEnemigo();

            yield return new WaitForSeconds(
                tiempoEntreSpawns
            );
        }

        generandoOleada = false;
    }

    private void CrearEnemigo()
    {
        if (PoolManager.Instance == null)
            return;

        Vector3 posicion;

        if (!ObtenerPosicionValida(
            out posicion))
        {
            return;
        }

        string idPool =
            ObtenerEnemigoAleatorio();

        if (string.IsNullOrEmpty(
            idPool))
        {
            return;
        }

        GameObject enemigo =
            PoolManager.Instance
            .ObtenerObjeto(
                idPool,
                posicion,
                Quaternion.identity
            );

        if (enemigo == null)
            return;

        enemigosVivos.Add(
            enemigo
        );

        if (!enemigosActivos.ContainsKey(
            idPool))
        {
            enemigosActivos.Add(
                idPool,
                0
            );
        }

        enemigosActivos[
            idPool
        ]++;

        StatsEnemigo stats =
            enemigo.GetComponent
            <StatsEnemigo>();

        if (stats != null)
        {
            stats.ConfigurarPorOleada(
                oleadaActual
            );
        }

        ControladorEnemigo controlador =
            enemigo.GetComponent
            <ControladorEnemigo>();

        if (controlador != null)
        {
            controlador.OnEnemyDeath -=
                ActualizarListaEnemigos;

            controlador.OnEnemyDeath +=
                ActualizarListaEnemigos;
        }

    }
    private void ActualizarListaEnemigos()
    {
        LimpiarLista();
    }

    private void LimpiarLista()
    {
        enemigosVivos.RemoveAll(
            enemigo =>
                enemigo == null ||
                !enemigo.activeInHierarchy
        );
    }

    private string ObtenerEnemigoAleatorio()
    {
        List<ConfiguracionEnemigo>
            candidatos =
            new List<ConfiguracionEnemigo>();

        int pesoTotal = 0;

        foreach (
            ConfiguracionEnemigo enemigo
            in enemigosDisponibles)
        {
            if (
                oleadaActual <
                enemigo.oleadaMinima)
            {
                continue;
            }

            if (
                enemigo.oleadaMaxima > 0 &&
                oleadaActual >
                enemigo.oleadaMaxima)
            {
                continue;
            }

            int activos = 0;

            enemigosActivos.TryGetValue(
                enemigo.idPool,
                out activos
            );

            if (
                activos >=
                enemigo.maximoSimultaneos)
            {
                continue;
            }

            candidatos.Add(
                enemigo
            );

            pesoTotal +=
                enemigo.probabilidad;
        }

        if (
            candidatos.Count == 0)
        {
            return "";
        }

        int numero =
            Random.Range(
                0,
                pesoTotal
            );

        int acumulado = 0;

        foreach (
            ConfiguracionEnemigo enemigo
            in candidatos)
        {
            acumulado +=
                enemigo.probabilidad;

            if (
                numero <
                acumulado)
            {
                return enemigo.idPool;
            }
        }

        return candidatos[0]
            .idPool;
    }

    private bool ObtenerPosicionValida(
        out Vector3 posicionFinal)
    {
        posicionFinal =
            Vector3.zero;

        if (
            areasSpawn == null ||
            areasSpawn.Length == 0
        )
        {
            return false;
        }

        for (
            int intento = 0;
            intento < intentosBusqueda;
            intento++
        )
        {
            AreaSpawn area =
                areasSpawn[
                    Random.Range(
                        0,
                        areasSpawn.Length
                    )
                ];

            if (area == null)
            {
                continue;
            }

            Vector3 punto =
                area.ObtenerPuntoAleatorio();

            if (jugador != null)
            {
                float distancia =
                    Vector3.Distance(
                        jugador.position,
                        punto
                    );

                if (
                    distancia <
                    distanciaMinimaJugador
                )
                {
                    continue;
                }
            }

            NavMeshHit hit;

            if (
                NavMesh.SamplePosition(
                    punto,
                    out hit,
                    radioBusquedaNavMesh,
                    NavMesh.AllAreas
                )
            )
            {
                bool ocupado =
                    false;

                foreach (
                    GameObject enemigo
                    in enemigosVivos
                )
                {
                    if (
                        enemigo == null ||
                        !enemigo.activeInHierarchy
                    )
                    {
                        continue;
                    }

                    if (
                        Vector3.Distance(
                            enemigo.transform.position,
                            hit.position
                        ) <
                        distanciaMinimaEntreEnemigos
                    )
                    {
                        ocupado =
                            true;

                        break;
                    }
                }

                if (ocupado)
                {
                    continue;
                }

                posicionFinal =
                    hit.position;

                return true;
            }
        }

        return false;
    }

    private IEnumerator EsperarFinOleada()
    {
        while (
            enemigosVivos.Count > 0 ||
            generandoOleada)
        {
            LimpiarLista();

            yield return null;
        }
    }
}