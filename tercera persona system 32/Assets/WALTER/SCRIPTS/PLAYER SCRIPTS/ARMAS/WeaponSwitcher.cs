using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Armas")]
    [SerializeField] private GameObject[] armas;
    [SerializeField] private int armaInicial = 0;

    [Header("Bloqueo")]
    [Tooltip("Marca true las armas que empiezan bloqueadas (deben comprarse en máquinas)")]
    [SerializeField] private bool[] iniciarBloqueada;

    private int armaActual;
    private bool[] bloqueadas;

    private void Start()
    {
        InicializarBloqueos();
        armaActual = armaInicial;
        // Si el arma inicial está bloqueada, buscar la primera disponible
        if (EstasBloqueada(armaActual))
            armaActual = PrimerArmaDisponible();
        ActualizarArmas();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) CambiarArma(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) CambiarArma(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) CambiarArma(2);
    }

    private void InicializarBloqueos()
    {
        bloqueadas = new bool[armas.Length];
        for (int i = 0; i < armas.Length; i++)
        {
            bloqueadas[i] = (iniciarBloqueada != null && i < iniciarBloqueada.Length)
                ? iniciarBloqueada[i]
                : false;
        }
    }

    public void CambiarArma(int indice)
    {
        if (indice < 0 || indice >= armas.Length) return;
        if (EstasBloqueada(indice)) return;
        armaActual = indice;
        ActualizarArmas();
    }

    public void DesbloquearArma(int indice)
    {
        if (indice < 0 || indice >= armas.Length) return;
        bloqueadas[indice] = false;
        // Cambiar automáticamente al arma recién desbloqueada
        armaActual = indice;
        ActualizarArmas();
    }

    public bool EstasBloqueada(int indice)
    {
        if (indice < 0 || indice >= armas.Length) return true;
        return bloqueadas != null && bloqueadas[indice];
    }

    private int PrimerArmaDisponible()
    {
        for (int i = 0; i < armas.Length; i++)
            if (!EstasBloqueada(i)) return i;
        return 0;
    }

    private void ActualizarArmas()
    {
        int count = armas.Length;
        for (int i = 0; i < count; i++)
        {
            if (armas[i] != null)
                armas[i].SetActive(i == armaActual && !EstasBloqueada(i));
        }
    }

    public GameObject ObtenerArmaActual() => armas[armaActual];
    public int ObtenerIndiceActual() => armaActual;
}
