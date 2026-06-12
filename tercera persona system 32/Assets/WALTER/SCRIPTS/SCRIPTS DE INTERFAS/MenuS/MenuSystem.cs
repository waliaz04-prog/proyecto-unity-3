using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour
{
    [Header("Escena de Juego")]
    [SerializeField] private string escenaJugar;

    [Header("Panel de Ajustes")]
    [SerializeField] private GameObject panelAjustes;
    private bool ajustesActivos = false;

    public void Jugar()
    {
        if (!string.IsNullOrEmpty(escenaJugar))
        {
            SceneManager.LoadScene(escenaJugar);
        }
        else
        {
            Debug.LogWarning("No se ha asignado la escena de juego.");
        }
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void Creditos()
    {
        SceneManager.LoadScene("Creditos");
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void Reiniciar()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //  Abre el panel de ajustes
    public void AbrirPanelAjustes()
    {
        if (panelAjustes == null)
        {
            Debug.LogWarning("No se ha asignado el Panel de Ajustes en el inspector.");
            return;
        }

        panelAjustes.SetActive(true);
        ajustesActivos = true;
    }

    //  Cierra el panel de ajustes
    public void CerrarPanelAjustes()
    {
        if (panelAjustes == null)
        {
            Debug.LogWarning("No se ha asignado el Panel de Ajustes en el inspector.");
            return;
        }

        panelAjustes.SetActive(false);
        ajustesActivos = false;
    }

    //  Alterna (muestra u oculta) el panel de ajustes
    public void TogglePanelAjustes()
    {
        if (panelAjustes == null)
        {
            Debug.LogWarning("No se ha asignado el Panel de Ajustes en el inspector.");
            return;
        }

        ajustesActivos = !ajustesActivos;
        panelAjustes.SetActive(ajustesActivos);
    }
}
