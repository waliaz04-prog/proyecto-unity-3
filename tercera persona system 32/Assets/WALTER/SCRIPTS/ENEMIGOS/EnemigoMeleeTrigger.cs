using UnityEngine;

// Colócalo en un GameObject hijo del enemigo, posicionado donde deba golpear
// (ej. delante del cuerpo o en la "mano"). Requiere un BoxCollider marcado
// como trigger — el script lo fuerza a trigger en Awake de todas formas.
// AtaqueEnemigo activa y desactiva este trigger durante la ventana de golpe.
[RequireComponent(typeof(BoxCollider))]
public class EnemigoMeleeTrigger : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Si se deja vacío, se busca automáticamente en los padres de este objeto.")]
    [SerializeField] private AtaqueEnemigo ataqueEnemigo;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs;

    private BoxCollider triggerCollider;
    private bool jugadorGolpeado;

    private void Awake()
    {
        triggerCollider = GetComponent<BoxCollider>();
        triggerCollider.isTrigger = true;
        triggerCollider.enabled = false;

        if (ataqueEnemigo == null)
            ataqueEnemigo = GetComponentInParent<AtaqueEnemigo>();
    }

    public void ActivarTrigger()
    {
        jugadorGolpeado = false;
        triggerCollider.enabled = true;
        if (mostrarLogs) Debug.Log(gameObject.name + ": hitbox de ataque enemigo activado");
    }

    public void DesactivarTrigger()
    {
        triggerCollider.enabled = false;
        if (mostrarLogs) Debug.Log(gameObject.name + ": hitbox de ataque enemigo desactivado");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mostrarLogs) Debug.Log(gameObject.name + ": trigger tocó a " + other.name);

        if (jugadorGolpeado) return;

        if (!other.TryGetComponent(out VidaPlayer jugador))
            jugador = other.GetComponentInParent<VidaPlayer>();

        if (jugador == null) return;
        if (ataqueEnemigo == null) return;

        jugadorGolpeado = true;
        jugador.RecibirDanio(ataqueEnemigo.ObtenerDanio());
        if (mostrarLogs) Debug.Log(gameObject.name + ": daño aplicado al jugador");
    }
}
