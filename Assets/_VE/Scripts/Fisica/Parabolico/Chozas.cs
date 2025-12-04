using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ControlQuemado))]
[RequireComponent(typeof(Rigidbody))]
public class Chozas : MonoBehaviour
{
    public ControlQuemado controlQuemado;
    public GameObject particulasExplosion;
    private void Awake()
    {
        controlQuemado = GetComponent<ControlQuemado>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Flecha"))
        {
            controlQuemado.EjecutarAnimacion();
            if (particulasExplosion != null) particulasExplosion.SetActive(true);
        }
    }
}
