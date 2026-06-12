using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class StatsEnemigo : MonoBehaviour
{
    [Header("Vida")]
    [SerializeField]
    private float vidaBase = 100f;

    [Header("Daño")]
    [SerializeField]
    private float danioBase = 10f;

    [Header("Ataque")]
    [SerializeField]
    private float velocidadAtaqueBase = 1f;

    [Header("Movimiento")]
    [SerializeField]
    private float velocidadMovimientoBase = 4f;

    [SerializeField]
    private float aceleracionBase = 8f;

    [Header("Defensa")]
    [SerializeField]
    private float resistenciaBase = 0f;

    [Header("Recompensa")]
    [SerializeField]
    private int puntosBase = 10;

    [Header("Escalado")]
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

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    private float vidaActual;
    private float danioActual;
    private float velocidadAtaqueActual;
    private float velocidadMovimientoActual;
    private float aceleracionActual;
    private float resistenciaActual;

    private bool muerto;

    private NavMeshAgent agent;

    private ControladorEnemigo controlador;

    private EnemyAttack enemyAttack;

    public float VidaActual =>
        vidaActual;

    public float VidaMaxima =>
        vidaBase;

    public float Danio =>
        danioActual;

    private void Awake()
    {
        agent =
            GetComponent<NavMeshAgent>();

        controlador =
            GetComponent<ControladorEnemigo>();

        enemyAttack =
            GetComponent<EnemyAttack>();

        AplicarStatsBase();
    }

    private void OnEnable()
    {
        muerto = false;

        AplicarStatsBase();
    }

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

    public void ConfigurarPorOleada(
        int oleada)
    {
        vidaActual =
            vidaBase *
            (
                1f +
                (
                    aumentoVida *
                    oleada
                )
            );

        danioActual =
            danioBase *
            (
                1f +
                (
                    aumentoDanio *
                    oleada
                )
            );

        velocidadAtaqueActual =
            velocidadAtaqueBase *
            (
                1f +
                (
                    aumentoAtaque *
                    oleada
                )
            );

        aceleracionActual =
            aceleracionBase *
            (
                1f +
                (
                    aumentoAceleracion *
                    oleada
                )
            );

        resistenciaActual =
            resistenciaBase *
            (
                1f +
                (
                    aumentoResistencia *
                    oleada
                )
            );

        velocidadMovimientoActual =
            velocidadMovimientoBase;

        AplicarMovimiento();
    }

    private void AplicarMovimiento()
    {
        if (agent != null)
        {
            agent.speed =
                velocidadMovimientoActual;

            agent.acceleration =
                aceleracionActual;
        }

        if (enemyAttack != null)
        {
            enemyAttack
                .ConfigurarDanio(
                    danioActual
                );
        }
    }

    public void RecibirDanio(
        float cantidad)
    {
        if (muerto)
            return;

        float danioFinal =
            Mathf.Max(
                cantidad -
                resistenciaActual,
                1f
            );

        vidaActual -=
            danioFinal;

        if (mostrarLogs)
        {
            Debug.Log(
                gameObject.name +
                " recibió daño"
            );
        }

        if (vidaActual <= 0f)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (muerto)
            return;

        muerto = true;

        if (controlador != null)
        {
            controlador.Morir();
        }
        else
        {
            gameObject.SetActive(
                false
            );
        }
    }

    public int ObtenerPuntos()
    {
        return puntosBase;
    }

    public float ObtenerPorcentajeVida()
    {
        return vidaActual /
               vidaBase;
    }
}