using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("TIPO DE ARMA")]
    [SerializeField]
    private WeaponType tipoArma;

    [Header("DAÑO")]
    [SerializeField]
    private float danio = 20f;

    [Header("ATAQUE")]
    [SerializeField]
    private float tiempoEntreAtaques = 0.3f;

    private float siguienteAtaque;

    // MELEE

    [Header("MELEE")]
    [SerializeField]
    private WeaponMeleeTrigger meleeTrigger;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float tiempoHitbox = 0.2f;

    // FIREARM

    [Header("DISPARO")]
    [SerializeField]
    private bool armaAutomatica = false;

    [SerializeField]
    private bool usarMunicion = true;

    [SerializeField]
    private int municionActual = 30;

    [SerializeField]
    private int municionMaxima = 30;

    [SerializeField]
    private int balasPorDisparo = 1;

    [SerializeField]
    private float dispersion = 0f;

    [SerializeField]
    private bool atravesarEnemigos = false;

    // BALA
    
    [Header("PREFAB BALA")]
    [SerializeField]
    private GameObject prefabBala;

    [SerializeField]
    private Transform puntoDisparo;

    [SerializeField]
    private float velocidadBala = 60f;

    [SerializeField]
    private float tiempoVidaBala = 5f;

    [SerializeField]
    private ParticleSystem efectoDisparo;

    [Header("DEBUG")]
    [SerializeField]
    private bool mostrarLogs = false;

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

    private void DetectarInput()
    {
        if (Time.time <
            siguienteAtaque)
        {
            return;
        }

        switch (tipoArma)
        {
            case WeaponType.Melee:

                if (
                    Input.GetMouseButtonDown(0)
                )
                {
                    AtaqueMelee();
                }

                break;

            case WeaponType.Firearm:

                if (armaAutomatica)
                {
                    if (
                        Input.GetMouseButton(0)
                    )
                    {
                        Disparar();
                    }
                }
                else
                {
                    if (
                        Input.GetMouseButtonDown(0)
                    )
                    {
                        Disparar();
                    }
                }

                break;
        }
    }

    // MELEE
    
    private void AtaqueMelee()
    {
        siguienteAtaque =
            Time.time +
            tiempoEntreAtaques;

        if (animator != null)
        {
            animator.SetTrigger(
                "Atacar"
            );
        }
        else
        {
            ActivarMelee();

            Invoke(
                nameof(
                    DesactivarMelee
                ),
                tiempoHitbox
            );
        }
    }

    public void ActivarMelee()
    {
        if (meleeTrigger == null)
            return;

        meleeTrigger
            .ActivarTrigger();
    }

    public void DesactivarMelee()
    {
        if (meleeTrigger == null)
            return;

        meleeTrigger
            .DesactivarTrigger();
    }

        // DISPARO
    
    private void Disparar()
    {
        if (
            usarMunicion &&
            municionActual <= 0
        )
        {
            if (mostrarLogs)
            {
                Debug.Log(
                    "Sin munición"
                );
            }

            return;
        }

        siguienteAtaque =
            Time.time +
            tiempoEntreAtaques;

        if (usarMunicion)
        {
            municionActual--;
        }

        if (efectoDisparo != null)
        {
            efectoDisparo.Play();
        }

        for (
            int i = 0;
            i < balasPorDisparo;
            i++
        )
        {
            CrearBala();
        }
    }

    private void CrearBala()
    {
        if (
            prefabBala == null ||
            puntoDisparo == null
        )
        {
            return;
        }

        Quaternion rotacion =
            puntoDisparo.rotation;

        rotacion *= Quaternion.Euler(

            Random.Range(
                -dispersion,
                dispersion
            ),

            Random.Range(
                -dispersion,
                dispersion
            ),

            0f
        );

        GameObject nuevaBala =
            Instantiate(
                prefabBala,
                puntoDisparo.position,
                rotacion
            );

        Bala bala =
            nuevaBala
            .GetComponent<Bala>();

        if (bala != null)
        {
            bala.Configurar(

                danio,

                velocidadBala,

                tiempoVidaBala,

                atravesarEnemigos
            );
        }
    }

        // MUNICIÓN
    
    public void Recargar()
    {
        municionActual =
            municionMaxima;
    }

    // GETTERS
    
    public float ObtenerDanio()
    {
        return danio;
    }

    public WeaponType
        ObtenerTipoArma()
    {
        return tipoArma;
    }

    public int
        ObtenerMunicion()
    {
        return municionActual;
    }

    public int
        ObtenerMunicionMaxima()
    {
        return municionMaxima;
    }
}