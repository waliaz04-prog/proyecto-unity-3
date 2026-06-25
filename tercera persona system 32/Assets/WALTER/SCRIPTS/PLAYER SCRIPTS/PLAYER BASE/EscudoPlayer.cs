using UnityEngine;

public class EscudoPlayer : MonoBehaviour
{
    [Header("Escudo")]
    [SerializeField] private float escudoMaximo = 100f;
    [SerializeField] private float tiempoAntesRegenerar = 4f;
    [SerializeField] private float velocidadRegeneracion = 20f;

    public float EscudoActual { get; private set; }

    private float timerRegeneracion;

    private void Awake()
    {
        EscudoActual = escudoMaximo;
    }

    private void Update()
    {
        RegenerarEscudo();
    }

    // Aplica daño al escudo y devuelve el daño sobrante que debe ir a la vida.
    public float RecibirDanioEscudo(float cantidad)
    {
        timerRegeneracion = 0f;
        EscudoActual -= cantidad;

        if (EscudoActual < 0f)
        {
            float sobrante = Mathf.Abs(EscudoActual);
            EscudoActual = 0f;
            return sobrante;
        }

        return 0f;
    }

    private void RegenerarEscudo()
    {
        if (EscudoActual >= escudoMaximo) return;

        timerRegeneracion += Time.deltaTime;
        if (timerRegeneracion < tiempoAntesRegenerar) return;

        EscudoActual = Mathf.Clamp(EscudoActual + velocidadRegeneracion * Time.deltaTime, 0f, escudoMaximo);
    }

    public void RecargarEscudo(float cantidad)
    {
        EscudoActual = Mathf.Clamp(EscudoActual + cantidad, 0f, escudoMaximo);
    }

    public float EscudoMaximo => escudoMaximo;

    public void SubirEscudoMaximo(float cantidad)
    {
        escudoMaximo += cantidad;
        EscudoActual = Mathf.Clamp(EscudoActual + cantidad, 0f, escudoMaximo);
    }

    // Reduce el tiempo de espera antes de regenerar (mínimo 0.5f)
    public void MejorarRegeneracion(float reduccion)
    {
        tiempoAntesRegenerar = Mathf.Max(0.5f, tiempoAntesRegenerar - reduccion);
    }

    public float ObtenerPorcentajeEscudo() => EscudoActual / escudoMaximo;
}
