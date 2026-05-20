using System;
using UnityEngine;

public class ControladorEnemigo : MonoBehaviour
{
    public Action OnEnemyDeath;

    public void Morir()
    {
        OnEnemyDeath?.Invoke();

        Destroy(gameObject);
    }
}