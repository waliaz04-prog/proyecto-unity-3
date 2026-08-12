using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ControladorVolumen : MonoBehaviour
{
    [SerializeField] private TipoAudio tipoAudio = TipoAudio.Efectos;

    private AudioSource audioSrc;
    private float volumenActual = -1f;
    private string claveVolumen;

    private void Awake()
    {
        claveVolumen = tipoAudio == TipoAudio.Efectos ? "VolumenSonidos" : "VolumenMusica";
    }

    private void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        volumenActual = PlayerPrefs.GetFloat(claveVolumen, 1f);
        audioSrc.volume = volumenActual;
    }

    // Llamar desde el slider de ajustes cuando el usuario cambie el volumen.
    public void AplicarVolumen(float nuevoVolumen)
    {
        volumenActual = nuevoVolumen;
        audioSrc.volume = volumenActual;
        PlayerPrefs.SetFloat(claveVolumen, volumenActual);
    }
}
