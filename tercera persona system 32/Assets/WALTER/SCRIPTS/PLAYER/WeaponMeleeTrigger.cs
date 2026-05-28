using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponMeleeTrigger : MonoBehaviour
{
    [Header("REFERENCIAS")]
    [SerializeField]
    private WeaponSystem weaponSystem;

    private Collider triggerCollider;

    // EVITA GOLPES MÚLTIPLES
    private List<StatsEnemigo>
        enemigosGolpeados =
        new List<StatsEnemigo>();

    private void Awake()
    {
        triggerCollider =
            GetComponent<Collider>();

        // IMPORTANTE
        triggerCollider.isTrigger = true;

        // DESACTIVADO AL INICIO
        triggerCollider.enabled = false;
    }

    // ACTIVAR HITBOX
    public void ActivarTrigger()
    {
        enemigosGolpeados.Clear();

        triggerCollider.enabled = true;
    }

    // DESACTIVAR HITBOX
    public void DesactivarTrigger()
    {
        triggerCollider.enabled = false;
    }

    // DETECTAR ENEMIGOS
    private void OnTriggerEnter(
        Collider other
    )
    {
        StatsEnemigo enemigo =
            other.GetComponentInParent
            <StatsEnemigo>();

        if (enemigo == null)
            return;

        // EVITA GOLPES REPETIDOS
        if (
            enemigosGolpeados
            .Contains(enemigo)
        )
        {
            return;
        }

        enemigosGolpeados
            .Add(enemigo);

        enemigo.RecibirDanio(
            weaponSystem.ObtenerDanio()
        );

        Debug.Log(
            "Golpeaste a: " +
            enemigo.name
        );
    }
}