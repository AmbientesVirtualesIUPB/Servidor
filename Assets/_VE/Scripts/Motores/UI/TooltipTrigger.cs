using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string tooltipMessage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        TooltipManager.singleton.MostrarToolTip(tooltipMessage, transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipManager.singleton.EsconderToolTIp();
        TooltipManager.singleton.RestablecerAlfaCanvasGroup();
    }
}
