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
        tiempoSobrevivido += Time.deltaTime;
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
}
