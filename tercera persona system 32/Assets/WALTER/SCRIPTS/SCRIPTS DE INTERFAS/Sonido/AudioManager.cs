using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Lista de sonidos (efectos y música)")]
    public Sonido[] Musica;

    [Header("Volumen General")]
    [Tooltip("Volumen de todos los sonidos marcados como música (0 a 1). Se guarda automáticamente entre partidas.")]
    [Range(0f, 1f)][SerializeField] private float volumenMusica = 1f;
    [Tooltip("Volumen de todos los sonidos marcados como efecto (0 a 1). Se guarda automáticamente entre partidas.")]
    [Range(0f, 1f)][SerializeField] private float volumenEfectos = 1f;

    // Mismas claves que usaba antes ControladorVolumen, para no perder el
    // volumen que el jugador ya haya guardado.
    private const string ClaveVolumenMusica = "VolumenMusica";
    private const string ClaveVolumenEfectos = "VolumenSonidos";

    private string currentSong;

    public float VolumenMusica => volumenMusica;
    public float VolumenEfectos => volumenEfectos;

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

        volumenMusica = PlayerPrefs.GetFloat(ClaveVolumenMusica, volumenMusica);
        volumenEfectos = PlayerPrefs.GetFloat(ClaveVolumenEfectos, volumenEfectos);

        foreach (Sonido s in Musica)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = VolumenFinal(s);
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
        // s.source.volume ya refleja el volumen general de su categoría
        // (se actualiza en AplicarVolumenPorCategoria), así que PlayOneShot
        // lo usa automáticamente como base sin necesitar un parámetro extra.
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

    // Llamar desde el slider de Ajustes ("Volumen de Musica") en su OnValueChanged.
    public void AjustarVolumenMusica(float valor)
    {
        volumenMusica = Mathf.Clamp01(valor);
        PlayerPrefs.SetFloat(ClaveVolumenMusica, volumenMusica);
        AplicarVolumenPorCategoria(esEfecto: false);
    }

    // Llamar desde el slider de Ajustes ("Volumen de Sonidos") en su OnValueChanged.
    public void AjustarVolumenEfectos(float valor)
    {
        volumenEfectos = Mathf.Clamp01(valor);
        PlayerPrefs.SetFloat(ClaveVolumenEfectos, volumenEfectos);
        AplicarVolumenPorCategoria(esEfecto: true);
    }

    private void AplicarVolumenPorCategoria(bool esEfecto)
    {
        foreach (Sonido s in Musica)
        {
            if (s.soundefect != esEfecto) continue;
            if (s.source != null)
                s.source.volume = VolumenFinal(s);
        }
    }

    // Volumen final = volumen propio del clip (0-1, autoral) multiplicado por
    // el volumen general de su categoría (música o efectos).
    private float VolumenFinal(Sonido s) => s.volume * (s.soundefect ? volumenEfectos : volumenMusica);

    private Sonido BuscarSonido(string nombre)
    {
        Sonido s = Array.Find(Musica, sonido => sonido.name == nombre);
        if (s == null) Debug.LogWarning("AudioManager: No se encontró el sonido '" + nombre + "'");
        return s;
    }
}
