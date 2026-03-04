using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractuadorMisiones : MonoBehaviour
{

    public SubObjetivo subObjetivo;
    public bool requiereTrigger;
    public bool requiereTeclaYTrigger;
    [ConditionalHide("requiereTecla", true)]
    public KeyCode keyCode;
    bool activado = false;

    public void Activar()
    {
        bool o = subObjetivo.CompletarObjetivo();    
        activado = activado || o;
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        if (!activado && requiereTeclaYTrigger && Input.GetKey(keyCode))
        {
            Activar();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;


        if (!activado && !requiereTeclaYTrigger )
        {
            Activar();
        }
    }
}
