using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Kills Aliens")]
    [SerializeField]
    private int aliensEliminados;

    [Header("Kills Naves")]
    [SerializeField]
    private int navesEliminadas;

    [Header("Kills Totales")]
    [SerializeField]
    private int enemigosTotalesEliminados;

    public int AliensEliminados =>
        aliensEliminados;

    public int NavesEliminadas =>
        navesEliminadas;

    public int EnemigosTotalesEliminados =>
        enemigosTotalesEliminados;

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
    }

    #region REGISTRAR MUERTES

    public void RegistrarAlienEliminado()
    {
        aliensEliminados++;

        ActualizarTotal();
    }

    public void RegistrarNaveEliminada()
    {
        navesEliminadas++;

        ActualizarTotal();
    }

    private void ActualizarTotal()
    {
        enemigosTotalesEliminados =
            aliensEliminados +
            navesEliminadas;

        Debug.Log(
            "Aliens: " + aliensEliminados +
            " | Naves: " + navesEliminadas +
            " | Total: " +
            enemigosTotalesEliminados
        );
    }

    #endregion

    #region RESET

    public void ReiniciarPartida()
    {
        aliensEliminados = 0;

        navesEliminadas = 0;

        enemigosTotalesEliminados = 0;
    }

    #endregion
}