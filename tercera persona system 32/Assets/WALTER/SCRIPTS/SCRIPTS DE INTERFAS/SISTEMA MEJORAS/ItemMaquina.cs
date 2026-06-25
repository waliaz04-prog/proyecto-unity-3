using UnityEngine;

// Clase base abstracta para todos los ítems de las máquinas expendedoras.
// Crea subclases concretas: ItemConsumible, ItemMejoraPermanente, ItemArma.
public abstract class ItemMaquina : ScriptableObject
{
    [Header("Datos generales")]
    public string nombreItem = "Ítem";
    [TextArea(2, 4)] public string descripcion;
    public Sprite icono;

    // Retorna el precio para el nivel actual. Nivel 0 = primera compra.
    public abstract int ObtenerPrecio(int nivelActual);

    // Aplica el efecto al jugador a través del handler.
    public abstract void Aplicar(PlayerUpgradeHandler handler, int nivelActual);

    // Texto del prompt de interacción.
    public abstract string ObtenerTextoAccion(int nivelActual);

    // Los consumibles no acumulan nivel — siempre cuestan lo mismo.
    public virtual bool EsConsumible() => false;
}
