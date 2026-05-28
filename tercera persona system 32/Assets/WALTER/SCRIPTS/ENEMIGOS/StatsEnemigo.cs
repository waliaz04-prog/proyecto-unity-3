using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class StatsEnemigo : MonoBehaviour
{
    [Header("VIDA")]
    [SerializeField]
    private float vidaBase = 100f;

    [SerializeField]
    private float vidaActual;

    [Header("DAÑO")]
    [SerializeField]
    private float danioBase = 10f;

    [Header("ATAQUE")]
    [SerializeField]
    private float velocidadAtaqueBase = 1f;

    [Header("MOVIMIENTO")]
    [SerializeField]
    private float velocidadMovimientoBase = 4f;

    [SerializeField]
    private float aceleracionBase = 8f;

    [Header("DEFENSA")]
    [SerializeField]
    private float resistenciaBase = 0f;

    [Header("ESCALADO POR OLEADA")]
    [SerializeField]
    private float aumentoVida = 0.15f;

    [SerializeField]
    private float aumentoDanio = 0.10f;

    [SerializeField]
    private float aumentoAtaque = 0.05f;

    [SerializeField]
    private float aumentoAceleracion = 0.05f;

    [SerializeField]
    private float aumentoResistencia = 0.03f;

    [Header("STATS FINALES")]
    [SerializeField]
    private float danioActual;

    [SerializeField]
    private float velocidadAtaqueActual;

    [SerializeField]
    private float velocidadMovimientoActual;

    [SerializeField]
    private float aceleracionActual;

    [SerializeField]
    private float resistenciaActual;

    private NavMeshAgent agent;

    private ControladorEnemigo controlador;

    private bool muerto;

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();

        controlador =
            GetComponent<ControladorEnemigo>();

        AplicarStatsBase();
    }

    // APLICAR STATS BASE
    private void AplicarStatsBase()
    {
        vidaActual =
            vidaBase;

        danioActual =
            danioBase;

        velocidadAtaqueActual =
            velocidadAtaqueBase;

        velocidadMovimientoActual =
            velocidadMovimientoBase;

        aceleracionActual =
            aceleracionBase;

        resistenciaActual =
            resistenciaBase;

        AplicarMovimiento();
    }

    // ESCALAR POR OLEADA
    public void ConfigurarPorOleada(
        int oleada
    )
    {
        vidaActual =
            vidaBase *
            (1f + aumentoVida * oleada);

        danioActual =
            danioBase *
            (1f + aumentoDanio * oleada);

        velocidadAtaqueActual =
            velocidadAtaqueBase *
            (1f + aumentoAtaque * oleada);

        aceleracionActual =
            aceleracionBase *
            (1f + aumentoAceleracion * oleada);

        resistenciaActual =
            resistenciaBase *
            (1f + aumentoResistencia * oleada);

        // NO aumenta velocidad
        velocidadMovimientoActual =
            velocidadMovimientoBase;

        AplicarMovimiento();
    }

    // APLICAR MOVIMIENTO
    private void AplicarMovimiento()
    {
        if (agent == null)
            return;

        agent.speed =
            velocidadMovimientoActual;

        agent.acceleration =
            aceleracionActual;
    }

    // RECIBIR DAÑO
    public void RecibirDanio(
        float danio
    )
    {
        if (muerto)
            return;

        float danioFinal =
            Mathf.Max(
                danio - resistenciaActual,
                1f
            );

        vidaActual -= danioFinal;

        Debug.Log(
            gameObject.name +
            " recibió daño: " +
            danioFinal
        );

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    // MORIR
    private void Morir()
    {
        if (muerto)
            return;

        muerto = true;

        Debug.Log(
            gameObject.name +
            " murió"
        );

        if (controlador != null)
        {
            controlador.Morir();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // GETTERS
    public float Vida =>
        vidaActual;

    public float Danio =>
        danioActual;

    public float VelocidadAtaque =>
        velocidadAtaqueActual;

    public float VelocidadMovimiento =>
        velocidadMovimientoActual;

    public float Aceleracion =>
        aceleracionActual;

    public float Resistencia =>
        resistenciaActual;
}