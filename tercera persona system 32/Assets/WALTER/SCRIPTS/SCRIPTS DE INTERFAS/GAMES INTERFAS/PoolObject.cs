using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public string PoolID
    {
        get;
        private set;
    }

    public void Configurar(
        string id)
    {
        PoolID = id;
    }

    public void RegresarAlPool()
    {
        if (
            PoolManager.Instance
            == null
        )
        {
            gameObject
                .SetActive(false);

            return;
        }

        PoolManager.Instance
            .DevolverObjeto(
                this
            );
    }
}