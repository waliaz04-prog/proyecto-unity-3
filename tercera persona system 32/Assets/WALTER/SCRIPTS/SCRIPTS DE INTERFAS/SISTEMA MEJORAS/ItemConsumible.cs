using UnityEngine;

[CreateAssetMenu(fileName = "Consumible", menuName = "Maquinas/Consumible")]
public class ItemConsumible : ItemMaquina
{
    [Header("Consumible")]
    public TipoConsumible tipo;
    public int precio = 200;
    [Tooltip("Cantidad de vida curada o munición recargada")]
    public float valor = 50f;

    public override int ObtenerPrecio(int nivelActual) => precio;

    public override void Aplicar(PlayerUpgradeHandler handler, int nivelActual)
    {
        switch (tipo)
        {
            case TipoConsumible.CuracionInstantanea:
                handler.CurarVida(valor);
                break;
            case TipoConsumible.PaqueteMunicion:
                handler.RecargarMunicion();
                break;
        }
    }

    public override string ObtenerTextoAccion(int nivelActual) =>
        $"E  {nombreItem}  —  {precio} pts";

    public override bool EsConsumible() => true;
}
