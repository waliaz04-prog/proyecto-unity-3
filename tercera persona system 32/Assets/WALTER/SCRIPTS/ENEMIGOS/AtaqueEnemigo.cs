// Fusión de DisparadorEnemigo y EnemyAttack. Usar ModoAtaque para elegir comportamiento.
using UnityEngine;

public class AtaqueEnemigo : MonoBehaviour
{
    public enum ModoAtaque
    {
        Melee,
        Distancia
    }

    [Header("Modo")]
    [SerializeField] private ModoAtaque modoAtaque = ModoAtaque.Melee;

    [Header("Daño")]
    [Tooltip("Valor de referencia. StatsEnemigo lo sobreescribe con ConfigurarDanio() al inicializar.")]
    [SerializeField] private float danio = 10f;

    [Header("Ataque")]
    [SerializeField] private float tiempoEntreAtaques = 2f;
    [SerializeField] private float distanciaAtaque = 2.5f;

    [Header("Melee (Box Collider)")]
    [Tooltip("Trigger que se activa/desactiva durante el golpe cuerpo a cuerpo. Si se deja vacío, se usa daño instantáneo por distancia (comportamiento anterior).")]
    [SerializeField] private EnemigoMeleeTrigger meleeTrigger;
    [Tooltip("Segundos que el hitbox permanece activo por cada golpe.")]
    [SerializeField] private float tiempoHitbox = 0.3f;

    [Header("Disparo (solo modo Distancia)")]
    [Tooltip("Punto desde donde sale la bala. Si se deja vacío, el ataque a distancia hace daño instantáneo (comportamiento anterior).")]
    [SerializeField] private Transform puntoDisparo;
    [Tooltip("Id del pool de la bala enemiga. Usa un prefab distinto al de las balas del jugador para diferenciarlas visualmente.")]
    [SerializeField] private string idPoolBala = "balaEnemigo";
    [Tooltip("Velocidad de la bala. Súbela para enemigos más peligrosos, bájala para dar tiempo a esquivar.")]
    [SerializeField] private float velocidadBala = 40f;
    [SerializeField] private float tiempoVidaBala = 5f;
    [SerializeField] private bool atravesarJugador;

    [Header("Objetivo")]
    [SerializeField] private Transform objetivo;

    [Header("Animación")]
    [Tooltip("Si se deja vacío, se busca automáticamente en este GameObject.")]
    [SerializeField] private Animator animator;

    [Header("Debug")]
    [SerializeField] private bool mostrarLogs = false;

    private float siguienteAtaque;
    private VidaPlayer vidaPlayerCacheada;

    private static readonly int AnimAtacar = Animator.StringToHash("Atacar");

    private void Awake()
    {
        // Ajustar distancia por defecto según modo si no fue modificada en Inspector
        if (modoAtaque == ModoAtaque.Distancia && distanciaAtaque == 2.5f)
            distanciaAtaque = 20f;

        if (animator == null)
            animator = GetComponent<Animator>();

        BuscarJugador();
    }

    private void BuscarJugador()
    {
        if (objetivo != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            objetivo = player.transform;
            vidaPlayerCacheada = player.GetComponent<VidaPlayer>();
        }
    }

    public void ConfigurarObjetivo(Transform nuevoObjetivo)
    {
        objetivo = nuevoObjetivo;
        vidaPlayerCacheada = nuevoObjetivo != null ? nuevoObjetivo.GetComponent<VidaPlayer>() : null;
    }

    public void ConfigurarDanio(float nuevoDanio)
    {
        danio = nuevoDanio;
    }

    public float ObtenerDanio() => danio;

    // Ajusta la velocidad de la bala en tiempo real (ej. escalado por oleada u otro sistema de stats).
    public void ConfigurarVelocidadBala(float nuevaVelocidad)
    {
        velocidadBala = nuevaVelocidad;
    }

    public void IntentarAtacar()
    {
        if (objetivo == null)
        {
            BuscarJugador();
            return;
        }

        if (Time.time < siguienteAtaque) return;

        float distancia = Vector3.Distance(transform.position, objetivo.position);
        if (distancia > distanciaAtaque) return;

        siguienteAtaque = Time.time + tiempoEntreAtaques;

        // Se dispara una sola vez por ataque, cubre tanto melee como distancia.
        if (animator != null)
            animator.SetTrigger(AnimAtacar);

        if (modoAtaque == ModoAtaque.Distancia && puntoDisparo != null)
        {
            Disparar();
            return;
        }

        AtacarCuerpoACuerpo();
    }

    // Ataque cuerpo a cuerpo. Si hay un EnemigoMeleeTrigger asignado, el daño lo
    // aplica el collider al tocar al jugador durante la ventana de tiempoHitbox.
    // Sin trigger asignado, mantiene el comportamiento anterior: daño instantáneo por distancia.
    private void AtacarCuerpoACuerpo()
    {
        if (meleeTrigger != null)
        {
            meleeTrigger.ActivarTrigger();
            Invoke(nameof(DesactivarMeleeTrigger), tiempoHitbox);
            if (mostrarLogs) Debug.Log(gameObject.name + " activó hitbox de ataque (" + modoAtaque + ").");
            return;
        }

        if (vidaPlayerCacheada == null)
            vidaPlayerCacheada = objetivo.GetComponent<VidaPlayer>();

        if (vidaPlayerCacheada == null) return;

        vidaPlayerCacheada.RecibirDanio(danio);
        if (mostrarLogs) Debug.Log(gameObject.name + " atacó al jugador (" + modoAtaque + ").");
    }

    private void DesactivarMeleeTrigger()
    {
        if (meleeTrigger != null)
            meleeTrigger.DesactivarTrigger();
    }

    // Dispara una bala real hacia el jugador. Usa el mismo pool/script Bala que el jugador,
    // con un idPool distinto para que el prefab (y por lo tanto la apariencia) sea diferente.
    private void Disparar()
    {
        if (PoolManager.Instance == null) return;

        Vector3 direccion = (objetivo.position - puntoDisparo.position).normalized;

        GameObject balaObj = PoolManager.Instance.ObtenerObjeto(
            idPoolBala, puntoDisparo.position, Quaternion.LookRotation(direccion));

        if (balaObj == null) return;

        if (balaObj.TryGetComponent(out Bala bala))
            bala.Configurar(danio, velocidadBala, tiempoVidaBala, atravesarJugador, disparadoPorJugador: false);

        if (mostrarLogs) Debug.Log(gameObject.name + " disparó al jugador.");
    }
}
