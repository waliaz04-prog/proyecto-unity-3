using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Lista de sonidos (efectos y música)")]
    public Sonido[] Musica;

    [Header("Volumen General")]
    [Range(0f, 1f)][SerializeField] private float volumenMusica = 1f;
    [Range(0f, 1f)][SerializeField] private float volumenEfectos = 1f;

    private const string ClaveVolumenMusica = "VolumenMusica";
    private const string ClaveVolumenEfectos = "VolumenSonidos";

    private string cancionActual;

    public float VolumenMusica => volumenMusica;
    public float VolumenEfectos => volumenEfectos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            UnityEngine.Object.Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        CargarAjustesVolumen();
        InicializarFuentes();
    }

    private void CargarAjustesVolumen()
    {
        volumenMusica = PlayerPrefs.GetFloat(ClaveVolumenMusica, 1f);
        volumenEfectos = PlayerPrefs.GetFloat(ClaveVolumenEfectos, 1f);
    }

    private void InicializarFuentes()
    {
        if (Musica == null) return;

        foreach (Sonido s in Musica)
        {
            if (s == null) continue;

            AudioSource src = gameObject.AddComponent<AudioSource>();
            src.clip = s.clip;
            src.loop = s.loop;
            src.pitch = s.pitch; // Se sincronizó con Sonido.cs
            src.playOnAwake = false;
            s.source = src;

            ActualizarVolumenFuente(s);
        }
    }

    // --- MÉTODOS DE REPRODUCCIÓN POR NOMBRE (Para Música y UI) ---

    public void ReproducirSonido(string nombre)
    {
        Sonido s = BuscarSonido(nombre);
        if (s == null || s.source == null) return;

        ActualizarVolumenFuente(s);
        if (s.tipoAudio == TipoAudio.Musica)
        {
            s.source.Play();
        }
        else
        {
            s.source.PlayOneShot(s.clip);
        }
    }

    public void ReproducirMusica(string nombre)
    {
        if (cancionActual == nombre && EstaSonando(nombre)) return;

        DetenerTodaLaMusica();

        Sonido s = BuscarSonido(nombre);
        if (s != null && s.source != null)
        {
            cancionActual = nombre;
            ActualizarVolumenFuente(s);
            s.source.Play();
        }
    }

    public void DetenerSonido(string nombre)
    {
        Sonido s = BuscarSonido(nombre);
        if (s != null && s.source != null && s.source.isPlaying)
        {
            s.source.Stop();
        }
    }

    public void DetenerTodaLaMusica()
    {
        if (Musica == null) return;

        foreach (Sonido s in Musica)
        {
            if (s != null && s.tipoAudio == TipoAudio.Musica && s.source != null)
            {
                if (s.source.isPlaying)
                {
                    s.source.Stop();
                }
            }
        }
        cancionActual = string.Empty;
    }

    public bool EstaSonando(string nombre)
    {
        Sonido s = BuscarSonido(nombre);
        return s != null && s.source != null && s.source.isPlaying;
    }

    // --- MÉTODOS DE REPRODUCCIÓN DIRECTA (Para Enemigos y Armas) ---

    public void ReproducirClip2D(AudioClip clip, float volumenLocal = 1f)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio2D");
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 0f;
        source.volume = volumenLocal * volumenEfectos;
        source.Play();

        UnityEngine.Object.Destroy(tempGO, clip.length + 0.1f);
    }

    public void ReproducirClip3D(AudioClip clip, Vector3 posicion, float volumenLocal = 1f)
    {
        if (clip == null) return;

        GameObject tempGO = new GameObject("TempAudio3D");
        tempGO.transform.position = posicion;

        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = 1f;
        source.minDistance = 2f;
        source.maxDistance = 35f;
        source.rolloffMode = AudioRolloffMode.Logarithmic;
        source.volume = volumenLocal * volumenEfectos;
        source.Play();

        UnityEngine.Object.Destroy(tempGO, clip.length + 0.1f);
    }

    // --- CONTROL DE VOLUMEN (Sliders UI) ---

    public void AjustarVolumenMusica(float valor)
    {
        volumenMusica = Mathf.Clamp01(valor);
        PlayerPrefs.SetFloat(ClaveVolumenMusica, volumenMusica);
        AplicarVolumenPorCategoria(TipoAudio.Musica);
    }

    public void AjustarVolumenEfectos(float valor)
    {
        volumenEfectos = Mathf.Clamp01(valor);
        PlayerPrefs.SetFloat(ClaveVolumenEfectos, volumenEfectos);
        AplicarVolumenPorCategoria(TipoAudio.Efectos);
    }

    private void AplicarVolumenPorCategoria(TipoAudio tipo)
    {
        if (Musica == null) return;

        foreach (Sonido s in Musica)
        {
            if (s == null || s.tipoAudio != tipo) continue;
            if (s.source != null)
                s.source.volume = VolumenFinal(s);
        }
    }

    private float VolumenFinal(Sonido s) => s.volumen * (s.tipoAudio == TipoAudio.Efectos ? volumenEfectos : volumenMusica);

    private void ActualizarVolumenFuente(Sonido s)
    {
        if (s != null && s.source != null)
            s.source.volume = VolumenFinal(s);
    }

    private Sonido BuscarSonido(string nombre)
    {
        if (Musica == null) return null;

        foreach (Sonido s in Musica)
        {
            if (s != null && s.nombre == nombre)
                return s;
        }
        return null;
    }
}