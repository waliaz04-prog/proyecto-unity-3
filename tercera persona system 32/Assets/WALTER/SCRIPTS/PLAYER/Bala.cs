using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Bala : MonoBehaviour
{
    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs = false;

    private float danio;
    private float velocidad;
    private float tiempoVida;

    private bool atravesarEnemigos;

    private float tiempoActual;

    public void Configurar(
        float nuevoDanio,
        float nuevaVelocidad,
        float nuevoTiempoVida,
        bool puedeAtravesar)
    {
        danio = nuevoDanio;

        velocidad = nuevaVelocidad;

        tiempoVida = nuevoTiempoVida;

        atravesarEnemigos =
            puedeAtravesar;

        tiempoActual = 0f;
    }

    private void Update()
    {
        transform.position +=
            transform.forward *
            velocidad *
            Time.deltaTime;

        tiempoActual +=
            Time.deltaTime;

        if (tiempoActual >=
            tiempoVida)
        {
            DestruirBala();
        }
    }

    private void OnTriggerEnter(
        Collider other)
    {
        // IGNORAR TRIGGERS
        if (other.isTrigger)
            return;

        //------------------------------------------------
        // ENEMIGOS
        //------------------------------------------------

        StatsEnemigo enemigo =
            other.GetComponentInParent
            <StatsEnemigo>();

        if (enemigo != null)
        {
            enemigo.RecibirDanio(
                danio
            );

            if (mostrarLogs)
            {
                Debug.Log(
                    "Impacto en enemigo: "
                    + enemigo.name
                );
            }

            if (!atravesarEnemigos)
            {
                DestruirBala();
            }

            return;
        }

        //------------------------------------------------
        // JUGADOR
        //------------------------------------------------

        VidaPlayer jugador =
            other.GetComponentInParent
            <VidaPlayer>();

        if (jugador != null)
        {
            jugador.RecibirDanio(
                danio
            );

            if (mostrarLogs)
            {
                Debug.Log(
                    "Impacto en jugador"
                );
            }

            DestruirBala();

            return;
        }

        //------------------------------------------------
        // CUALQUIER OTRO OBJETO
        //------------------------------------------------

        DestruirBala();
    }

    private void DestruirBala()
    {
        Destroy(gameObject);
    }
}