using UnityEngine;
using UnityEngine.EventSystems;

public class UIDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        // Busca el Canvas más cercano en los padres
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("No se encontró un Canvas en los padres.");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Opcional: podrías bajar el alpha o desactivar raycasts aquí si necesitas
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // Se asegura de que el movimiento sea relativo al tipo de renderizado del Canvas
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Opcional: restaurar visuales, raycasts, etc.
    }
}
