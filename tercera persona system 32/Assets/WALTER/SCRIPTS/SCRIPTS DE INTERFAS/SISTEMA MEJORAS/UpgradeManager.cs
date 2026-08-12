using System.Collections.Generic;
using UnityEngine;

// Singleton que rastrea el nivel de cada ítem comprado durante la partida.
// Colocar en la escena del juego (no DontDestroyOnLoad — se resetea al recargar escena).
public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance;

    private readonly Dictionary<string, int> niveles = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Retorna el nivel actual de un ítem (0 = nunca comprado)
    public int ObtenerNivel(ItemMaquina item)
    {
        if (item == null) return 0;
        return niveles.TryGetValue(item.name, out int nivel) ? nivel : 0;
    }

    // Sube el nivel del ítem en 1
    public void SubirNivel(ItemMaquina item)
    {
        if (item == null) return;
        if (!niveles.ContainsKey(item.name))
            niveles[item.name] = 0;
        niveles[item.name]++;
    }

    public bool EstaDesbloqueado(ItemMaquina item) => ObtenerNivel(item) > 0;

    // Llamar si se quiere reiniciar todas las mejoras (ej. nueva partida)
    public void Resetear() => niveles.Clear();
}
