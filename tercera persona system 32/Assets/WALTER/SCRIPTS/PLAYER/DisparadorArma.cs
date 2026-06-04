using UnityEngine;

public class DisparadorArma : MonoBehaviour
{
    [Header("Daño")]
    [SerializeField]
    private float danio = 10f;

    [Header("Disparo")]
    [SerializeField]
    private int balasPorDisparo = 1;

    [SerializeField]
    private float tiempoEntreDisparos = 0.2f;

    [SerializeField]
    private float alcance = 100f;

    [SerializeField]
    private float dispersion = 0f;

    [Header("Munición")]
    [SerializeField]
    private bool usarMunicion = true;

    [SerializeField]
    private int municionActual = 30;

    [SerializeField]
    private int municionMaxima = 30;

    [Header("Referencias")]
    [SerializeField]
    private Camera camaraJugador;

    [SerializeField]
    private LayerMask capasImpacto;

    [SerializeField]
    private ParticleSystem efectoDisparo;

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    private float siguienteDisparo;

    public bool PuedeDisparar()
    {
        return Time.time >= siguienteDisparo;
    }

    public void Disparar()
    {
        if (!PuedeDisparar())
            return;

        if (usarMunicion &&
            municionActual <= 0)
        {
            return;
        }

        siguienteDisparo =
            Time.time +
            tiempoEntreDisparos;

        if (usarMunicion)
        {
            municionActual--;
        }

        if (efectoDisparo != null)
        {
            efectoDisparo.Play();
        }

        for (int i = 0;
             i < balasPorDisparo;
             i++)
        {
            LanzarRayo();
        }
    }

    private void LanzarRayo()
    {
        if (camaraJugador == null)
            return;

        Vector3 direccion =
            camaraJugador.transform.forward;

        direccion += new Vector3(
            Random.Range(
                -dispersion,
                dispersion
            ),
            Random.Range(
                -dispersion,
                dispersion
            ),
            Random.Range(
                -dispersion,
                dispersion
            )
        );

        Ray ray =
            new Ray(
                camaraJugador.transform.position,
                direccion.normalized
            );

        if (Physics.Raycast(
            ray,
            out RaycastHit hit,
            alcance,
            capasImpacto))
        {
            StatsEnemigo enemigo =
                hit.collider
                .GetComponentInParent
                <StatsEnemigo>();

            if (enemigo != null)
            {
                enemigo.RecibirDanio(
                    danio
                );
            }

            if (mostrarLogs)
            {
                Debug.Log(
                    "Impacto: " +
                    hit.collider.name
                );
            }
        }
    }

    public void Recargar()
    {
        municionActual =
            municionMaxima;
    }

    public int MunicionActual =>
        municionActual;

    public int MunicionMaxima =>
        municionMaxima;
}