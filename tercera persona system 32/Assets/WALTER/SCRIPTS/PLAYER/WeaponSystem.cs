using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("TIPO DE ARMA")]
    [SerializeField]
    private WeaponType tipoArma;

    [Header("CONFIGURACIÓN GENERAL")]
    [SerializeField]
    private float danio = 20f;

    [SerializeField]
    private float tiempoEntreAtaques = 0.5f;

    private float siguienteAtaque;

    [Header("CONFIGURACIÓN MELEE")]
    [SerializeField]
    private WeaponMeleeTrigger meleeTrigger;

    [SerializeField]
    private Animator animator;

    [Header("CONFIGURACIÓN FIREARM")]
    [SerializeField]
    private Camera camaraJugador;

    [SerializeField]
    private LayerMask capaEnemigo;

    [SerializeField]
    private float rangoDisparo = 100f;

    [SerializeField]
    private bool armaAutomatica;

    [SerializeField]
    private int municion = 30;

    [SerializeField]
    private int municionMaxima = 30;

    [SerializeField]
    private ParticleSystem efectoDisparo;

    private void Awake()
    {
        if (animator == null)
        {
            animator =
                GetComponent<Animator>();
        }
    }

    private void Update()
    {
        DetectarInput();
    }

    // DETECTAR INPUT
    private void DetectarInput()
    {
        // COOLDOWN
        if (Time.time < siguienteAtaque)
            return;

        switch (tipoArma)
        {
            // MELEE
            case WeaponType.Melee:

                if (Input.GetMouseButtonDown(0))
                {
                    AtaqueMelee();
                }

                break;

            // FIREARM
            case WeaponType.Firearm:

                // AUTOMÁTICA
                if (armaAutomatica)
                {
                    if (Input.GetMouseButton(0))
                    {
                        Disparar();
                    }
                }
                else
                {
                    // SEMIAUTOMÁTICA
                    if (Input.GetMouseButtonDown(0))
                    {
                        Disparar();
                    }
                }

                break;
        }
    }

    // ATAQUE MELEE
    private void AtaqueMelee()
    {
        siguienteAtaque =
            Time.time + tiempoEntreAtaques;

        // ANIMACIÓN
        if (animator != null)
        {
            animator.SetTrigger(
                "Atacar"
            );
        }
        else
        {
            // SI NO HAY ANIMACIÓN
            ActivarMelee();

            Invoke(
                nameof(DesactivarMelee),
                0.2f
            );
        }
    }

    // ACTIVAR HITBOX
    public void ActivarMelee()
    {
        if (meleeTrigger == null)
            return;

        meleeTrigger.ActivarTrigger();
    }

    // DESACTIVAR HITBOX
    public void DesactivarMelee()
    {
        if (meleeTrigger == null)
            return;

        meleeTrigger.DesactivarTrigger();
    }

    // DISPARAR
    private void Disparar()
    {
        // SIN MUNICIÓN
        if (municion <= 0)
        {
            Debug.Log(
                "Sin munición"
            );

            return;
        }

        municion--;

        siguienteAtaque =
            Time.time + tiempoEntreAtaques;

        // EFECTO
        if (efectoDisparo != null)
        {
            efectoDisparo.Play();
        }

        // RAYCAST
        Ray rayo =
            camaraJugador
            .ViewportPointToRay(
                new Vector3(0.5f, 0.5f)
            );

        // DETECTAR ENEMIGO
        if (Physics.Raycast(
            rayo,
            out RaycastHit impacto,
            rangoDisparo,
            capaEnemigo
        ))
        {
            StatsEnemigo enemigo =
                impacto.collider
                .GetComponentInParent
                <StatsEnemigo>();

            if (enemigo != null)
            {
                enemigo.RecibirDanio(
                    danio
                );

                Debug.Log(
                    "Disparo a: " +
                    enemigo.name
                );
            }
        }
    }

    // RECARGAR
    public void Recargar()
    {
        municion = municionMaxima;
    }

    // GETTERS
    public float ObtenerDanio()
    {
        return danio;
    }

    public WeaponType ObtenerTipoArma()
    {
        return tipoArma;
    }
}