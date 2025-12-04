using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonaPuerta : MonoBehaviour
{
    public AnimatedDoorSystem puertas;

    private void OnTriggerStay(Collider other)
    {
        if (puertas != null && other.CompareTag("Player"))
        {
            puertas.OpenDoor();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (puertas != null && other.CompareTag("Player"))
        {
            puertas.CloseDoor();
        }
    }
}
