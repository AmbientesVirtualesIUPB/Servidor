using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraInterior : MonoBehaviour
{
    public GameObject camara;
    float tiempoBloqueo;
    private void OnTriggerStay(Collider other)
    {
        if(other.CompareTag("Player") && other.gameObject.Equals(ControlUsuarios.singleton.usuarioLocal.gameObject))
        {
            camara.SetActive(true);
            tiempoBloqueo = Time.time + 2;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.Equals(ControlUsuarios.singleton.usuarioLocal.gameObject))
        {
            camara.SetActive(false);
            tiempoBloqueo = Time.time + 2;
        }
    }
}
