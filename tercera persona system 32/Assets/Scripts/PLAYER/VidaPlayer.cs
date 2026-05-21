using UnityEngine;

public class VidaPlayer : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField] private float vidaMaxima = 100f;

    public float VidaActual { get; private set; }

    public bool EstaMuerto { get; private set; }

    private PlayerMovimiento movimientoJugador;
    private EscudoPlayer escudoPlayer;

    private void Awake()
    {
        VidaActual = vidaMaxima;

        movimientoJugador =
            GetComponent<PlayerMovimiento>();

        escudoPlayer =
            GetComponent<EscudoPlayer>();
    }

    // RECIBIR DAÑO
    public void RecibirDanio(float cantidad)
    {
        if (EstaMuerto)
            return;

        // Primero daño al escudo
        if (escudoPlayer != null &&
            escudoPlayer.EscudoActual > 0)
        {
            cantidad =
                escudoPlayer.RecibirDanioEscudo(cantidad);

            if (cantidad <= 0)
                return;
        }

        // Daño restante a vida
        VidaActual -= cantidad;

        VidaActual = Mathf.Clamp(
            VidaActual,
            0,
            vidaMaxima
        );

        Debug.Log(
            "Vida actual: " + VidaActual
        );

        if (VidaActual <= 0)
        {
            Morir();
        }
    }

    // CURAR VIDA
    public void CurarVida(float cantidad)
    {
        if (EstaMuerto)
            return;

        VidaActual += cantidad;

        VidaActual = Mathf.Clamp(
            VidaActual,
            0,
            vidaMaxima
        );
    }

    // MORIR
    private void Morir()
    {
        if (EstaMuerto)
            return;

        EstaMuerto = true;

        Debug.Log("Jugador murió");

        if (movimientoJugador != null)
        {
            movimientoJugador.Morir();
        }
    }

    // PORCENTAJE VIDA
    public float ObtenerPorcentajeVida()
    {
        return VidaActual / vidaMaxima;
    }
}