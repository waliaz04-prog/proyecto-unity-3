using UnityEngine;

public class WeaponSystem : MonoBehaviour
{
    [Header("Tipo")]
    [SerializeField]
    private WeaponType tipoArma;

    [Header("General")]
    [SerializeField]
    private float danio = 20f;

    [SerializeField]
    private float tiempoEntreAtaques = 0.5f;

    [Header("Melee")]
    [SerializeField]
    private WeaponMeleeTrigger meleeTrigger;

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float tiempoHitbox = 0.2f;

    [Header("Disparo")]
    [SerializeField]
    private bool armaAutomatica;

    [SerializeField]
    private Transform puntoDisparo;

    [SerializeField]
    private ParticleSystem efectoDisparo;

    [SerializeField]
    private int balasPorDisparo = 1;

    [SerializeField]
    private float dispersion;

    [SerializeField]
    private bool usarMunicion = true;

    [SerializeField]
    private int municionActual = 30;

    [SerializeField]
    private int municionMaxima = 30;

    [SerializeField]
    private float velocidadBala = 80f;

    [SerializeField]
    private float tiempoVidaBala = 5f;

    [SerializeField]
    private bool atravesarEnemigos;

    [Header("Pool")]
    [SerializeField]
    private string idPoolBala =
        "bala";

    [Header("Debug")]
    [SerializeField]
    private bool mostrarLogs;

    private float siguienteAtaque;

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
        if (
            Time.time <
            siguienteAtaque
        )
        {
            return;
        }

        switch (tipoArma)
        {
            case WeaponType.Melee:

                if (
                    Input.GetMouseButtonDown(
                        0
                    )
                )
                {
                    AtaqueMelee();
                }

                break;

            case WeaponType.Firearm:

                if (armaAutomatica)
                {
                    if (
                        Input.GetMouseButton(
                            0
                        )
                    )
                    {
                        Disparar();
                    }
                }
                else
                {
                    if (
                        Input.GetMouseButtonDown(
                            0
                        )
                    )
                    {
                        Disparar();
                    }
                }

                break;
        }
    }

    private void AtaqueMelee()
    {
        siguienteAtaque =
            Time.time +
            tiempoEntreAtaques;

        if (
            animator != null
        )
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
        if (
            meleeTrigger == null
        )
        {
            return;
        }

        meleeTrigger
            .ActivarTrigger();
    }

    public void DesactivarMelee()
    {
        if (
            meleeTrigger == null
        )
        {
            return;
        }

        meleeTrigger
            .DesactivarTrigger();
    }

    private void Disparar()
    {
        if (
            usarMunicion &&
            municionActual <= 0
        )
        {
            return;
        }

        siguienteAtaque =
            Time.time +
            tiempoEntreAtaques;

        if (usarMunicion)
        {
            municionActual--;
        }

        if (
            efectoDisparo != null
        )
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
            puntoDisparo == null
        )
        {
            return;
        }

        if (
            PoolManager.Instance ==
            null
        )
        {
            return;
        }

        Camera camara =
            Camera.main;

        if (camara == null)
        {
            return;
        }

        Ray ray =
            camara
            .ViewportPointToRay(
                new Vector3(
                    0.5f,
                    0.5f,
                    0f
                )
            );

        Vector3 objetivo;

        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                1000f
            )
        )
        {
            objetivo =
                hit.point;
        }
        else
        {
            objetivo =
                ray.origin +
                ray.direction *
                1000f;
        }

        Vector3 direccion =
            (
                objetivo -
                puntoDisparo.position
            ).normalized;

        direccion +=
            new Vector3(

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

        direccion.Normalize();

        GameObject balaObj =
            PoolManager.Instance
            .ObtenerObjeto(
                idPoolBala,
                puntoDisparo.position,
                Quaternion.LookRotation(
                    direccion
                )
            );

        if (balaObj == null)
            return;

        Bala bala =
            balaObj.GetComponent
            <Bala>();

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

    public void Recargar()
    {
        municionActual =
            municionMaxima;
    }

    public float ObtenerDanio()
    {
        return danio;
    }

    public WeaponType
        ObtenerTipoArma()
    {
        return tipoArma;
    }

    public int ObtenerMunicion()
    {
        return municionActual;
    }

    public int
        ObtenerMunicionMaxima()
    {
        return municionMaxima;
    }
}