using UnityEngine;

// Colócalo en cualquier prefab que deba activarse un momento y volver solo al pool
// (ej. el efecto que aparece debajo del jugador al comprar en una máquina).
// El prefab debe estar registrado en el PoolManager como cualquier otro objeto pooleado.
[RequireComponent(typeof(PoolObject))]
public class EfectoTemporal : MonoBehaviour
{
    [Header("Duración")]
    [Tooltip("Segundos que el efecto permanece activo antes de volver solo al pool.")]
    [SerializeField] private float duracion = 2f;

    private PoolObject poolObject;

    private void Awake()
    {
        poolObject = GetComponent<PoolObject>();
    }

    private void OnEnable()
    {
        Invoke(nameof(Desactivar), duracion);
    }

    private void OnDisable()
    {
        CancelInvoke(nameof(Desactivar));
    }

    private void Desactivar()
    {
        if (poolObject != null)
            poolObject.RegresarAlPool();
        else
            gameObject.SetActive(false);
    }
}
