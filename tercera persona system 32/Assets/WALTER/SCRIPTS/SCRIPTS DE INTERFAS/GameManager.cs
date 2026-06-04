using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event Action<int> OnPuntosCambiados;

    [Header("Balance")]
    [SerializeField]
    private float multiplicadorPuntosPorOleada = 0.2f;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = true;

    [Header("Enemigos Eliminados")]
    [SerializeField]
    private int aliensEliminados;

    [SerializeField]
    private int navesEliminadas;

    [SerializeField]
    private int enemigosTotalesEliminados;

    [Header("Puntos")]
    [SerializeField]
    private int puntosActuales;

    [SerializeField]
    private int puntosGanados;

    [SerializeField]
    private int puntosGastados;

    [Header("Tiempo")]
    [SerializeField]
    private float tiempoSobrevivido;

    [Header("Oleadas")]
    [SerializeField]
    private int oleadaActual = 1;

    [SerializeField]
    private int oleadaMaxima = 1;

    private const string KEY_PUNTOS_ACTUALES = "PUNTOS_ACTUALES";
    private const string KEY_PUNTOS_GANADOS = "PUNTOS_GANADOS";
    private const string KEY_PUNTOS_GASTADOS = "PUNTOS_GASTADOS";

    public int PuntosActuales => puntosActuales;
    public int PuntosGanados => puntosGanados;
    public int PuntosGastados => puntosGastados;

    public int AliensEliminados => aliensEliminados;
    public int NavesEliminadas => navesEliminadas;
    public int EnemigosTotalesEliminados => enemigosTotalesEliminados;

    public int OleadaActual => oleadaActual;
    public int OleadaMaxima => oleadaMaxima;

    public float TiempoSobrevivido => tiempoSobrevivido;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);

        CargarDatos();
    }

    private void Start()
    {
        OnPuntosCambiados?.Invoke(
            puntosActuales
        );
    }

    private void Update()
    {
        tiempoSobrevivido +=
            Time.deltaTime;
    }

    #region Guardado

    private void GuardarDatos()
    {
        PlayerPrefs.SetInt(
            KEY_PUNTOS_ACTUALES,
            puntosActuales
        );

        PlayerPrefs.SetInt(
            KEY_PUNTOS_GANADOS,
            puntosGanados
        );

        PlayerPrefs.SetInt(
            KEY_PUNTOS_GASTADOS,
            puntosGastados
        );

        PlayerPrefs.Save();
    }

    private void CargarDatos()
    {
        puntosActuales =
            PlayerPrefs.GetInt(
                KEY_PUNTOS_ACTUALES,
                0
            );

        puntosGanados =
            PlayerPrefs.GetInt(
                KEY_PUNTOS_GANADOS,
                0
            );

        puntosGastados =
            PlayerPrefs.GetInt(
                KEY_PUNTOS_GASTADOS,
                0
            );
    }

    #endregion

    #region Puntos

    public void AgregarPuntos(
        int cantidad)
    {
        puntosActuales += cantidad;

        puntosGanados += cantidad;

        GuardarDatos();

        OnPuntosCambiados?.Invoke(
            puntosActuales
        );

        if (mostrarLogs)
        {
            Debug.Log(
                $"Puntos +{cantidad} | Total {puntosActuales}"
            );
        }
    }

    public bool GastarPuntos(
        int cantidad)
    {
        if (puntosActuales < cantidad)
        {
            return false;
        }

        puntosActuales -= cantidad;

        puntosGastados += cantidad;

        GuardarDatos();

        OnPuntosCambiados?.Invoke(
            puntosActuales
        );

        return true;
    }

    #endregion

    #region Enemigos

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

    #endregion

    #region Oleadas

    public void CambiarOleada(
        int nuevaOleada)
    {
        oleadaActual =
            nuevaOleada;

        if (oleadaActual >
            oleadaMaxima)
        {
            oleadaMaxima =
                oleadaActual;
        }
    }

    public float ObtenerMultiplicadorPuntos()
    {
        return 1f +
               (
                (oleadaActual - 1)
                * multiplicadorPuntosPorOleada
               );
    }

    #endregion

    [ContextMenu("Resetear Puntos")]
    public void ResetearPuntos()
    {
        puntosActuales = 0;
        puntosGanados = 0;
        puntosGastados = 0;

        GuardarDatos();

        OnPuntosCambiados?.Invoke(
            puntosActuales
        );
    }
}