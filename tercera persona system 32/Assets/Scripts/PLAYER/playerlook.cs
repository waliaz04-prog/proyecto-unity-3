using UnityEngine;

public class Playerlook : MonoBehaviour
{
    [Header("Sensibilidad")]
    public float mouseSensitivityX = 200f;
    public float mouseSensitivityY = 200f;

    [Header("Límites Verticales")]
    public float minY = -40f;
    public float maxY = 70f;

    private Transform payaso;

    private float xRotation = 0f;

    private VidaPlayer vidaPlayer;

    private void Start()
    {
        // El Player
        payaso = transform.parent;

        // Sistema de vida
        vidaPlayer =
            payaso.GetComponent<VidaPlayer>();

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
        // Pausa
        if (Time.timeScale == 0)
            return;

        // Si el jugador murió
        if (vidaPlayer != null &&
            vidaPlayer.EstaMuerto)
            return;

        RotarCamara();
    }

    // ROTAR CÁMARA TPS
    private void RotarCamara()
    {
        float mouseX =
            Input.GetAxis("Mouse X") *
            mouseSensitivityX *
            Time.deltaTime;

        float mouseY =
            Input.GetAxis("Mouse Y") *
            mouseSensitivityY *
            Time.deltaTime;

        // Rotación vertical
        xRotation -= mouseY;

        xRotation = Mathf.Clamp(
            xRotation,
            minY,
            maxY
        );

        transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        // Rotación horizontal del jugador
        payaso.Rotate(Vector3.up * mouseX);
    }

    // BLOQUEAR CURSOR
    void BloquearCursor()
    {
        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    // DESBLOQUEAR CURSOR
    void DesbloquearCursor()
    {
        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }
}