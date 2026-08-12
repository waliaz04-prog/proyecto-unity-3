using UnityEngine;

public class Playerlook : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float mouseSensitivityX = 200f;
    public float mouseSensitivityY = 200f;

    [Header("Límites Verticales")]
    public float minY = -40f;
    public float maxY = 70f;

    // Renombrado de 'payaso' a 'cuerpoJugador' para mayor claridad
    private Transform cuerpoJugador;
    private float xRotation = 0f;
    private VidaPlayer vidaPlayer;

    private void Start()
    {
        cuerpoJugador = transform.parent;
        if (cuerpoJugador != null)
            vidaPlayer = cuerpoJugador.GetComponent<VidaPlayer>();
        BloquearCursor();
    }

    private void OnEnable()
    {
        BloquearCursor();
    }

    private void OnDisable()
    {
        DesbloquearCursor();
    }

    private void Update()
    {
        if (Time.timeScale == 0) return;
        if (vidaPlayer != null && vidaPlayer.EstaMuerto) return;
        RotarCamara();
    }

    private void RotarCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, minY, maxY);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        cuerpoJugador.Rotate(Vector3.up * mouseX);
    }

    private void BloquearCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void DesbloquearCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
