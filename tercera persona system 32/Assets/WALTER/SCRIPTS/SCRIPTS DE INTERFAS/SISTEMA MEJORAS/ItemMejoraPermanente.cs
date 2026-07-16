using UnityEngine;

[CreateAssetMenu(fileName = "MejoraPermanente", menuName = "Maquinas/Mejora Permanente")]
public class ItemMejoraPermanente : ItemMaquina
{
    [Header("Mejora")]
    public TipoMejora tipo;
    public int precioBase = 300;
    [Range(1f, 3f), Tooltip("Multiplicador de precio por nivel. 1.5 = cada nivel cuesta 50% más")]
    public float multiplicadorPrecio = 1.5f;
    [Tooltip("Valor que se suma al stat por cada nivel comprado")]
    public float valorPorNivel = 10f;

    [Header("Arma objetivo (solo mejoras de arma)")]
    [Tooltip("Marcado: la mejora aplica solo al arma del índice de abajo. Desmarcado: aplica a todas las armas. Las mejoras de jugador (vida, escudo, velocidad) ignoran esto.")]
    public bool mejorarSoloUnArma = true;
    [Tooltip("Índice del arma en el array de WeaponSwitcher. Ej: 0 = espada, 1 = pistola, 2 = metralleta")]
    public int indiceArma = 0;

    public override int ObtenerPrecio(int nivelActual) =>
        Mathf.RoundToInt(precioBase * Mathf.Pow(multiplicadorPrecio, nivelActual));

    public override void Aplicar(PlayerUpgradeHandler handler, int nivelActual) =>
        handler.AplicarMejora(tipo, valorPorNivel, mejorarSoloUnArma ? indiceArma : -1);

    public override string ObtenerTextoAccion(int nivelActual) =>
        $"E  {nombreItem}  Lv{nivelActual + 1}  —  {ObtenerPrecio(nivelActual)} pts";

    // Las mejoras dirigidas a un arma concreta requieren tenerla desbloqueada.
    // Las mejoras de jugador o globales siempre se pueden comprar.
    public override bool PuedeComprar(PlayerUpgradeHandler handler, int nivelActual)
    {
        if (!mejorarSoloUnArma) return true;
        if (!EsMejoraDeArma()) return true;
        return handler != null && handler.ArmaDesbloqueada(indiceArma);
    }

    private bool EsMejoraDeArma() =>
        tipo == TipoMejora.CadenciaDisparo ||
        tipo == TipoMejora.CapacidadMunicion ||
        tipo == TipoMejora.VelocidadRecarga ||
        tipo == TipoMejora.Danio;
}
