using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Lista de sonidos (efectos y música)")]
    public Sonido[] Musica;

    private string currentSong;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        foreach (Sonido s in Musica)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }

    // Reproduce un sonido. Si es loop no lo reinicia si ya está sonando.
    public void Play(string nombre)
    {
        Sonido s = BuscarSonido(nombre);
        if (s == null) return;

        if (s.loop)
        {
            if (!s.source.isPlaying) s.source.Play();
        }
        else
        {
            s.source.Play();
        }

        if (!s.soundefect) currentSong = s.name;
    }

    // Reproduce un efecto corto sin interrumpir otras fuentes.
    public void PlayOneShot(string nombre)
    {
        Sonido s = BuscarSonido(nombre);
        if (s == null) return;
        s.source.PlayOneShot(s.clip);
    }

    public void Stop(string nombre)
    {
        Sonido s = BuscarSonido(nombre);
        if (s == null) return;
        s.source.Stop();
    }

    public void StopMusic()
    {
        foreach (Sonido s in Musica)
        {
            if (!s.soundefect && s.source.isPlaying)
                s.source.Stop();
        }
        currentSong = null;
    }

    public bool IsPlaying(string nombre)
    {
        Sonido s = BuscarSonido(nombre);
        return s != null && s.source.isPlaying;
    }

    private Sonido BuscarSonido(string nombre)
    {
        Sonido s = Array.Find(Musica, sonido => sonido.name == nombre);
        if (s == null) Debug.LogWarning("AudioManager: No se encontró el sonido '" + nombre + "'");
        return s;
    }
}
