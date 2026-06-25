using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Jugador")]
    [SerializeField] protected Transform jugador;

    // Cache estático: se invalida en OnDisable para evitar referencias rotas tras reload de escena.
    private static Transform jugadorGlobal;

    protected virtual void Awake()
    {
        BuscarJugador();
    }

    private void OnDisable()
    {
        // Limpiar referencia estática para que el próximo Enable re-busque
        // si el jugador fue destruido (ej. cambio de escena).
        if (jugadorGlobal != null && jugadorGlobal.gameObject == null)
            jugadorGlobal = null;
    }

    protected void BuscarJugador()
    {
        // Revalidar la referencia estática antes de usarla
        if (jugadorGlobal != null && jugadorGlobal.gameObject == null)
            jugadorGlobal = null;

        if (jugadorGlobal != null)
        {
            jugador = jugadorGlobal;
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            jugadorGlobal = player.transform;
            jugador = jugadorGlobal;
        }
    }

    protected bool TieneJugador()
    {
        if (jugador == null || jugador.gameObject == null)
            BuscarJugador();
        return jugador != null;
    }

    public void ConfigurarJugador(Transform nuevoJugador)
    {
        jugador = nuevoJugador;
        jugadorGlobal = nuevoJugador;
    }

    // Llamar desde SceneManager.sceneLoaded o al iniciar nueva escena para limpiar el cache.
    public static void LimpiarCacheJugador()
    {
        jugadorGlobal = null;
    }
}
