using UnityEngine;

// Ítem de máquina que vende balas para un tipo de munición concreto
// (ej. "Balas de Pistola", "Balas de Metralleta").
// Las balas van a la reserva (mochila) del arma que use ese tipo.
[CreateAssetMenu(fileName = "ItemMunicion", menuName = "Maquinas/Municion")]
public class ItemMunicion : ItemMaquina
{
    [Header("Munición")]
    [Tooltip("Tipo de balas que vende esta máquina")]
    public TipoMunicion tipoMunicion = TipoMunicion.Pistola;
    [Tooltip("Balas que se agregan a la reserva del arma con este tipo")]
    public int cantidad = 60;
    public int precio = 500;

    public override int ObtenerPrecio(int nivelActual) => precio;

    public override void Aplicar(PlayerUpgradeHandler handler, int nivelActual) =>
        handler.AgregarMunicion(tipoMunicion, cantidad);

    public override string ObtenerTextoAccion(int nivelActual) =>
        $"E  {nombreItem}  —  {precio} pts";

    // La munición es consumible: siempre cuesta lo mismo y no acumula nivel.
    public override bool EsConsumible() => true;

    // Solo se puede comprar si el jugador ya desbloqueó un arma que use estas balas.
    public override bool PuedeComprar(PlayerUpgradeHandler handler, int nivelActual) =>
        handler != null && handler.TieneArmaConMunicion(tipoMunicion);
}
