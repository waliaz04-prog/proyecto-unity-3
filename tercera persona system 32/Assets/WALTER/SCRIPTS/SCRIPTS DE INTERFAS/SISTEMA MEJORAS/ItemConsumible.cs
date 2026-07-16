using UnityEngine;

[CreateAssetMenu(fileName = "Consumible", menuName = "Maquinas/Consumible")]
public class ItemConsumible : ItemMaquina
{
    [Header("Consumible")]
    public TipoConsumible tipo;
    public int precio = 200;
    [Tooltip("Cantidad de vida curada")]
    public float valor = 50f;

    public override int ObtenerPrecio(int nivelActual) => precio;

    // Para vender munición usar ItemMunicion (menú Maquinas/Municion).
    public override void Aplicar(PlayerUpgradeHandler handler, int nivelActual)
    {
        switch (tipo)
        {
            case TipoConsumible.CuracionInstantanea:
                handler.CurarVida(valor);
                break;
        }
    }

    public override string ObtenerTextoAccion(int nivelActual) =>
        $"E  {nombreItem}  —  {precio} pts";

    public override bool EsConsumible() => true;
}
