using UnityEngine;

public class MusicaEscena : MonoBehaviour
{
    [Header("Configuración de Música de Escena")]
    [Tooltip("Debe coincidir exactamente con el campo 'name' del sonido registrado en la lista del AudioManager.")]
    [SerializeField] private string nombrePista = "MusicaFondo";

    [Tooltip("Si se marca true, la música de esta escena se detendrá automáticamente al salir de ella.")]
    [SerializeField] private bool detenerAlSalir = true;

    private void Start()
    {
        if (AudioManager.Instance != null && !string.IsNullOrEmpty(nombrePista))
        {
            AudioManager.Instance.ReproducirMusica(nombrePista);
        }
    }

    private void OnDestroy()
    {
        if (detenerAlSalir && AudioManager.Instance != null)
        {
            AudioManager.Instance.DetenerSonido(nombrePista);
        }
    }
}