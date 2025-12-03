using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class Pulso : MonoBehaviour
{
    private Image btnImagen;
    private Vector3 escalaOriginal;
    public float aumentoEscala = 1.2f; // Factor de aumento
    public float velocidad = 1f; // Velocidad del efecto

    void Start()
    {
        btnImagen = GetComponent<Image>();

        if (btnImagen == null)
        {
            Debug.LogError("No se encontró el componente Image en el botón.");
            return;
        }

        escalaOriginal = transform.localScale;
        StartCoroutine(EfectoPulso());
    }

    IEnumerator EfectoPulso()
    {
        while (true)
        {
            // Aumentar la escala
            yield return StartCoroutine(EscalarA(escalaOriginal * aumentoEscala, velocidad));

            // Volver a la escala original
            yield return StartCoroutine(EscalarA(escalaOriginal, velocidad));
        }
    }

    IEnumerator EscalarA(Vector3 escalaObjetivo, float duracion)
    {
        Vector3 escalaInicial = transform.localScale;
        float tiempo = 0f;

        while (tiempo < duracion)
        {
            transform.localScale = Vector3.Lerp(escalaInicial, escalaObjetivo, tiempo / duracion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.localScale = escalaObjetivo;
    }

    private void OnEnable()
    {
        StartCoroutine(EfectoPulso());
    }
}
