using System;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class ControladorEnemigo : MonoBehaviour
{
    // El evento entrega el propio controlador para que el suscriptor
    // sepa exactamente qué enemigo murió (sin buscar en listas).
    public event Action<ControladorEnemigo> OnEnemyDeath;

    [Header("Tipo")]
    [SerializeField] private TipoEnemigo tipoEnemigo = TipoEnemigo.Alien;

    [Header("Animación")]
    [Tooltip("Si se deja vacío, se busca automáticamente en este GameObject.")]
    [SerializeField] private Animator animator;
    [Tooltip("Segundos que espera antes de volver al pool, para que la animación de muerte se vea completa. Ajusta esto a la duración real de tu clip de muerte.")]
    [SerializeField] private float duracionAnimacionMuerte = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = false;

    private bool muerto;
    private StatsEnemigo statsEnemigo;
    private PoolObject poolObject;

    private static readonly int AnimMuerto = Animator.StringToHash("Muerto");

    public bool Muerto => muerto;

    private void Awake()
    {
        statsEnemigo = GetComponent<StatsEnemigo>();
        poolObject = GetComponent<PoolObject>();
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        muerto = false;
    }

    private void OnDisable()
    {
        // Al volver al pool se limpian todas las suscripciones.
        // Evita que sistemas viejos (ej. una nave que lo generó) sigan
        // recibiendo eventos cuando el enemigo es reutilizado por otro sistema.
        OnEnemyDeath = null;

        // Cancela la vuelta al pool pendiente si el objeto se desactiva por
        // otro motivo (ej. cambio de escena) antes de que termine el Invoke.
        CancelInvoke(nameof(RegresarPool));
    }

    public void Morir()
    {
        if (muerto) return;
        muerto = true;
        RegistrarMuerte();
        OnEnemyDeath?.Invoke(this);
        if (mostrarLogs) Debug.Log(gameObject.name + " eliminado");

        if (animator != null)
        {
            animator.SetTrigger(AnimMuerto);
            Invoke(nameof(RegresarPool), duracionAnimacionMuerte);
        }
        else
        {
            RegresarPool();
        }
    }

    private void RegistrarMuerte()
    {
        if (GameManager.Instance == null) return;

        switch (tipoEnemigo)
        {
            case TipoEnemigo.Alien:
                GameManager.Instance.RegistrarAlienEliminado();
                break;
            case TipoEnemigo.Nave:
                GameManager.Instance.RegistrarNaveEliminada();
                break;
        }

        int puntos = statsEnemigo != null ? statsEnemigo.ObtenerPuntos() : 0;
        int puntosFinales = Mathf.RoundToInt(puntos * GameManager.Instance.ObtenerMultiplicadorPuntos());
        GameManager.Instance.AgregarPuntos(puntosFinales);
    }

    private void RegresarPool()
    {
        if (poolObject != null)
            poolObject.RegresarAlPool();
        else
            gameObject.SetActive(false);
    }

    [ContextMenu("Matar Enemigo")]
    private void DebugMorir() => Morir();
}
