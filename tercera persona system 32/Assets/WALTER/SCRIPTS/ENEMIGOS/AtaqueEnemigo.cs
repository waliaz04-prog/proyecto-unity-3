// Fusión de DisparadorEnemigo y EnemyAttack. Usar ModoAtaque para elegir comportamiento.
using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    public enum ModoAtaque
    {
        Melee,
        Distancia
    }

    [Header("Modo")]
    [SerializeField] private ModoAtaque modoAtaque = ModoAtaque.Melee;

    [Header("Daño")]
    [Tooltip("Valor de referencia. StatsEnemigo lo sobreescribe con ConfigurarDanio() al inicializar.")]
    [SerializeField] private float danio = 10f;

    [Header("Ataque")]
    [SerializeField] private float tiempoEntreAtaques = 2f;
    [SerializeField] private float distanciaAtaque = 2.5f;

    [Header("Objetivo")]
    [SerializeField] private Transform objetivo;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = false;

    private float siguienteAtaque;
    private VidaPlayer vidaPlayerCacheada;

    private void Awake()
    {
        // Ajustar distancia por defecto según modo si no fue modificada en Inspector
        if (modoAtaque == ModoAtaque.Distancia && distanciaAtaque == 2.5f)
            distanciaAtaque = 20f;

        BuscarJugador();
    }

    private void BuscarJugador()
    {
        if (objetivo != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            objetivo = player.transform;
            vidaPlayerCacheada = player.GetComponent<VidaPlayer>();
        }
    }

    public void ConfigurarObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
        vidaPlayerCacheada = nuevoObjetivo != null ? nuevoObjetivo.GetComponent<VidaPlayer>() : null;
    }

    public void ConfigurarDanio(float nuevoDanio)
    {
        danio = nuevoDanio;
    }

    public void IntentarAtacar()
    {
        if (objetivo == null)
        {
            BuscarJugador();
            return;
        }

        if (Time.time < siguienteAtaque) return;

        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia > distanciaAtaque) return;

        siguienteAtaque = Time.time + tiempoEntreAtaques;

        if (vidaPlayerCacheada == null)
            vidaPlayerCacheada = objetivo.GetComponent<VidaPlayer>();

        if (vidaPlayerCacheada == null) return;

        vidaPlayerCacheada.RecibirDanio(danio);
        if (mostrarLogs) Debug.Log(gameObject.name + " atacó al jugador (" + modoAtaque + ").");
    }
}
