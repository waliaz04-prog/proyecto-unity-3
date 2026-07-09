using UnityEngine;

[CreateAssetMenu(fileName = "ItemArma", menuName = "Maquinas/Arma")]
public class ItemArma : ItemMaquina
{
    [Header("Arma")]
    [Tooltip("Índice del arma en el array de WeaponSwitcher del jugador")]
    public int indiceArma = 1;
    public int precioDesbloqueo = 1000;
    public int precioMejoraBase = 500;
    [Range(1f, 3f)] public float multiplicadorMejora = 1.3f;
    [Tooltip("Daño que se suma al arma por cada mejora comprada")]
    public float mejoraDanoPorNivel = 10f;

    public override int ObtenerPrecio(int nivelActual)
    {
        if (nivelActual == 0) return precioDesbloqueo;
        return Mathf.RoundToInt(precioMejoraBase * Mathf.Pow(multiplicadorMejora, nivelActual - 1));
    }

    public override void Aplicar(PlayerUpgradeHandler handler, int nivelActual)
    {
        if (nivelActual == 0)
            handler.DesbloquearArma(indiceArma);
        else
            handler.MejorarDanoArma(indiceArma, mejoraDanoPorNivel);
    }

    public override string ObtenerTextoAccion(int nivelActual)
    {
        if (nivelActual == 0)
            return $"E  Comprar {nombreItem}  —  {precioDesbloqueo} pts";
        return $"E  Mejorar {nombreItem}  Lv{nivelActual}  —  {ObtenerPrecio(nivelActual)} pts";
    }
}
