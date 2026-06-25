using UnityEngine;

public class AreaSpawn : MonoBehaviour
{
    [Header("Tamaño del Área")]
    [SerializeField] private Vector3 tamanoArea = new Vector3(30f, 5f, 30f);

    [Header("Debug")]
    [SerializeField] private bool mostrarGizmos = true;

    public Vector3 ObtenerPuntoAleatorio()
    {
        Vector3 puntoLocal = new Vector3(
            Random.Range(-tamanoArea.x * 0.5f, tamanoArea.x * 0.5f),
            Random.Range(-tamanoArea.y * 0.5f, tamanoArea.y * 0.5f),
            Random.Range(-tamanoArea.z * 0.5f, tamanoArea.z * 0.5f));
        return transform.TransformPoint(puntoLocal);
    }

    public Vector3 ObtenerCentro() => transform.position;

    public Vector3 ObtenerTamano() => tamanoArea;

    private void OnDrawGizmos()
    {
        if (!mostrarGizmos) return;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(Vector3.zero, tamanoArea);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, tamanoArea);
    }
}
