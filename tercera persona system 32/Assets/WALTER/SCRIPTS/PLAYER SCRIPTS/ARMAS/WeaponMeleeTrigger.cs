using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponMeleeTrigger : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private WeaponSystem weaponSystem;

    private Collider triggerCollider;

    private List<StatsEnemigo>
        enemigosGolpeados =
        new List<StatsEnemigo>();

    private void Awake()
    {
        triggerCollider =
            GetComponent<Collider>();

        triggerCollider.isTrigger =
            true;

        triggerCollider.enabled =
            false;
    }

    public void ActivarTrigger()
    {
        enemigosGolpeados.Clear();

        triggerCollider.enabled =
            true;
    }

    public void DesactivarTrigger()
    {
        triggerCollider.enabled =
            false;
    }

    private void OnTriggerEnter(
        Collider other)
    {
        StatsEnemigo enemigo =
            other.GetComponentInParent
            <StatsEnemigo>();

        if (enemigo == null)
            return;

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
            weaponSystem
            .ObtenerDanio()
        );
    }
}