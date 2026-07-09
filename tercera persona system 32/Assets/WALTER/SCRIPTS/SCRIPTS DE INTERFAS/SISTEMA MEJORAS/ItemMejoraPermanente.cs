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

    public override int ObtenerPrecio(int nivelActual) =>
        Mathf.RoundToInt(precioBase * Mathf.Pow(multiplicadorPrecio, nivelActual));

    public override void Aplicar(PlayerUpgradeHandler handler, int nivelActual) =>
        handler.AplicarMejora(tipo, valorPorNivel);

    public override string ObtenerTextoAccion(int nivelActual) =>
        $"E  {nombreItem}  Lv{nivelActual + 1}  —  {ObtenerPrecio(nivelActual)} pts";
}
