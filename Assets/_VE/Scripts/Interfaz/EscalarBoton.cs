using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EscalarBoton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Escalas Personalizables")]
    public Vector3 escalaNormal = Vector3.one;  // Escala inicial
    public Vector3 escalaAumentada = new Vector3(1.1f, 1.1f, 1.1f); // Tamaño al pasar el mouse
    public float velocidadTransicion = 0.1f;

    private Coroutine escalaCoroutine; // Para saber si hay currutinas activas

    private void Start()
    {
        escalaNormal = transform.localScale;// Asegura que empiece con la escala correcta
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Si hay una corrutina en ejecución, detenerla
        if (escalaCoroutine != null) StopCoroutine(escalaCoroutine);

        // Iniciar la animación de escalado
        escalaCoroutine = StartCoroutine(Escalar(escalaAumentada));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Si hay una corrutina en ejecución, detenerla
        if (escalaCoroutine != null) StopCoroutine(escalaCoroutine);

        // Volver a la escala original suavemente
        escalaCoroutine = StartCoroutine(Escalar(escalaNormal));
    }

    private IEnumerator Escalar(Vector3 objetivo)
    {
        Vector3 escalaInicial = transform.localScale;
        float tiempo = 0;

        while (tiempo < velocidadTransicion)
        {
            transform.localScale = Vector3.Lerp(escalaInicial, objetivo, tiempo / velocidadTransicion);
            tiempo += Time.deltaTime;
            yield return null;
        }

        transform.localScale = objetivo; // Asegurar la escala final
    }
}
