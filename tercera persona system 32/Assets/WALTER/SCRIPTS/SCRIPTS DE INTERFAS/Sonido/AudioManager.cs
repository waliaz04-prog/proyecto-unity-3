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
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }

        // Configurar cada sonido
        foreach (Sonido s in Musica)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
        }
    }

    /// <summary>
    /// Reproduce un sonido (no lo repite si ya está sonando y es loop)
    /// </summary>
    public void Play(string nombre)
    {
        Sonido s = Array.Find(Musica, sonido => sonido.name == nombre);
        if (s == null)
        {
            Debug.LogWarning($"AudioManager: No se encontró el sonido '{nombre}'");
            return;
        }

        if (s.loop)
        {
            if (!s.source.isPlaying)
                s.source.Play();
        }
        else
        {
            s.source.Play();
        }

        if (!s.soundefect)
            currentSong = s.name;
    }

    /// <summary>
    /// Reproduce un sonido como efecto corto, incluso si ya está en uso (no interrumpe loops)
    /// </summary>
    public void PlayOneShot(string nombre)
    {
        Sonido s = Array.Find(Musica, sonido => sonido.name == nombre);
        if (s == null)
        {
            Debug.LogWarning($"AudioManager: No se encontró el sonido '{nombre}'");
            return;
        }

        s.source.PlayOneShot(s.clip);
    }

    /// <summary>
    /// Detiene un sonido por nombre
    /// </summary>
    public void Stop(string nombre)
    {
        Sonido s = Array.Find(Musica, sonido => sonido.name == nombre);
        if (s == null)
        {
            Debug.LogWarning($"AudioManager: No se encontró el sonido '{nombre}' para detenerlo");
            return;
        }

        s.source.Stop();
    }

    /// <summary>
    /// Detiene toda la música (no efectos)
    /// </summary>
    public void StopMusic()
    {
        foreach (Sonido s in Musica)
        {
            if (!s.soundefect && s.source.isPlaying)
                s.source.Stop();
        }
        currentSong = null;
    }

    /// <summary>
    /// Verifica si un sonido está sonando
    /// </summary>
    public bool IsPlaying(string nombre)
    {
        Sonido s = Array.Find(Musica, sonido => sonido.name == nombre);
        if (s != null)
            return s.source.isPlaying;
        return false;
    }
}
