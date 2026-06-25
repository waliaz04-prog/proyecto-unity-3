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

    public void RecargarMunicion()
    {
        if (sistemaArmas == null) return;
        foreach (WeaponSystem arma in sistemaArmas)
            arma?.Recargar();
    }

    public void AplicarMejora(TipoMejora tipo, float valor)
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
                if (sistemaArmas == null) break;
                foreach (WeaponSystem arma in sistemaArmas)
                    arma?.MejorarCadencia(valor);
                break;
            case TipoMejora.CapacidadMunicion:
                if (sistemaArmas == null) break;
                foreach (WeaponSystem arma in sistemaArmas)
                    arma?.SubirCapacidadMunicion(Mathf.RoundToInt(valor));
                break;
            case TipoMejora.VelocidadRecarga:
                if (sistemaArmas == null) break;
                foreach (WeaponSystem arma in sistemaArmas)
                    arma?.MejorarVelocidadRecarga(valor);
                break;
        }
    }

    public void DesbloquearArma(int indice)
    {
        weaponSwitcher?.DesbloquearArma(indice);
    }

    public void MejorarDanoArma(int indice, float cantidad)
    {
        if (sistemaArmas == null || indice < 0 || indice >= sistemaArmas.Length) return;
        sistemaArmas[indice]?.SubirDano(cantidad);
    }
}
