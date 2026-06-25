using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camaraJugador;
    [SerializeField] private Transform arma;

    [Header("Apuntado")]
    [SerializeField] private float distanciaApuntado = 1000f;
    [SerializeField] private float velocidadRotacion = 15f;
    [SerializeField] private LayerMask capasImpacto = ~0;

    [Header("Corrección del Modelo")]
    [SerializeField] private Vector3 rotacionInicial;

    [Header("Debug")]
    [SerializeField] private bool dibujarLinea = true;

    private readonly Vector3 centroViewport = new Vector3(0.5f, 0.5f, 0f);

    private void LateUpdate()
    {
        if (camaraJugador == null || arma == null) return;

        Ray ray = camaraJugador.ViewportPointToRay(centroViewport);

        Vector3 puntoObjetivo = Physics.Raycast(ray, out RaycastHit hit, distanciaApuntado, capasImpacto)
            ? hit.point
            : ray.origin + ray.direction * distanciaApuntado;

        Vector3 direccion = puntoObjetivo - arma.position;
        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion) * Quaternion.Euler(rotacionInicial);
        arma.rotation = Quaternion.Slerp(arma.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);

        if (dibujarLinea)
            Debug.DrawLine(arma.position, puntoObjetivo, Color.red);
    }
}
