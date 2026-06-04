using UnityEditor.EditorTools;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public string PoolID { get; private set; }

    public void Configurar(string id)
    {
        PoolID = id;
    }

    public void RegresarAlPool()
    {
        PoolManager.Instance.DevolverObjeto(this);
    }
}