using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTriggerCanvas : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string tooltipMessage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManagerCanvas.singleton.MostrarToolTip(tooltipMessage, transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManagerCanvas.singleton.EsconderToolTIp();
        TooltipManagerCanvas.singleton.RestablecerAlfaCanvasGroup();
    }
}
