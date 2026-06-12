using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTipoEfecto : MonoBehaviour
{
    private AudioSource audioSrc;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();

        // Aplica volumen de efectos guardado
        float volEfectos = PlayerPrefs.GetFloat("VolumenSonidos", 1f);
        audioSrc.volume *= volEfectos;
    }

    void Update()
    {
        // Actualiza dinámicamente por si el jugador cambia el volumen
        float volEfectos = PlayerPrefs.GetFloat("VolumenSonidos", 1f);
        audioSrc.volume = volEfectos;
    }
}
