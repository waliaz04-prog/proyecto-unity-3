using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Ataque")]
    [SerializeField]
    private float danio = 10f;

    [SerializeField]
    private float tiempoEntreAtaques = 2f;

    [SerializeField]
    private float distanciaAtaque = 2.5f;

    [Header("Jugador")]
    [SerializeField]
    private Transform jugador;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    private float siguienteAtaque;

    private void Awake()
    {
        BuscarJugador();
    }

    private void BuscarJugador()
    {
        if (jugador != null)
            return;

        GameObject player =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (player != null)
        {
            jugador =
                player.transform;
        }
    }

    public void ConfigurarJugador(
        Transform nuevoJugador)
    {
        jugador =
            nuevoJugador;
    }

    public void ConfigurarDanio(
        float nuevoDanio)
    {
        danio =
            nuevoDanio;
    }

    public void IntentarAtacar()
    {
        if (jugador == null)
        {
            BuscarJugador();
            return;
        }

        if (
            Time.time <
            siguienteAtaque
        )
        {
            return;
        }

        float distancia =
            Vector3.Distance(
                transform.position,
                jugador.position
            );

        if (
            distancia >
            distanciaAtaque
        )
        {
            return;
        }

        siguienteAtaque =
            Time.time +
            tiempoEntreAtaques;

        VidaPlayer vida =
            jugador.GetComponent<
                VidaPlayer>();

        if (vida == null)
            return;

        vida.RecibirDanio(
            danio
        );

        if (mostrarLogs)
        {
            Debug.Log(
                gameObject.name +
                " atacó al jugador."
            );
        }
    }
}