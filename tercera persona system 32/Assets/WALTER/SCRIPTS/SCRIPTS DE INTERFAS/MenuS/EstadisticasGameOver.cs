using TMPro;
using UnityEngine;
using System.Text;

// Colocar en el panel de Game Over (junto a GameOverMenu).
// Rellena los textos con las estadísticas de la partida que guarda el GameManager.
// Asigna SOLO los textos que quieras mostrar: los campos vacíos se ignoran.
public class EstadisticasGameOver : MonoBehaviour
{
    [Header("Resumen en un solo texto (opcional)")]
    [Tooltip("Si lo asignas, aquí se escriben TODAS las estadísticas juntas, una por línea. Úsalo si prefieres un solo TextMeshPro en vez de uno por cada dato.")]
    [SerializeField] private TextMeshProUGUI textoResumenCompleto;

    [Header("Enemigos")]
    [SerializeField] private TextMeshProUGUI textoAliensEliminados;
    [SerializeField] private TextMeshProUGUI textoNavesEliminadas;
    [SerializeField] private TextMeshProUGUI textoTotalEliminados;

    [Header("Puntos")]
    [SerializeField] private TextMeshProUGUI textoPuntosGanados;
    [SerializeField] private TextMeshProUGUI textoPuntosGastados;
    [SerializeField] private TextMeshProUGUI textoPuntosRestantes;

    [Header("Tiempo")]
    [SerializeField] private TextMeshProUGUI textoTiempoSobrevivido;

    [Header("Oleadas")]
    [SerializeField] private TextMeshProUGUI textoOleadaAlcanzada;
    [SerializeField] private TextMeshProUGUI textoOleadaRecord;

    [Header("Formato")]
    [Tooltip("Marcado: escribe 'Etiqueta: valor'. Desmarcado: escribe solo el valor (útil si pones las etiquetas como textos fijos en el Canvas).")]
    [SerializeField] private bool incluirEtiquetas = true;

    private void Start()
    {
        GameManager gm = GameManager.Instance;

        // Si abres la escena GameOver directo en el editor no hay GameManager:
        // se muestran ceros para poder maquetar la UI sin errores.
        int aliens = gm != null ? gm.AliensEliminados : 0;
        int naves = gm != null ? gm.NavesEliminadas : 0;
        int total = gm != null ? gm.EnemigosTotalesEliminados : 0;
        int ganados = gm != null ? gm.PuntosGanados : 0;
        int gastados = gm != null ? gm.PuntosGastados : 0;
        int restantes = gm != null ? gm.PuntosActuales : 0;
        float tiempo = gm != null ? gm.TiempoSobrevivido : 0f;
        int oleada = gm != null ? gm.OleadaActual : 0;
        int record = gm != null ? gm.OleadaMaxima : 0;

        Escribir(textoAliensEliminados, "Aliens eliminados", aliens.ToString());
        Escribir(textoNavesEliminadas, "Naves eliminadas", naves.ToString());
        Escribir(textoTotalEliminados, "Enemigos eliminados", total.ToString());
        Escribir(textoPuntosGanados, "Puntos ganados", ganados.ToString());
        Escribir(textoPuntosGastados, "Puntos gastados", gastados.ToString());
        Escribir(textoPuntosRestantes, "Puntos restantes", restantes.ToString());
        Escribir(textoTiempoSobrevivido, "Tiempo sobrevivido", FormatearTiempo(tiempo));
        Escribir(textoOleadaAlcanzada, "Oleada alcanzada", oleada.ToString());
        Escribir(textoOleadaRecord, "Récord de oleadas", record.ToString());

        EscribirResumenCompleto(aliens, naves, total, ganados, gastados, restantes, tiempo, oleada, record);
    }

    // Arma un solo bloque de texto con todas las estadísticas, una por línea.
    private void EscribirResumenCompleto(int aliens, int naves, int total, int ganados,
        int gastados, int restantes, float tiempo, int oleada, int record)
    {
        if (textoResumenCompleto == null) return;

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Oleada alcanzada: " + oleada);
        sb.AppendLine("Récord de oleadas: " + record);
        sb.AppendLine("Tiempo sobrevivido: " + FormatearTiempo(tiempo));
        sb.AppendLine();
        sb.AppendLine("Aliens eliminados: " + aliens);
        sb.AppendLine("Naves eliminadas: " + naves);
        sb.AppendLine("Enemigos eliminados: " + total);
        sb.AppendLine();
        sb.AppendLine("Puntos ganados: " + ganados);
        sb.AppendLine("Puntos gastados: " + gastados);
        sb.Append("Puntos restantes: ").Append(restantes);

        textoResumenCompleto.text = sb.ToString();
    }

    private void Escribir(TextMeshProUGUI texto, string etiqueta, string valor)
    {
        if (texto == null) return;
        texto.text = incluirEtiquetas ? etiqueta + ": " + valor : valor;
    }

    // Convierte segundos a formato mm:ss (ej. 754s -> "12:34").
    private string FormatearTiempo(float segundos)
    {
        int minutos = Mathf.FloorToInt(segundos / 60f);
        int segs = Mathf.FloorToInt(segundos % 60f);
        return string.Format("{0:00}:{1:00}", minutos, segs);
    }
}
