using UnityEngine;

public class ColocarItemsManager : MonoBehaviour
{
    public LayerMask slotLayer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, slotLayer))
            {
                EstanteriaSlot slot = hit.collider.GetComponent<EstanteriaSlot>();
                if (slot != null)
                {
                    slot.IntentarColocarItem();
                }
            }
        }
    }
}
