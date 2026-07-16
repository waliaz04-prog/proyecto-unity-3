using TMPro;
using UnityEngine;

// Coloca este script en cada máquina expendedora del mapa.
// Requiere un Collider con isTrigger = true en el GameObject o en un hijo.
// Asigna un ItemMaquina (ScriptableObject) en el campo 'item' del Inspector.
public class MaquinaExpendedora : MonoBehaviour
{
    [Header("Ítem a vender")]
    [SerializeField] private ItemMaquina item;

    [Header("Interacción")]
    [SerializeField] private KeyCode teclaInteraccion = KeyCode.E;

    [Header("UI Prompt (panel world-space o screen-space)")]
    [SerializeField] private GameObject panelPrompt;
    [SerializeField] private TextMeshProUGUI textoAccion;
    [SerializeField] private TextMeshProUGUI textoDescripcion;

    [Header("Feedback de audio")]
    [SerializeField] private AudioSource audioFeedback;
    [SerializeField] private AudioClip sonidoCompra;
    [SerializeField] private AudioClip sonidoSinPuntos;

    [Header("Debug")]
    [SerializeField] private bool mostrarGizmo = true;

    private bool jugadorCerca;
    private PlayerUpgradeHandler upgradeHandler;

    private void Start()
    {
        OcultarPrompt();
    }

    private void Update()
    {
        if (!jugadorCerca || item == null) return;
        if (Input.GetKeyDown(teclaInteraccion))
            IntentarComprar();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        jugadorCerca = true;
        upgradeHandler = other.GetComponentInParent<PlayerUpgradeHandler>()
                      ?? other.GetComponent<PlayerUpgradeHandler>();

        MostrarPrompt();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        jugadorCerca = false;
        OcultarPrompt();
    }

    private void IntentarComprar()
    {
        if (item == null || upgradeHandler == null) return;
        if (GameManager.Instance == null || UpgradeManager.Instance == null) return;

        int nivelActual = UpgradeManager.Instance.ObtenerNivel(item);
        int precio = item.ObtenerPrecio(nivelActual);

        // Compra inválida (ej. munición de un arma que aún no se desbloqueó):
        // se rechaza antes de cobrar.
        if (!item.PuedeComprar(upgradeHandler, nivelActual))
        {
            ReproducirSonido(sonidoSinPuntos);
            return;
        }

        if (!GameManager.Instance.GastarPuntos(precio))
        {
            ReproducirSonido(sonidoSinPuntos);
            return;
        }

        item.Aplicar(upgradeHandler, nivelActual);

        if (!item.EsConsumible())
            UpgradeManager.Instance.SubirNivel(item);

        ReproducirSonido(sonidoCompra);
        ActualizarTextoPrompt();
    }

    private void MostrarPrompt()
    {
        if (panelPrompt != null) panelPrompt.SetActive(true);
        ActualizarTextoPrompt();
    }

    private void OcultarPrompt()
    {
        if (panelPrompt != null) panelPrompt.SetActive(false);
    }

    private void ActualizarTextoPrompt()
    {
        if (item == null || textoAccion == null) return;

        int nivelActual = UpgradeManager.Instance != null
            ? UpgradeManager.Instance.ObtenerNivel(item)
            : 0;

        textoAccion.text = item.ObtenerTextoAccion(nivelActual);

        if (textoDescripcion != null)
            textoDescripcion.text = item.descripcion;
    }

    private void ReproducirSonido(AudioClip clip)
    {
        if (audioFeedback != null && clip != null)
            audioFeedback.PlayOneShot(clip);
    }

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmo) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 3f);
    }
}
