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
            SceneManager.LoadScene(escenaJugar);
        else
            Debug.LogWarning("MenuSystem: No se ha asignado la escena de juego.");
    }

    public void Salir()
    {
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

    public void AbrirPanelAjustes()
    {
        if (!ValidarPanelAjustes()) return;
        ajustesActivos = true;
        panelAjustes.SetActive(true);
    }

    public void CerrarPanelAjustes()
    {
        if (!ValidarPanelAjustes()) return;
        ajustesActivos = false;
        panelAjustes.SetActive(false);
    }

    public void TogglePanelAjustes()
    {
        if (!ValidarPanelAjustes()) return;
        ajustesActivos = !ajustesActivos;
        panelAjustes.SetActive(ajustesActivos);
    }

    private bool ValidarPanelAjustes()
    {
        if (panelAjustes != null) return true;
        Debug.LogWarning("MenuSystem: No se ha asignado el Panel de Ajustes en el inspector.");
        return false;
    }
}
