using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemigoNave : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private Transform jugador;

    [Header("Movimiento Nave")]
    [SerializeField] private float velocidadMovimiento = 10f;
    [SerializeField] private float distanciaDeteccion = 25f;
    [SerializeField] private float distanciaAtaque = 15f;
    [SerializeField] private float alturaVuelo = 15f;

    [Header("Patrulla")]
    [SerializeField] private Transform[] puntosPatrulla;
    private int indicePatrulla;

    [Header("Ataque")]
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float tiempoEntreDisparos = 2f;

    [Header("Spawn Enemigos")]
    [SerializeField] private GameObject enemigoPrefab;
    [SerializeField] private Transform puntoSpawn;

    [Tooltip("Cuántos segundos espera antes de empezar a generar enemigos")]
    [SerializeField] private float tiempoAntesDeGenerar = 10f;

    [Tooltip("Cada cuántos segundos genera un enemigo")]
    [SerializeField] private float tiempoEntreSpawns = 5f;

    [Tooltip("Máximo de enemigos que esta nave puede generar")]
    [SerializeField] private int maximoPorNave = 10;

    [Tooltip("Máximo de enemigos vivos permitidos por horda")]
    [SerializeField] private int maximoPorHorda = 30;

    private int enemigosGenerados;
    private static int enemigosTotalesVivos;

    private float timerDisparo;

    private void Start()
    {
        StartCoroutine(IniciarSpawn());
    }

    private void Update()
    {
        if (jugador == null)
            return;

        float distancia = Vector3.Distance(transform.position, jugador.position);

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
    }

    #region Movimiento

    private void SeguirJugador()
    {
        Vector3 objetivo = jugador.position;
        objetivo.y += alturaVuelo;

        transform.position = Vector3.MoveTowards(
            transform.position,
            objetivo,
            velocidadMovimiento * Time.deltaTime
        );

        transform.LookAt(jugador);
    }

    private void Patrullar()
    {
        if (puntosPatrulla.Length == 0)
            return;

        Transform objetivo = puntosPatrulla[indicePatrulla];

        Vector3 destino = objetivo.position;
        destino.y += alturaVuelo;

        transform.position = Vector3.MoveTowards(
            transform.position,
            destino,
            velocidadMovimiento * Time.deltaTime
        );

        transform.LookAt(destino);

        float distancia = Vector3.Distance(transform.position, destino);

        if (distancia < 1f)
        {
            indicePatrulla++;

            if (indicePatrulla >= puntosPatrulla.Length)
            {
                indicePatrulla = 0;
            }
        }
    }

    #endregion

    #region Ataque

    private void AtacarJugador()
    {
        timerDisparo += Time.deltaTime;

        if (timerDisparo >= tiempoEntreDisparos)
        {
            timerDisparo = 0f;

            if (proyectilPrefab != null && puntoDisparo != null)
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
        yield return new WaitForSeconds(tiempoAntesDeGenerar);

        while (enemigosGenerados < maximoPorNave)
        {
            if (enemigosTotalesVivos < maximoPorHorda)
            {
                CrearEnemigo();
            }

            yield return new WaitForSeconds(tiempoEntreSpawns);
        }
    }

    private void CrearEnemigo()
    {
        if (enemigoPrefab == null || puntoSpawn == null)
            return;

        GameObject enemigo = Instantiate(
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
            controlador.OnEnemyDeath += ReducirContador;
        }
    }

    private void ReducirContador()
    {
        enemigosTotalesVivos--;
    }

    #endregion

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaAtaque);
    }
}