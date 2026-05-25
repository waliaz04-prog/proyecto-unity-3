using System;
using UnityEngine;

public class ControladorEnemigo : MonoBehaviour
{
    public Action OnEnemyDeath;

    [Header("Tipo Enemigo")]
    [SerializeField]
    private TipoEnemigo tipoEnemigo;

    [Header("Debug")]
    [SerializeField]
    private bool destruirAlMorir = true;

    private bool muerto;

    public void Morir()
    {
        if (muerto)
            return;

        muerto = true;

        // EVENTO
        OnEnemyDeath?.Invoke();

        // REGISTRAR KILL
        RegistrarKill();

        // DESTRUIR
        if (destruirAlMorir)
        {
            Destroy(gameObject);
        }
    }

    private void RegistrarKill()
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
    }
}