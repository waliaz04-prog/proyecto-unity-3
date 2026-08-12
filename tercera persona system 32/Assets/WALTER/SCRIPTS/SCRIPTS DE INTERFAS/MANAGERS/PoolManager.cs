using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [Header("Pools")]
    [SerializeField] private PoolData[] pools;

    [Header("Configuración")]
    [SerializeField] private bool permitirExpansionAutomatica = true;
    [SerializeField] private bool mostrarLogs = false;

    private readonly Dictionary<string, Queue<PoolObject>> diccionarioPools = new Dictionary<string, Queue<PoolObject>>();
    private readonly Dictionary<string, PoolData> datosPools = new Dictionary<string, PoolData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CrearPools();
    }

    private void CrearPools()
    {
        int count = pools.Length;
        for (int p = 0; p < count; p++)
        {
            PoolData pool = pools[p];
            Queue<PoolObject> cola = new Queue<PoolObject>();

            for (int i = 0; i < pool.cantidadInicial; i++)
            {
                PoolObject po = InstanciarObjeto(pool.prefab, pool.id);
                cola.Enqueue(po);
            }

            diccionarioPools.Add(pool.id, cola);
            datosPools.Add(pool.id, pool);

            if (mostrarLogs) Debug.Log("Pool creado: " + pool.id);
        }
    }

    private PoolObject InstanciarObjeto(GameObject prefab, string id)
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);

        if (!obj.TryGetComponent(out PoolObject poolObject))
            poolObject = obj.AddComponent<PoolObject>();

        poolObject.Configurar(id);
        return poolObject;
    }

    public GameObject ObtenerObjeto(string id, Vector3 posicion, Quaternion rotacion)
    {
        if (!diccionarioPools.TryGetValue(id, out Queue<PoolObject> cola))
            return null;

        PoolObject objeto = null;

        while (cola.Count > 0)
        {
            objeto = cola.Dequeue();
            if (objeto != null) break;
        }

        if (objeto == null)
        {
            if (!permitirExpansionAutomatica) return null;
            objeto = InstanciarObjeto(datosPools[id].prefab, id);
        }

        objeto.transform.SetPositionAndRotation(posicion, rotacion);
        objeto.gameObject.SetActive(true);
        return objeto.gameObject;
    }

    public void DevolverObjeto(PoolObject objeto)
    {
        if (objeto == null) return;

        if (!diccionarioPools.ContainsKey(objeto.PoolID))
        {
            Destroy(objeto.gameObject);
            return;
        }

        objeto.gameObject.SetActive(false);
        diccionarioPools[objeto.PoolID].Enqueue(objeto);
    }

    public int ObtenerCantidadDisponible(string id)
    {
        return diccionarioPools.TryGetValue(id, out Queue<PoolObject> cola) ? cola.Count : 0;
    }
}
