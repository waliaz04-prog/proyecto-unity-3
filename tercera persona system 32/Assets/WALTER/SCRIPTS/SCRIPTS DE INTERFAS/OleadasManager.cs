using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OleadasManager : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField]
    private Transform jugador;

    [Header("Áreas Spawn")]
    [SerializeField]
    private AreaSpawn[] areasSpawn;

    [Header("Pool")]
    [SerializeField]
    private string idPoolAlien = "alien";

    [Header("Oleadas")]
    [SerializeField]
    private int enemigosIniciales = 10;

    [SerializeField]
    private int incrementoPorOleada = 3;

    [SerializeField]
    private float tiempoEntreOleadas = 15f;

    [SerializeField]
    private float tiempoEntreSpawns = 0.15f;

    [Header("Distancias")]
    [SerializeField]
    private float distanciaMinimaJugador = 20f;

    [SerializeField]
    private float radioBusquedaNavMesh = 10f;

    [SerializeField]
    private int intentosBusqueda = 20;

    [Header("Límites")]
    [SerializeField]
    private int maximoEnemigosSimultaneos = 250;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    private readonly List<GameObject>
        enemigosVivos =
            new List<GameObject>();

    private bool generandoOleada;

    private int oleadaActual;

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
        yield return new WaitForSeconds(
            2f
        );

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

        if (GameManager.Instance != null)
        {
            GameManager.Instance
                .CambiarOleada(
                    oleadaActual
                );
        }

        int cantidadEnemigos =
            enemigosIniciales +
            (
                (oleadaActual - 1)
                * incrementoPorOleada
            );

        cantidadEnemigos =
            Mathf.Min(
                cantidadEnemigos,
                maximoEnemigosSimultaneos
            );

        if (mostrarLogs)
        {
            Debug.Log(
                $"Oleada {oleadaActual} - Enemigos {cantidadEnemigos}"
            );
        }

        for (int i = 0;
             i < cantidadEnemigos;
             i++)
        {
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

        Vector3 posicionSpawn;

        if (!ObtenerPosicionValida(
            out posicionSpawn))
        {
            return;
        }

        GameObject enemigo =
            PoolManager.Instance
            .ObtenerObjeto(
                idPoolAlien,
                posicionSpawn,
                Quaternion.identity
            );

        if (enemigo == null)
            return;

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
        enemigosVivos.RemoveAll(
            enemigo =>
                enemigo == null ||
                !enemigo.activeInHierarchy
        );
    }

    private bool ObtenerPosicionValida(
        out Vector3 posicionFinal)
    {
        posicionFinal =
            Vector3.zero;

        if (areasSpawn.Length == 0)
            return false;

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

            Vector3 punto =
                area.ObtenerPuntoAleatorio();

            if (jugador != null)
            {
                float distancia =
                    Vector3.Distance(
                        jugador.position,
                        punto
                    );

                if (distancia <
                    distanciaMinimaJugador)
                {
                    continue;
                }
            }

            NavMeshHit hit;

            if (NavMesh.SamplePosition(
                punto,
                out hit,
                radioBusquedaNavMesh,
                NavMesh.AllAreas))
            {
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
            generandoOleada
        )
        {
            enemigosVivos.RemoveAll(
                enemigo =>
                    enemigo == null ||
                    !enemigo.activeInHierarchy
            );

            yield return null;
        }
    }
}