using UnityEngine;

public class ZonaVueloNaves : MonoBehaviour
{
    [Header("Tamaño Zona")]
    [SerializeField] private Vector3 tamanoZona = new Vector3(200f, 50f, 200f);

    [Header("Debug")]
    [SerializeField] private bool mostrarGizmos = true;

    public Vector3 ObtenerPuntoAleatorio()
    {
        Vector3 centro = transform.position;
        float randomX = Random.Range(-tamanoZona.x * 0.5f, tamanoZona.x * 0.5f);
        float randomY = Random.Range(-tamanoZona.y * 0.5f, tamanoZona.y * 0.5f);
        float randomZ = Random.Range(-tamanoZona.z * 0.5f, tamanoZona.z * 0.5f);
        return new Vector3(centro.x + randomX, centro.y + randomY, centro.z + randomZ);
    }

    public Vector3 ObtenerTamano() => tamanoZona;

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, tamanoZona);
        Gizmos.color = new Color(0f, 1f, 1f, 0.15f);
        Gizmos.DrawCube(transform.position, tamanoZona);
    }
}
