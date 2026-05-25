using UnityEngine;

public class EscudoPlayer : MonoBehaviour
{
    [Header("Escudo")]
    [SerializeField] private float escudoMaximo = 100f;

    [SerializeField]
    private float tiempoAntesRegenerar = 4f;

    [SerializeField]
    private float velocidadRegeneracion = 20f;

    public float EscudoActual { get; private set; }

    private float timerRegeneracion;

    private void Awake()
    {
        EscudoActual = escudoMaximo;
    }

    private void Update()
    {
        RegenerarEscudo();

        // TEST TEMPORAL
        if (Input.GetKeyDown(KeyCode.K))
        {
            GetComponent<VidaPlayer>()
                .RecibirDanio(20f);
        }
    }

    // RECIBIR DAÑO
    public float RecibirDanioEscudo(float cantidad)
    {
        // Reinicia temporizador
        timerRegeneracion = 0f;

        EscudoActual -= cantidad;

        // Si sobró daño
        if (EscudoActual < 0)
        {
            float danoRestante =
                Mathf.Abs(EscudoActual);

            EscudoActual = 0;

            return danoRestante;
        }

        return 0f;
    }

    // REGENERAR ESCUDO
    private void RegenerarEscudo()
    {
        if (EscudoActual >= escudoMaximo)
            return;

        timerRegeneracion += Time.deltaTime;

        if (timerRegeneracion >=
            tiempoAntesRegenerar)
        {
            EscudoActual +=
                velocidadRegeneracion *
                Time.deltaTime;

            EscudoActual = Mathf.Clamp(
                EscudoActual,
                0,
                escudoMaximo
            );
        }
    }

    // RECARGAR ESCUDO
    public void RecargarEscudo(float cantidad)
    {
        EscudoActual += cantidad;

        EscudoActual = Mathf.Clamp(
            EscudoActual,
            0,
            escudoMaximo
        );
    }

    // PORCENTAJE ESCUDO
    public float ObtenerPorcentajeEscudo()
    {
        return EscudoActual / escudoMaximo;
    }
}