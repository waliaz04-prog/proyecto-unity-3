using UnityEngine;
using UnityEngine.UI;

public class ControladorVolumen : MonoBehaviour
{
    [SerializeField] private TipoAudio tipoAudio = TipoAudio.Efectos;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    private void Start()
    {
        if (slider == null) return;

        float volumenGuardado = (tipoAudio == TipoAudio.Efectos)
            ? PlayerPrefs.GetFloat("VolumenSonidos", 1f)
            : PlayerPrefs.GetFloat("VolumenMusica", 1f);

        slider.value = volumenGuardado;
        slider.onValueChanged.AddListener(AplicarCambioVolumen);
    }

    private void OnDestroy()
    {
        if (slider != null)
            slider.onValueChanged.RemoveListener(AplicarCambioVolumen);
    }

    public void AplicarCambioVolumen(float valor)
    {
        if (AudioManager.Instance == null) return;

        if (tipoAudio == TipoAudio.Efectos)
            AudioManager.Instance.AjustarVolumenEfectos(valor);
        else
            AudioManager.Instance.AjustarVolumenMusica(valor);
    }
}