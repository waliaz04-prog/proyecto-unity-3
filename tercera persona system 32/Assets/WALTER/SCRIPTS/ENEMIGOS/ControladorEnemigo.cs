using System;
using UnityEngine;

[RequireComponent(typeof(PoolObject))]
public class ControladorEnemigo : MonoBehaviour
{
    public Action OnEnemyDeath;

    [Header("Tipo Enemigo")]
    [SerializeField]
    private TipoEnemigo tipoEnemigo =
        TipoEnemigo.Alien;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    private bool muerto;

    private StatsEnemigo statsEnemigo;
    private PoolObject poolObject;

    public bool Muerto => muerto;

    private void Awake()
    {
        statsEnemigo =
            GetComponent<StatsEnemigo>();

        poolObject =
            GetComponent<PoolObject>();
    }

    private void OnEnable()
    {
        muerto = false;
    }

    public void Morir()
    {
        if (muerto)
            return;

        muerto = true;

        RegistrarMuerte();

        OnEnemyDeath?.Invoke();

        if (mostrarLogs)
        {
            Debug.Log(
                gameObject.name +
                " eliminado."
            );
        }

        RegresarAlPool();
    }

    private void RegistrarMuerte()
    {
        if (GameManager.Instance == null)
            return;

        switch (tipoEnemigo)
        {
            case TipoEnemigo.Alien:

                GameManager.Instance
                    .RegistrarAlienEliminado();

                break;

            case TipoEnemigo.Nave:

                GameManager.Instance
                    .RegistrarNaveEliminada();

                break;
        }

        int puntosBase = 0;

        if (statsEnemigo != null)
        {
            puntosBase =
                statsEnemigo.ObtenerPuntos();
        }

        int puntosFinales =
            Mathf.RoundToInt(
                puntosBase *
                GameManager.Instance
                .ObtenerMultiplicadorPuntos()
            );

        GameManager.Instance
            .AgregarPuntos(
                puntosFinales
            );
    }

    private void RegresarAlPool()
    {
        if (poolObject != null)
        {
            poolObject.RegresarAlPool();
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    [ContextMenu("Matar Enemigo")]
    private void DebugMorir()
    {
        Morir();
    }
}