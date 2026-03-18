using UnityEngine;

public class ObjetoCreado : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("ObjetoCreador"))
        {
            ObjetoCreador objeto = other.GetComponent<ObjetoCreador>();
            objeto.noCrear = true;

            ColocarItemsManager.singleton.noCrearManager = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ObjetoCreador"))
        {
            ObjetoCreador objeto = other.GetComponent<ObjetoCreador>();
            objeto.noCrear = false;

            ColocarItemsManager.singleton.noCrearManager = false;
        }
    }
}
