using UnityEngine;

public class CopiarPosicionUI : MonoBehaviour
{
    public RectTransform destino;  // El objeto que copia

    RectTransform origen;   // El objeto que se sigue

    private void Awake()
    {
        origen = GetComponent<RectTransform>();
    }
    void LateUpdate()
    {
        if (!origen || !destino) return;

        // Copia exacta en espacio UI
        origen.anchoredPosition = destino.anchoredPosition;
    }
}
