using UnityEngine;

public class EscudoPlayer : MonoBehaviour
{
    [Header("Escudo")]
    [SerializeField]
    private float escudoMaximo = 100f;

    [SerializeField]
    private float tiempoAntesRegenerar = 4f;

    [SerializeField]
    private float velocidadRegeneracion = 20f;

    public float EscudoActual
    {
        get;
        private set;
    }

    private float timerRegeneracion;

    private void Awake()
    {
        EscudoActual =
            escudoMaximo;
    }

    private void Update()
    {
        RegenerarEscudo();
    }

    public float RecibirDanioEscudo(
        float cantidad)
    {
        timerRegeneracion = 0f;

        EscudoActual -=
            cantidad;

        if (EscudoActual < 0f)
        {
            float restante =
                Mathf.Abs(
                    EscudoActual
                );

            EscudoActual = 0f;

            return restante;
        }

        return 0f;
    }

    private void RegenerarEscudo()
    {
        if (
            EscudoActual >=
            escudoMaximo
        )
        {
            return;
        }

        timerRegeneracion +=
            Time.deltaTime;

        if (
            timerRegeneracion <
            tiempoAntesRegenerar
        )
        {
            return;
        }

        EscudoActual +=
            velocidadRegeneracion *
            Time.deltaTime;

        EscudoActual =
            Mathf.Clamp(
                EscudoActual,
                0f,
                escudoMaximo
            );
    }

    public void RecargarEscudo(
        float cantidad)
    {
        EscudoActual +=
            cantidad;

        EscudoActual =
            Mathf.Clamp(
                EscudoActual,
                0f,
                escudoMaximo
            );
    }

    public float ObtenerPorcentajeEscudo()
    {
        return
            EscudoActual /
            escudoMaximo;
    }
}