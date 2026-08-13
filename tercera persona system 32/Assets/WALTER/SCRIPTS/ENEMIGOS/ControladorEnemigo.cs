using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class ControladorEnemigo : MonoBehaviour
{
    public event Action<ControladorEnemigo> OnEnemyDeath;

    [Header("Tipo")]
    [SerializeField] private TipoEnemigo tipoEnemigo = TipoEnemigo.Alien;

    [Header("Audio")]
    [SerializeField] private AudioClip sonidoMuerte;

    [Header("Animación")]
    [SerializeField] private Animator animator;
    [SerializeField] private float duracionAnimacionMuerte = 1.5f;

    private bool muerto;
    private StatsEnemigo statsEnemigo;
    private PoolObject poolObject;

    private static readonly int AnimMuerto = Animator.StringToHash("Muerto");

    public bool Muerto => muerto;

    private void Awake()
    {
        statsEnemigo = GetComponent<StatsEnemigo>();
        poolObject = GetComponent<PoolObject>();
        if (animator == null) animator = GetComponent<Animator>();
    }

    private void OnEnable() => muerto = false;

    public void Morir()
    {
        if (muerto) return;
        muerto = true;

        if (AudioManager.Instance != null && sonidoMuerte != null)
            AudioManager.Instance.ReproducirClip3D(sonidoMuerte, transform.position);

        RegistrarMuerte();
        OnEnemyDeath?.Invoke(this);

        if (animator != null)
        {
            animator.SetTrigger(AnimMuerto);
            StartCoroutine(RutinaMuerte());
        }
        else
        {
            RegresarPool();
        }
    }

    private IEnumerator RutinaMuerte()
    {
        yield return new WaitForSeconds(duracionAnimacionMuerte);
        RegresarPool();
    }

    private void RegistrarMuerte()
    {
        if (GameManager.Instance == null) return;

        if (tipoEnemigo == TipoEnemigo.Alien)
            GameManager.Instance.RegistrarAlienEliminado();
        else
            GameManager.Instance.RegistrarNaveEliminada();

        int puntos = statsEnemigo != null ? statsEnemigo.ObtenerPuntos() : 0;
        GameManager.Instance.AgregarPuntos(puntos);
    }

    private void RegresarPool()
    {
        if (poolObject != null) poolObject.RegresarAlPool();
        else gameObject.SetActive(false);
    }
}