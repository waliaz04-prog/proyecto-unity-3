using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField]
    protected Transform jugador;

    private static Transform jugadorGlobal;

    protected virtual void Awake()
    {
        BuscarJugador();
    }

    protected void BuscarJugador()
    {
        if (jugadorGlobal != null)
        {
            jugador = jugadorGlobal;
            return;
        }

        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (player != null)
        {
            jugadorGlobal =
                player.transform;

            jugador =
                jugadorGlobal;
        }
    }

    protected bool TieneJugador()
    {
        if (jugador == null)
        {
            BuscarJugador();
        }

        return jugador != null;
    }

    public void ConfigurarJugador(
        Transform nuevoJugador)
    {
        jugador =
            nuevoJugador;

        jugadorGlobal =
            nuevoJugador;
    }
}