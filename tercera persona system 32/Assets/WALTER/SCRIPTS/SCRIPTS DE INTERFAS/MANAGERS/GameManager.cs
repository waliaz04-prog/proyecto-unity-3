using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action<int> OnPuntosCambiados;

    [Header("Balance")]
    [SerializeField] private float multiplicadorPuntosPorOleada = 0.2f;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = true;

    [Header("Enemigos")]
    [SerializeField] private int aliensEliminados;
    [SerializeField] private int navesEliminadas;
    [SerializeField] private int enemigosTotalesEliminados;

    [Header("Puntos")]
    [SerializeField] private int puntosActuales;
    [SerializeField] private int puntosGanados;
    [SerializeField] private int puntosGastados;

    [Header("Tiempo")]
    [SerializeField] private float tiempoSobrevivido;

    [Header("Oleadas")]
    [SerializeField] private int oleadaActual = 1;
    [SerializeField] private int oleadaMaxima = 1;

    public int PuntosActuales => puntosActuales;
    public int OleadaActual => oleadaActual;

    // Estadísticas de solo lectura para el panel de Game Over.
    public int AliensEliminados => aliensEliminados;
    public int NavesEliminadas => navesEliminadas;
    public int EnemigosTotalesEliminados => enemigosTotalesEliminados;
    public int PuntosGanados => puntosGanados;
    public int PuntosGastados => puntosGastados;
    public float TiempoSobrevivido => tiempoSobrevivido;
    public int OleadaMaxima => oleadaMaxima;

    // El tiempo solo cuenta durante la partida (no en menús ni en Game Over).
    private bool partidaActiva;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (partidaActiva)
            tiempoSobrevivido += Time.deltaTime;
    }

    // Congela las estadísticas. Llamar cuando el jugador muere,
    // así el tiempo no sigue contando en la pantalla de Game Over.
    public void FinalizarPartida()
    {
        partidaActiva = false;
    }

    public void AgregarPuntos(int cantidad)
    {
        puntosActuales += cantidad;
        puntosGanados += cantidad;
        OnPuntosCambiados?.Invoke(puntosActuales);
        if (mostrarLogs) Debug.Log("Puntos: " + puntosActuales);
    }

    public bool GastarPuntos(int cantidad)
    {
        if (puntosActuales < cantidad) return false;
        puntosActuales -= cantidad;
        puntosGastados += cantidad;
        OnPuntosCambiados?.Invoke(puntosActuales);
        return true;
    }

    public void RegistrarAlienEliminado()
    {
        aliensEliminados++;
        enemigosTotalesEliminados++;
    }

    public void RegistrarNaveEliminada()
    {
        navesEliminadas++;
        enemigosTotalesEliminados++;
    }

    public void CambiarOleada(int nuevaOleada)
    {
        oleadaActual = nuevaOleada;
        if (oleadaActual > oleadaMaxima)
            oleadaMaxima = oleadaActual;
    }

    public float ObtenerMultiplicadorPuntos()
    {
        return 1f + (oleadaActual - 1) * multiplicadorPuntosPorOleada;
    }

    // Reinicia las estadísticas de la partida. Llamar al iniciar/reiniciar la escena de juego.
    // oleadaMaxima NO se resetea: es el récord histórico entre partidas.
    public void ReiniciarPartida()
    {
        puntosActuales = 0;
        puntosGanados = 0;
        puntosGastados = 0;
        aliensEliminados = 0;
        navesEliminadas = 0;
        enemigosTotalesEliminados = 0;
        tiempoSobrevivido = 0f;
        oleadaActual = 1;
        partidaActiva = true;
        OnPuntosCambiados?.Invoke(puntosActuales);
        if (mostrarLogs) Debug.Log("GameManager: partida reiniciada");
    }
}
