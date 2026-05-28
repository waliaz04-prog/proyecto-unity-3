using UnityEngine;

public class AreaSpawn : MonoBehaviour
{
    [Header("Tamaño Área")]
    [SerializeField]
    private Vector3 tamañoArea =
        new Vector3(20f, 5f, 20f);

    [Header("Debug")]
    [SerializeField]
    private bool mostrarGizmos = true;

    public Vector3 ObtenerPuntoAleatorio()
    {
        Vector3 centro =
            transform.position;

        Vector3 puntoAleatorio =
            new Vector3(
                Random.Range(
                    centro.x - tamañoArea.x / 2f,
                    centro.x + tamañoArea.x / 2f
                ),

                centro.y,

                Random.Range(
                    centro.z - tamañoArea.z / 2f,
                    centro.z + tamañoArea.z / 2f
                )
            );

        return puntoAleatorio;
    }

    private void OnDrawGizmos()
    {
        if (!mostrarGizmos)
            return;

        Gizmos.color =
            new Color(0f, 1f, 0f, 0.3f);

        Gizmos.DrawCube(
            transform.position,
            tamañoArea
        );

        Gizmos.color =
            Color.green;

        Gizmos.DrawWireCube(
            transform.position,
            tamañoArea
        );
    }
}