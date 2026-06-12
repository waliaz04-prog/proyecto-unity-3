using UnityEngine;

[System.Serializable]
public class Sonido
{
    public bool soundefect;

    public string name;

    public AudioClip clip;

    [Range (0,1)]
    public float volume;

    public bool loop;

    [HideInInspector]
    public AudioSource source;
}
