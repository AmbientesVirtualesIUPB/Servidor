using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivadorBotonesPestanas : MonoBehaviour
{
    public Button btn;


    private void OnTriggerStay(Collider other)
    {
        if (btn != null && other.CompareTag("Player"))
        {
            btn.interactable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (btn != null && other.CompareTag("Player"))
        {
            btn.interactable = false;
        }
    }

    public void PonerBoton(Button _btn)
    {
        btn = _btn;
        btn.interactable = false;
    }
}
