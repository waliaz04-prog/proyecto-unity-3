// Colocar este script en un GameObject de la escena de juego (no en el GameManager).
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInitializer : MonoBehaviour
{
    [SerializeField] private bool resetearTimeScale = true;
    [SerializeField] private bool bloquearCursor = true;

    private void Awake()
    {
        EnemyBase.LimpiarCacheJugador();
        EnemigoNave.ResetearContadorGlobal();
        SceneManager.sceneLoaded += OnSceneCargada;
    }

    private void Start()
    {
        if (resetearTimeScale)
            Time.timeScale = 1f;

        if (bloquearCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneCargada;
    }

    private void OnSceneCargada(Scene scene, LoadSceneMode mode)
    {
        EnemyBase.LimpiarCacheJugador();
        EnemigoNave.ResetearContadorGlobal();
    }
}
