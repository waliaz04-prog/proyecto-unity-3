using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    [Header("Pools")]
    [SerializeField]
    private PoolData[] pools;

    [Header("Configuración")]
    [SerializeField]
    private bool permitirExpansionAutomatica = true;

    [SerializeField]
    private bool mostrarLogs = false;

    private Dictionary<string, Queue<PoolObject>>
        diccionarioPools =
            new Dictionary<string, Queue<PoolObject>>();

    private Dictionary<string, PoolData>
        datosPools =
            new Dictionary<string, PoolData>();

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CrearPools();
    }

    private void CrearPools()
    {
        foreach (PoolData pool in pools)
        {
            Queue<PoolObject> cola =
                new Queue<PoolObject>();

            for (int i = 0;
                 i < pool.cantidadInicial;
                 i++)
            {
                GameObject objeto =
                    Instantiate(
                        pool.prefab,
                        transform
                    );

                objeto.SetActive(false);

                PoolObject poolObject =
                    objeto.GetComponent<PoolObject>();

                if (poolObject == null)
                {
                    poolObject =
                        objeto.AddComponent<PoolObject>();
                }

                poolObject.Configurar(
                    pool.id
                );

                cola.Enqueue(
                    poolObject
                );
            }

            diccionarioPools.Add(
                pool.id,
                cola
            );

            datosPools.Add(
                pool.id,
                pool
            );

            if (mostrarLogs)
            {
                Debug.Log(
                    $"Pool creado: {pool.id} ({pool.cantidadInicial})"
                );
            }
        }
    }

    public GameObject ObtenerObjeto(
        string id,
        Vector3 posicion,
        Quaternion rotacion)
    {
        if (!diccionarioPools.ContainsKey(id))
        {
            Debug.LogError(
                "Pool no encontrado: " +
                id
            );

            return null;
        }

        PoolObject objeto = null;

        Queue<PoolObject> cola =
            diccionarioPools[id];

        while (cola.Count > 0)
        {
            objeto =
                cola.Dequeue();

            if (objeto != null)
            {
                break;
            }
        }

        if (objeto == null)
        {
            if (!permitirExpansionAutomatica)
            {
                return null;
            }

            PoolData pool =
                datosPools[id];

            GameObject nuevo =
                Instantiate(
                    pool.prefab,
                    transform
                );

            objeto =
                nuevo.GetComponent<PoolObject>();

            if (objeto == null)
            {
                objeto =
                    nuevo.AddComponent<PoolObject>();
            }

            objeto.Configurar(id);

            if (mostrarLogs)
            {
                Debug.Log(
                    "Pool expandido: " +
                    id
                );
            }
        }

        objeto.transform.SetPositionAndRotation(
            posicion,
            rotacion
        );

        objeto.gameObject.SetActive(true);

        return objeto.gameObject;
    }

    public void DevolverObjeto(
        PoolObject objeto)
    {
        if (objeto == null)
            return;

        if (!diccionarioPools.ContainsKey(
            objeto.PoolID))
        {
            Destroy(
                objeto.gameObject
            );

            return;
        }

        objeto.gameObject.SetActive(false);

        diccionarioPools[
            objeto.PoolID
        ].Enqueue(
            objeto
        );
    }

    public int ObtenerCantidadDisponible(
        string id)
    {
        if (!diccionarioPools.ContainsKey(id))
            return 0;

        return diccionarioPools[id].Count;
    }
}