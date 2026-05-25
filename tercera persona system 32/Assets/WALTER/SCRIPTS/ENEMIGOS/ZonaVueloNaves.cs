using UnityEngine;

public class ZonaVueloNaves : MonoBehaviour
{
    [Header("Tamaño Zona")]
    [SerializeField]
    private Vector3 tamanoZona =
        new Vector3(200f, 50f, 200f);

    public Vector3 ObtenerPuntoAleatorio()
    {
        Vector3 centro =
            transform.position;

        float randomX =
            Random.Range(
                -tamanoZona.x / 2f,
                tamanoZona.x / 2f
            );

        float randomY =
            Random.Range(
                -tamanoZona.y / 2f,
                tamanoZona.y / 2f
            );

        float randomZ =
            Random.Range(
                -tamanoZona.z / 2f,
                tamanoZona.z / 2f
            );

        return new Vector3(
            centro.x + randomX,
            centro.y + randomY,
            centro.z + randomZ
        );
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireCube(
            transform.position,
            tamanoZona
        );
    }
}