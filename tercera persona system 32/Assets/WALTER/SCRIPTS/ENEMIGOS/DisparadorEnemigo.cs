using UnityEngine;

public class DisparadorEnemigo : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField]
    private float danio = 10f;

    [Header("Ataque")]
    [SerializeField]
    private float tiempoEntreDisparos = 2f;

    [SerializeField]
    private float distanciaAtaque = 20f;

    [Header("Objetivo")]
    [SerializeField]
    private Transform objetivo;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    private float siguienteDisparo;

    private void Awake()
    {
        BuscarJugador();
    }

    private void BuscarJugador()
    {
        GameObject jugador =
            GameObject.FindGameObjectWithTag(
                "Player"
            );

        if (jugador != null)
        {
            objetivo =
                jugador.transform;
        }
    }

    public void ConfigurarObjetivo(
        Transform nuevoObjetivo)
    {
        objetivo =
            nuevoObjetivo;
    }

    public void IntentarDisparar()
    {
        if (objetivo == null)
        {
            BuscarJugador();
            return;
        }

        if (Time.time <
            siguienteDisparo)
        {
            return;
        }

        float distancia =
            Vector3.Distance(
                transform.position,
                objetivo.position
            );

        if (distancia >
            distanciaAtaque)
        {
            return;
        }

        siguienteDisparo =
            Time.time +
            tiempoEntreDisparos;

        VidaPlayer vidaPlayer =
            objetivo.GetComponent<
                VidaPlayer>();

        if (vidaPlayer != null)
        {
            vidaPlayer.RecibirDanio(
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
}