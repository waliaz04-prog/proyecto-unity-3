using UnityEngine;
using UnityEngine.SceneManagement;

// Colocar en un GameObject de la escena de juego (junto al Canvas, por ejemplo).
// Los botones del panel de pausa llaman a estos métodos públicos desde su
// OnClick() en el Inspector, igual que cualquier botón del menú — funcionan
// con el EventSystem normal de Unity UI, que sigue respondiendo aunque
// Time.timeScale esté en 0 (por eso los menús de pausa siempre funcionan).
//
// El panel de Ajustes NO se maneja aquí a propósito: ya lo controla tu
// MenuSystem (TogglePanelAjustes / AbrirPanelAjustes / CerrarPanelAjustes).
// Conecta el botón "Ajustes" del panel de pausa directamente a ese MenuSystem.
public class PausaManager : MonoBehaviour
{
    [Header("Tecla")]
    [SerializeField] private KeyCode teclaPausa = KeyCode.Escape;

    [Header("Panel")]
    [SerializeField] private GameObject panelPausa;

    [Header("Escenas")]
    [SerializeField] private string escenaMenu = "MenuPrincipal";

    [Header("Jugador (opcional)")]
    [Tooltip("Si el jugador ya está muerto, ESC no abre la pausa (la escena de Game Over se encarga desde ahí).")]
    [SerializeField] private VidaPlayer vidaPlayer;

    public bool JuegoPausado { get; private set; }

    private void Start()
    {
        OcultarPanel();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(teclaPausa)) return;
        if (vidaPlayer != null && vidaPlayer.EstaMuerto) return;

        if (JuegoPausado)
            Reanudar();
        else
            Pausar();
    }

    public void Pausar()
    {
        JuegoPausado = true;
        Time.timeScale = 0f;
        MostrarCursor(true);
        if (panelPausa != null) panelPausa.SetActive(true);
    }

    public void Reanudar()
    {
        JuegoPausado = false;
        Time.timeScale = 1f;
        MostrarCursor(false);
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    // Reinicia el nivel actual desde cero (misma escena, misma oleada 1).
    public void Reiniciar()
    {
        Time.timeScale = 1f; // igual que en IrAlMenu: si no se resetea, el nivel recargado queda congelado
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void IrAlMenu()
    {
        // Importante: si no se resetea el timeScale aquí, el menú principal
        // también queda congelado al cargar (Time.timeScale es global, no
        // se resetea solo por cambiar de escena).
        Time.timeScale = 1f;
        SceneManager.LoadScene(escenaMenu);
    }

    public void SalirDelJuego()
    {
        Application.Quit();
    }

    private void OcultarPanel()
    {
        JuegoPausado = false;
        if (panelPausa != null) panelPausa.SetActive(false);
    }

    private void MostrarCursor(bool mostrar)
    {
        Cursor.lockState = mostrar ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = mostrar;
    }
}
