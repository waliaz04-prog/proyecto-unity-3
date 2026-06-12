using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField]
    private Camera camaraJugador;

    [SerializeField]
    private Transform arma;

    [Header("Apuntado")]
    [SerializeField]
    private float distanciaApuntado = 1000f;

    [SerializeField]
    private float velocidadRotacion = 15f;

    [SerializeField]
    private LayerMask capasImpacto = ~0;

    [Header("Corrección del Modelo")]
    [SerializeField]
    private Vector3 rotacionInicial;

    [Header("Debug")]
    [SerializeField]
    private bool dibujarLinea = true;

    private void LateUpdate()
    {
        if (
            camaraJugador == null ||
            arma == null
        )
        {
            return;
        }

        Ray ray =
            camaraJugador
            .ViewportPointToRay(
                new Vector3(
                    0.5f,
                    0.5f,
                    0f
                )
            );

        Vector3 puntoObjetivo;

        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                distanciaApuntado,
                capasImpacto
            )
        )
        {
            puntoObjetivo =
                hit.point;
        }
        else
        {
            puntoObjetivo =
                ray.origin +
                ray.direction *
                distanciaApuntado;
        }

        Vector3 direccion =
            puntoObjetivo -
            arma.position;

        Quaternion rotacionObjetivo =
            Quaternion.LookRotation(
                direccion
            );

        rotacionObjetivo *=
            Quaternion.Euler(
                rotacionInicial
            );

        arma.rotation =
            Quaternion.Slerp(
                arma.rotation,
                rotacionObjetivo,
                velocidadRotacion *
                Time.deltaTime
            );

        if (dibujarLinea)
        {
            Debug.DrawLine(
                arma.position,
                puntoObjetivo,
                Color.red
            );
        }
    }
}