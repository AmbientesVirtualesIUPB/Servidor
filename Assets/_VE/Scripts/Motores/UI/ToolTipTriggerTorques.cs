
using UnityEngine;
using UnityEngine.EventSystems;

public class ToolTipTriggerTorques : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [TextArea]
    public string tooltipMessage;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ToolTipManagerTorques.singleton.MostrarToolTip(tooltipMessage, transform);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipManagerTorques.singleton.EsconderToolTIp();
        ToolTipManagerTorques.singleton.RestablecerAlfaCanvasGroup();
    }
}
