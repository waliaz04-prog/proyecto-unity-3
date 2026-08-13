using System.Collections;
using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    public enum ModoAtaque { Melee, Distancia }

    [Header("Modo")]
    [SerializeField] private ModoAtaque modoAtaque = ModoAtaque.Melee;

    [Header("Audio Ataque")]
    [SerializeField] private AudioClip sonidoAtaque;

    [Header("Daño")]
    [SerializeField] private float danio = 10f;
    [SerializeField] private float tiempoEntreAtaques = 2f;
    [SerializeField] private float distanciaAtaque = 2.5f;

    [Header("Melee")]
    [SerializeField] private EnemigoMeleeTrigger meleeTrigger;
    [SerializeField] private float tiempoHitbox = 0.3f;

    [Header("Disparo")]
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private string idPoolBala = "bala_enemigo";
    [SerializeField] private float velocidadBala = 40f;
    [SerializeField] private float tiempoVidaBala = 4f;

    private Transform objetivo;
    private float timerAtaque;

    private void Update()
    {
        if (objetivo == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) objetivo = p.transform;
            return;
        }

        timerAtaque += Time.deltaTime;
        float distSqr = (objetivo.position - transform.position).sqrMagnitude;

        if (distSqr <= distanciaAtaque * distanciaAtaque && timerAtaque >= tiempoEntreAtaques)
        {
            timerAtaque = 0f;
            EjecutarAtaque();
        }
    }

    private void EjecutarAtaque()
    {
        if (AudioManager.Instance != null && sonidoAtaque != null)
            AudioManager.Instance.ReproducirClip3D(sonidoAtaque, transform.position);

        if (modoAtaque == ModoAtaque.Distancia && puntoDisparo != null)
        {
            Vector3 dir = (objetivo.position - puntoDisparo.position).normalized;
            GameObject balaObj = PoolManager.Instance.ObtenerObjeto(idPoolBala, puntoDisparo.position, Quaternion.LookRotation(dir));
            if (balaObj != null && balaObj.TryGetComponent(out Bala bala))
                bala.Configurar(danio, velocidadBala, tiempoVidaBala, false, false);
            return;
        }

        if (meleeTrigger != null)
        {
            meleeTrigger.ActivarTrigger();
            StartCoroutine(RutinaHitbox());
        }
    }

    private IEnumerator RutinaHitbox()
    {
        yield return new WaitForSeconds(tiempoHitbox);
        if (meleeTrigger != null) meleeTrigger.DesactivarTrigger();
    }

    public void ConfigurarDanio(float d) => danio = d;
    public float ObtenerDanio() => danio;
}