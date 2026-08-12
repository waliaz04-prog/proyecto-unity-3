using UnityEngine;

// Orienta el arma según hacia dónde mira la cámara, pero SOLO en el eje vertical
// (arriba/abajo). El giro lateral (roll) queda bloqueado matemáticamente: en vez
// de apuntar hacia un punto del mundo con LookRotation (que puede "voltear" el
// arma de lado si el punto queda en un ángulo casi vertical, el clásico gimbal
// flip), se construye la rotación directamente a partir del pitch de la cámara
// y el yaw del jugador, con roll fijo en 0. Así el arma nunca puede torcerse.
public class WeaponAim : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera camaraJugador;
    [SerializeField] private Transform arma;

    [Header("Apuntado")]
    [SerializeField] private float velocidadRotacion = 15f;

    [Header("Límites de Inclinación")]
    [Tooltip("Grados máximos que el arma puede subir la mira. Debería coincidir con el 'maxY' de Playerlook para que el arma no se quede atrás ni se adelante a la cámara.")]
    [SerializeField] private float anguloMaximoArriba = 70f;
    [Tooltip("Grados máximos que el arma puede bajar la mira. Debería coincidir con el 'minY' de Playerlook.")]
    [SerializeField] private float anguloMaximoAbajo = 40f;

    [Header("Corrección del Modelo")]
    [SerializeField] private Vector3 rotacionInicial;

    private void LateUpdate()
    {
        if (camaraJugador == null || arma == null) return;

        // Pitch: ángulo vertical local de la cámara (Playerlook solo gira su eje X,
        // así que este valor ya viene limpio, sin roll).
        float pitch = camaraJugador.transform.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, -anguloMaximoArriba, anguloMaximoAbajo);

        // Yaw: hacia dónde mira el jugador en el plano horizontal (heredado del
        // cuerpo, ya que la cámara no gira en Y por sí misma).
        float yaw = camaraJugador.transform.eulerAngles.y;

        // Roll siempre en 0: es la garantía de que el arma nunca se tuerce de lado.
        Quaternion rotacionObjetivo = Quaternion.Euler(pitch, yaw, 0f) * Quaternion.Euler(rotacionInicial);
        arma.rotation = Quaternion.Slerp(arma.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
    }
}
