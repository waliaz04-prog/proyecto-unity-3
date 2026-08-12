using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Header("Escenas")]
    [SerializeField] private string escenaJuego = "Juego";
    [SerializeField] private string escenaMenu = "MenuPrincipal";

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }

    public void Reintentar()
    {
        SceneManager.LoadScene(escenaJuego);
    }

    public void IrAlMenu()
    {
        SceneManager.LoadScene(escenaMenu);
    }

    public void SalirJuego()
    {
        Application.Quit();
    }
}
