using UnityEngine;

// Coloca este script en el GameObject raíz del jugador.
// Centraliza todos los efectos que los ítems de las máquinas pueden aplicar al jugador.
[DisallowMultipleComponent]
public class PlayerUpgradeHandler : MonoBehaviour
{
    [Header("Componentes del jugador")]
    [SerializeField] private VidaPlayer vidaPlayer;
    [SerializeField] private EscudoPlayer escudoPlayer;
    [SerializeField] private PlayerMovimiento movimiento;
    [SerializeField] private WeaponSwitcher weaponSwitcher;

    [Header("Sistemas de armas (en orden del array de WeaponSwitcher)")]
    [SerializeField] private WeaponSystem[] sistemaArmas;

    private void Awake()
    {
        if (vidaPlayer == null) vidaPlayer = GetComponent<VidaPlayer>();
        if (escudoPlayer == null) escudoPlayer = GetComponent<EscudoPlayer>();
        if (movimiento == null) movimiento = GetComponent<PlayerMovimiento>();
        if (weaponSwitcher == null) weaponSwitcher = GetComponentInChildren<WeaponSwitcher>(true);
        if (sistemaArmas == null || sistemaArmas.Length == 0)
            sistemaArmas = GetComponentsInChildren<WeaponSystem>(true);
    }

    public void CurarVida(float cantidad)
    {
        vidaPlayer?.CurarVida(cantidad);
    }

    // Suma balas a la reserva de las armas que usen ese tipo de munición.
    // Lo llaman los ItemMunicion de las máquinas expendedoras.
    public void AgregarMunicion(TipoMunicion tipo, int cantidad)
    {
        if (sistemaArmas == null) return;
        foreach (WeaponSystem arma in sistemaArmas)
        {
            if (arma == null || !arma.UsaMunicion) continue;
            if (arma.ObtenerTipoMunicion() != tipo) continue;
            arma.AgregarMunicionReserva(cantidad);
        }
    }

    public void AplicarMejora(TipoMejora tipo, float valor)
    {
        AplicarMejora(tipo, valor, -1);
    }

    // indiceArma: -1 = todas las armas; 0, 1, 2... = solo esa arma
    // (según el orden del array de WeaponSwitcher).
    // Las mejoras de jugador (vida, escudo, velocidad) ignoran el índice.
    public void AplicarMejora(TipoMejora tipo, float valor, int indiceArma)
    {
        switch (tipo)
        {
            case TipoMejora.VidaMaxima:
                vidaPlayer?.SubirVidaMaxima(valor);
                break;
            case TipoMejora.EscudoMaximo:
                escudoPlayer?.SubirEscudoMaximo(valor);
                break;
            case TipoMejora.VelocidadRegenEscudo:
                escudoPlayer?.MejorarRegeneracion(valor);
                break;
            case TipoMejora.VelocidadMovimiento:
                movimiento?.SubirVelocidad(valor);
                break;
            case TipoMejora.CadenciaDisparo:
            case TipoMejora.CapacidadMunicion:
            case TipoMejora.VelocidadRecarga:
            case TipoMejora.Danio:
                AplicarMejoraArmas(tipo, valor, indiceArma);
                break;
        }
    }

    private void AplicarMejoraArmas(TipoMejora tipo, float valor, int indiceArma)
    {
        if (sistemaArmas == null) return;

        if (indiceArma >= sistemaArmas.Length)
        {
            Debug.LogWarning("PlayerUpgradeHandler: índice de arma " + indiceArma + " fuera de rango.");
            return;
        }

        for (int i = 0; i < sistemaArmas.Length; i++)
        {
            // Con índice válido solo se mejora esa arma; con -1 se mejoran todas.
            if (indiceArma >= 0 && i != indiceArma) continue;

            WeaponSystem arma = sistemaArmas[i];
            if (arma == null) continue;

            switch (tipo)
            {
                case TipoMejora.CadenciaDisparo:
                    arma.MejorarCadencia(valor);
                    break;
                case TipoMejora.CapacidadMunicion:
                    arma.SubirCapacidadMunicion(Mathf.RoundToInt(valor));
                    break;
                case TipoMejora.VelocidadRecarga:
                    arma.MejorarVelocidadRecarga(valor);
                    break;
                case TipoMejora.Danio:
                    arma.SubirDano(valor);
                    break;
            }
        }
    }

    public void DesbloquearArma(int indice)
    {
        weaponSwitcher?.DesbloquearArma(indice);
    }

    // True si el arma de ese índice ya está desbloqueada.
    public bool ArmaDesbloqueada(int indice)
    {
        if (weaponSwitcher == null) return true;
        return !weaponSwitcher.EstasBloqueada(indice);
    }

    // True si el jugador tiene desbloqueada al menos un arma que use este tipo de munición.
    // Se usa para impedir comprar balas de armas que aún no se compraron.
    public bool TieneArmaConMunicion(TipoMunicion tipo)
    {
        if (sistemaArmas == null) return false;
        for (int i = 0; i < sistemaArmas.Length; i++)
        {
            WeaponSystem arma = sistemaArmas[i];
            if (arma == null || !arma.UsaMunicion) continue;
            if (arma.ObtenerTipoMunicion() != tipo) continue;
            if (ArmaDesbloqueada(i)) return true;
        }
        return false;
    }

    public void MejorarDanoArma(int indice, float cantidad)
    {
        if (sistemaArmas == null || indice < 0 || indice >= sistemaArmas.Length) return;
        sistemaArmas[indice]?.SubirDano(cantidad);
    }
}
