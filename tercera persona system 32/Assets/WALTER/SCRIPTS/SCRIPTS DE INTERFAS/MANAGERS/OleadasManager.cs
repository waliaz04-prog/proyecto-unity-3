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
        [Range(1, 100)] public int probabilidad = 100;
        public int oleadaMinima = 1;
        public int oleadaMaxima = 0;

        [Header("Límite")]
        public int maximoSimultaneos = 999;
    }

    [Header("Jugador")]
    [SerializeField] private Transform jugador;

    [Header("Áreas Spawn")]
    [SerializeField] private AreaSpawn[] areasSpawn;

    [Header("Tipos de Enemigos")]
    [SerializeField] private ConfiguracionEnemigo[] enemigosDisponibles;

    [Header("Oleadas")]
    [SerializeField] private int enemigosPrimeraOleada = 10;
    [SerializeField] private int incrementoPorOleada = 3;
    [SerializeField] private float esperaPrimeraOleada = 10f;
    [SerializeField] private float tiempoEntreOleadas = 15f;
    [SerializeField] private float tiempoEntreSpawns = 0.15f;

    [Header("Spawn")]
    [SerializeField] private float distanciaMinimaJugador = 20f;
    [SerializeField] private float radioBusquedaNavMesh = 10f;
    [SerializeField] private int intentosBusqueda = 30;
    [SerializeField] private float distanciaMinimaEntreEnemigos = 2f;

    [Header("Límites")]
    [SerializeField] private int maximoEnemigosSimultaneos = 250;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = false;

    private readonly List<GameObject> enemigosVivos = new List<GameObject>();

    // Conteo de enemigos activos por tipo (idPool). Se decrementa en CuandoMuereEnemigo.
    private readonly Dictionary<string, int> enemigosActivos = new Dictionary<string, int>();

    // Mapeo enemigo -> idPool para decrementar el conteo correcto al morir
    private readonly Dictionary<GameObject, string> enemigoAIdPool = new Dictionary<GameObject, string>();

    private bool generandoOleada;
    private int oleadaActual;

    public int OleadaActual => oleadaActual;

    private void Start()
    {
        StartCoroutine(BucleOleadas());
    }

    private IEnumerator BucleOleadas()
    {
        yield return new WaitForSeconds(esperaPrimeraOleada);

        while (true)
        {
            yield return StartCoroutine(IniciarOleada());
            yield return StartCoroutine(EsperarFinOleada());
            yield return new WaitForSeconds(tiempoEntreOleadas);
        }
    }

    private IEnumerator IniciarOleada()
    {
        generandoOleada = true;
        oleadaActual++;

        if (GameManager.Instance != null)
            GameManager.Instance.CambiarOleada(oleadaActual);

        int cantidadEnemigos = enemigosPrimeraOleada + (oleadaActual - 1) * incrementoPorOleada;

        if (mostrarLogs) Debug.Log("Oleada " + oleadaActual + " - Enemigos: " + cantidadEnemigos);

        for (int i = 0; i < cantidadEnemigos; i++)
        {
            while (enemigosVivos.Count >= maximoEnemigosSimultaneos)
            {
                LimpiarLista();
                yield return null;
            }

            CrearEnemigo();
            yield return new WaitForSeconds(tiempoEntreSpawns);
        }

        generandoOleada = false;
    }

    private void CrearEnemigo()
    {
        if (PoolManager.Instance == null) return;
        if (!ObtenerPosicionValida(out Vector3 posicion)) return;

        string idPool = ObtenerEnemigoAleatorio();
        if (string.IsNullOrEmpty(idPool)) return;

        GameObject enemigo = PoolManager.Instance.ObtenerObjeto(idPool, posicion, Quaternion.identity);
        if (enemigo == null) return;

        enemigosVivos.Add(enemigo);
        enemigoAIdPool[enemigo] = idPool;

        if (!enemigosActivos.ContainsKey(idPool))
            enemigosActivos[idPool] = 0;
        enemigosActivos[idPool]++;

        if (enemigo.TryGetComponent(out StatsEnemigo stats))
            stats.ConfigurarPorOleada(oleadaActual);

        if (enemigo.TryGetComponent(out ControladorEnemigo controlador))
        {
            // ControladorEnemigo limpia sus suscripciones en OnDisable,
            // así que basta con suscribirse una vez por spawn.
            controlador.OnEnemyDeath += CuandoMuereEnemigo;
        }
    }

    // El evento entrega el enemigo exacto que murió: se elimina y decrementa
    // directamente, sin escanear la lista buscando inactivos.
    private void CuandoMuereEnemigo(ControladorEnemigo controlador)
    {
        if (controlador == null) return;
        GameObject enemigo = controlador.gameObject;
        enemigosVivos.Remove(enemigo);
        QuitarDeRegistro(enemigo);
    }

    // Decrementa el conteo por tipo y limpia el mapeo. Siempre usar este método
    // al sacar un enemigo del sistema para que los contadores no se corrompan.
    private void QuitarDeRegistro(GameObject enemigo)
    {
        if (enemigo == null || !enemigoAIdPool.TryGetValue(enemigo, out string id)) return;

        enemigoAIdPool.Remove(enemigo);
        if (enemigosActivos.TryGetValue(id, out int activos))
            enemigosActivos[id] = Mathf.Max(0, activos - 1);
    }

    // Red de seguridad: quita enemigos destruidos o desactivados por fuera
    // del flujo normal de muerte (ej. cambio de escena).
    private void LimpiarLista()
    {
        for (int i = enemigosVivos.Count - 1; i >= 0; i--)
        {
            GameObject e = enemigosVivos[i];
            if (e == null || !e.activeInHierarchy)
            {
                QuitarDeRegistro(e);
                enemigosVivos.RemoveAt(i);
            }
        }
    }

    private string ObtenerEnemigoAleatorio()
    {
        List<ConfiguracionEnemigo> candidatos = new List<ConfiguracionEnemigo>();
        int pesoTotal = 0;

        int count = enemigosDisponibles.Length;
        for (int i = 0; i < count; i++)
        {
            ConfiguracionEnemigo cfg = enemigosDisponibles[i];
            if (oleadaActual < cfg.oleadaMinima) continue;
            if (cfg.oleadaMaxima > 0 && oleadaActual > cfg.oleadaMaxima) continue;

            enemigosActivos.TryGetValue(cfg.idPool, out int activos);
            if (activos >= cfg.maximoSimultaneos) continue;

            candidatos.Add(cfg);
            pesoTotal += cfg.probabilidad;
        }

        if (candidatos.Count == 0) return string.Empty;

        int numero = Random.Range(0, pesoTotal);
        int acumulado = 0;
        int candidatoCount = candidatos.Count;

        for (int i = 0; i < candidatoCount; i++)
        {
            acumulado += candidatos[i].probabilidad;
            if (numero < acumulado) return candidatos[i].idPool;
        }

        return candidatos[0].idPool;
    }

    private bool ObtenerPosicionValida(out Vector3 posicionFinal)
    {
        posicionFinal = Vector3.zero;

        if (areasSpawn == null || areasSpawn.Length == 0) return false;

        for (int intento = 0; intento < intentosBusqueda; intento++)
        {
            AreaSpawn area = areasSpawn[Random.Range(0, areasSpawn.Length)];
            if (area == null) continue;

            Vector3 punto = area.ObtenerPuntoAleatorio();

            if (jugador != null && Vector3.Distance(jugador.position, punto) < distanciaMinimaJugador)
                continue;

            if (!NavMesh.SamplePosition(punto, out NavMeshHit hit, radioBusquedaNavMesh, NavMesh.AllAreas))
                continue;

            bool ocupado = false;
            int vivoCount = enemigosVivos.Count;
            for (int j = 0; j < vivoCount; j++)
            {
                GameObject e = enemigosVivos[j];
                if (e == null || !e.activeInHierarchy) continue;
                if (Vector3.Distance(e.transform.position, hit.position) < distanciaMinimaEntreEnemigos)
                {
                    ocupado = true;
                    break;
                }
            }

            if (ocupado) continue;

            posicionFinal = hit.position;
            return true;
        }

        return false;
    }

    private IEnumerator EsperarFinOleada()
    {
        while (enemigosVivos.Count > 0 || generandoOleada)
        {
            LimpiarLista();
            yield return null;
        }
    }
}
