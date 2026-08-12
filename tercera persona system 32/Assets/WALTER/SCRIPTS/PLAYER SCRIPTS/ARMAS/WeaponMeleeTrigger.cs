using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeaponMeleeTrigger : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private WeaponSystem weaponSystem;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs;

    private Collider triggerCollider;
    private readonly List<StatsEnemigo> enemigosGolpeados = new List<StatsEnemigo>();

    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        triggerCollider.isTrigger = true;
        triggerCollider.enabled = false;

        if (weaponSystem == null)
            Debug.LogWarning(gameObject.name + ": WeaponMeleeTrigger no tiene asignado WeaponSystem en el Inspector. No se aplicará daño.");
    }

    public void ActivarTrigger()
    {
        enemigosGolpeados.Clear();
        triggerCollider.enabled = true;
        if (mostrarLogs) Debug.Log(gameObject.name + ": hitbox de melee activado");
    }

    public void DesactivarTrigger()
    {
        triggerCollider.enabled = false;
        if (mostrarLogs) Debug.Log(gameObject.name + ": hitbox de melee desactivado");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (mostrarLogs) Debug.Log(gameObject.name + ": trigger tocó a " + other.name);

        if (!other.TryGetComponent(out StatsEnemigo enemigo))
            enemigo = other.GetComponentInParent<StatsEnemigo>();

        if (enemigo == null) return;
        if (enemigosGolpeados.Contains(enemigo)) return;
        if (weaponSystem == null) return;

        enemigosGolpeados.Add(enemigo);
        enemigo.RecibirDanio(weaponSystem.ObtenerDanio());
        if (mostrarLogs) Debug.Log(gameObject.name + ": daño aplicado a " + enemigo.gameObject.name);
    }
}
