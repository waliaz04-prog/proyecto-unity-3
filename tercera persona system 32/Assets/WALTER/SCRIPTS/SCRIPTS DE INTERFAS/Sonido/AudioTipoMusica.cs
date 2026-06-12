using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioTipoMusica : MonoBehaviour
{
    private AudioSource audioSrc;

    void Start()
    {
        audioSrc = GetComponent<AudioSource>();
        float volMusica = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        audioSrc.volume *= volMusica;
    }

    void Update()
    {
        float volMusica = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        audioSrc.volume = volMusica;
    }
}