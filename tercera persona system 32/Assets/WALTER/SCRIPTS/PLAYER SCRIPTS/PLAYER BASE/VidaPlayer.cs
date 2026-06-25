using UnityEngine;
using UnityEngine.SceneManagement;

public class VidaPlayer : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 100f;

    [Header("Game Over")]
    [SerializeField] private string escenaGameOver = "GameOver";
    [SerializeField] private float tiempoAntesCambiarEscena = 2f;

    public float VidaActual { get; private set; }
    public bool EstaMuerto { get; private set; }

    private PlayerMovimiento movimientoJugador;
    private EscudoPlayer escudoPlayer;

    private void Awake()
    {
        VidaActual = vidaMaxima;
        movimientoJugador = GetComponent<PlayerMovimiento>();
        escudoPlayer = GetComponent<EscudoPlayer>();
    }

    public void RecibirDanio(float cantidad)
    {
        if (EstaMuerto) return;

        if (escudoPlayer != null && escudoPlayer.EscudoActual > 0)
        {
            cantidad = escudoPlayer.RecibirDanioEscudo(cantidad);
            if (cantidad <= 0) return;
        }

        VidaActual = Mathf.Clamp(VidaActual - cantidad, 0, vidaMaxima);

        if (VidaActual <= 0)
            Morir();
    }

    public void CurarVida(float cantidad)
    {
        if (EstaMuerto) return;
        VidaActual = Mathf.Clamp(VidaActual + cantidad, 0, vidaMaxima);
    }

    public float VidaMaxima => vidaMaxima;

    public void SubirVidaMaxima(float cantidad)
    {
        vidaMaxima += cantidad;
        // Al subir el máximo, también curar la diferencia
        VidaActual = Mathf.Clamp(VidaActual + cantidad, 0f, vidaMaxima);
    }

    private void Morir()
    {
        if (EstaMuerto) return;
        EstaMuerto = true;

        if (movimientoJugador != null)
            movimientoJugador.Morir();

        Invoke(nameof(IrAGameOver), tiempoAntesCambiarEscena);
    }

    private void IrAGameOver()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaGameOver);
    }

    public float ObtenerPorcentajeVida() => VidaActual / vidaMaxima;
}
