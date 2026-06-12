using UnityEngine;

public class AreaSpawn : MonoBehaviour
{
    [Header("Tamaño del Área")]
    [SerializeField]
    private Vector3 tamañoArea =
        new Vector3(
            30f,
            5f,
            30f
        );

    [Header("Debug")]
    [SerializeField]
    private bool mostrarGizmos = true;

    public Vector3 ObtenerPuntoAleatorio()
    {
        float randomX =
            Random.Range(
                -tamañoArea.x * 0.5f,
                tamañoArea.x * 0.5f
            );

        float randomY =
            Random.Range(
                -tamañoArea.y * 0.5f,
                tamañoArea.y * 0.5f
            );

        float randomZ =
            Random.Range(
                -tamañoArea.z * 0.5f,
                tamañoArea.z * 0.5f
            );

        Vector3 puntoLocal =
            new Vector3(
                randomX,
                randomY,
                randomZ
            );

        return transform.TransformPoint(
            puntoLocal
        );
    }

    public Vector3 ObtenerCentro()
    {
        return transform.position;
    }

    public Vector3 ObtenerTamano()
    {
        return tamañoArea;
    }

    private void OnDrawGizmos()
    {
        if (!mostrarGizmos)
            return;

        Gizmos.matrix =
            transform.localToWorldMatrix;

        Gizmos.color =
            new Color(
                0f,
                1f,
                0f,
                0.25f
            );

        Gizmos.DrawCube(
            Vector3.zero,
            tamañoArea
        );

        Gizmos.color =
            Color.green;

        Gizmos.DrawWireCube(
            Vector3.zero,
            tamañoArea
        );
    }
}