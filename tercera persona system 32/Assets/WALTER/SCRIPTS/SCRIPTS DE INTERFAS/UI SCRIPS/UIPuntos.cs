using TMPro;
using UnityEngine;

public class UIPuntos : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI textoPuntos;

    [Header("Formato")]
    [SerializeField] private string prefijo = "Puntos: ";

    private void Start()
    {
        if (GameManager.Instance == null) return;
        ActualizarUI(GameManager.Instance.PuntosActuales);
        GameManager.Instance.OnPuntosCambiados += ActualizarUI;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnPuntosCambiados -= ActualizarUI;
    }

    private void ActualizarUI(int puntos)
    {
        if (textoPuntos == null) return;
        textoPuntos.text = prefijo + puntos;
    }
}
