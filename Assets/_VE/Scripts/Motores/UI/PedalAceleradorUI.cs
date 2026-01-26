using UnityEngine.UI;
using UnityEngine;

public class PedalAceleradorUI : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public Slider sliderAcelerador;
    public RectTransform pedal;

    [Header("CONFIGURACIÓN")]
    public float rotacionMaxima = -15f;
    public float velocidadSuavizado = 8f;

    private float rotacionActual;

    void Update()
    {
        float valor = sliderAcelerador.value;

        float rotacionObjetivo = Mathf.Lerp(0f, rotacionMaxima, valor);

        rotacionActual = Mathf.Lerp(rotacionActual, rotacionObjetivo, Time.deltaTime * velocidadSuavizado);

        pedal.localRotation = Quaternion.Euler(0, 0, rotacionActual);
    }
}
