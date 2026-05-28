using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OleadasManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField]
    private GameObject[] enemigosPrefabs;

    [Header("Puntos Spawn")]
    [SerializeField]
    private Transform[] puntosSpawn;

    [Header("Oleadas")]
    [SerializeField]
    private int enemigosIniciales = 5;

    [SerializeField]
    private int incrementoPorOleada = 3;

    [SerializeField]
    private float tiempoEntreOleadas = 10f;

    [SerializeField]
    private int oleadaActual = 0;

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
                0.3f
            );
        }

        generandoOleada = false;
    }

    private void CrearEnemigo()
    {
        if (enemigosPrefabs.Length == 0)
            return;

        if (puntosSpawn.Length == 0)
            return;

        GameObject prefab =
            enemigosPrefabs[
                Random.Range(
                    0,
                    enemigosPrefabs.Length
                )
            ];

        Transform punto =
            puntosSpawn[
                Random.Range(
                    0,
                    puntosSpawn.Length
                )
            ];

        GameObject enemigo =
            Instantiate(
                prefab,
                punto.position,
                punto.rotation
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