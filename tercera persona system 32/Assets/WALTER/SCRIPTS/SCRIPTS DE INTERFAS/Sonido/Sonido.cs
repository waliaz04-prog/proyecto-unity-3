using UnityEngine;

[System.Serializable]
public class Sonido
{
    public string nombre;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volumen = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop = false;
    public TipoAudio tipoAudio = TipoAudio.Efectos;

    [HideInInspector]
    public AudioSource source;
}