using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] private VidaPlayer vidaPlayer;
    [SerializeField] private EscudoPlayer escudoPlayer;
    [SerializeField] private PlayerMovimiento playerMovimiento;
    [SerializeField] private WeaponSwitcher weaponSwitcher;

    [Header("Barras")]
    [SerializeField] private Slider barraVida;
    [SerializeField] private Slider barraEscudo;
    [SerializeField] private Slider barraEstamina;
    [SerializeField] private float velocidadLerp = 8f;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI textoMunicion;
    [SerializeField] private TextMeshProUGUI textoOleada;
    [SerializeField] private TextMeshProUGUI textoMultiplicador;
    [SerializeField] private TextMeshProUGUI textoPuntos;

    private float vidaObjetivo;
    private float escudoObjetivo;
    private float estaminaObjetivo;

    private int municionCacheada = -1;
    private int oleadaCacheada = -1;

    private void Start()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuntosCambiados += ActualizarPuntos;

        RefrescarUI();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPuntosCambiados -= ActualizarPuntos;
    }

    private void Update()
    {
        ActualizarBarras();
        ActualizarTextosDinamicos();
    }

    private void ActualizarBarras()
    {
        float delta = velocidadLerp * Time.deltaTime;

        if (vidaPlayer != null && barraVida != null)
        {
            vidaObjetivo = vidaPlayer.ObtenerPorcentajeVida();
            barraVida.value = Mathf.Lerp(barraVida.value, vidaObjetivo, delta);
        }

        if (escudoPlayer != null && barraEscudo != null)
        {
            escudoObjetivo = escudoPlayer.ObtenerPorcentajeEscudo();
            barraEscudo.value = Mathf.Lerp(barraEscudo.value, escudoObjetivo, delta);
        }

        if (playerMovimiento != null && barraEstamina != null)
        {
            estaminaObjetivo = playerMovimiento.ObtenerPorcentajeEstamina();
            barraEstamina.value = Mathf.Lerp(barraEstamina.value, estaminaObjetivo, delta);
        }
    }

    private void ActualizarTextosDinamicos()
    {
        if (GameManager.Instance == null) return;

        // Oleada: actualizar solo cuando cambia
        int oleada = GameManager.Instance.OleadaActual;
        if (oleada != oleadaCacheada)
        {
            oleadaCacheada = oleada;
            if (textoOleada != null)
                textoOleada.text = "Oleada: " + oleada;
        }

        // Multiplicador: se actualiza junto con la oleada
        if (textoMultiplicador != null)
        {
            float mult = GameManager.Instance.ObtenerMultiplicadorPuntos();
            textoMultiplicador.text = "x" + mult.ToString("F1");
        }
    }

    // Fuerza actualización inmediata sin lerp. Llamar al iniciar la escena.
    public void RefrescarUI()
    {
        if (vidaPlayer != null && barraVida != null)
            barraVida.value = vidaPlayer.ObtenerPorcentajeVida();

        if (escudoPlayer != null && barraEscudo != null)
            barraEscudo.value = escudoPlayer.ObtenerPorcentajeEscudo();

        if (playerMovimiento != null && barraEstamina != null)
            barraEstamina.value = playerMovimiento.ObtenerPorcentajeEstamina();

        if (GameManager.Instance != null)
        {
            ActualizarPuntos(GameManager.Instance.PuntosActuales);

            oleadaCacheada = GameManager.Instance.OleadaActual;
            if (textoOleada != null)
                textoOleada.text = "Oleada: " + oleadaCacheada;
        }
    }

    private void ActualizarPuntos(int puntos)
    {
        if (textoPuntos != null)
            textoPuntos.text = "Puntos: " + puntos;
    }
}
