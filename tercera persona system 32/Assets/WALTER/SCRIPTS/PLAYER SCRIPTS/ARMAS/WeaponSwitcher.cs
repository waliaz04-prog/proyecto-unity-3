using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [Header("Armas")]
    [SerializeField]
    private GameObject[] armas;

    [SerializeField]
    private int armaInicial = 0;

    private int armaActual;

    private void Start()
    {
        armaActual =
            armaInicial;

        ActualizarArmas();
    }

    private void Update()
    {
        if (
            Input.GetKeyDown(
                KeyCode.Alpha1
            )
        )
        {
            CambiarArma(0);
        }

        if (
            Input.GetKeyDown(
                KeyCode.Alpha2
            )
        )
        {
            CambiarArma(1);
        }

        if (
            Input.GetKeyDown(
                KeyCode.Alpha3
            )
        )
        {
            CambiarArma(2);
        }
    }

    public void CambiarArma(
        int indice)
    {
        if (indice < 0)
            return;

        if (
            indice >=
            armas.Length
        )
        {
            return;
        }

        armaActual =
            indice;

        ActualizarArmas();
    }

    private void ActualizarArmas()
    {
        for (
            int i = 0;
            i < armas.Length;
            i++
        )
        {
            if (armas[i] != null)
            {
                armas[i].SetActive(
                    i ==
                    armaActual
                );
            }
        }
    }

    public GameObject
        ObtenerArmaActual()
    {
        return
            armas[
                armaActual
            ];
    }
}