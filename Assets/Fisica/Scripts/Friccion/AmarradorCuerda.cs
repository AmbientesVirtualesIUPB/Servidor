using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class AmarradorCuerda : MonoBehaviour
{
    LineRenderer linea;
    public Transform posicionInicial;
    public Transform posicionFinal;

    private void Awake()
    {
        linea = GetComponent<LineRenderer>();
    }

    void Update()
    {
        linea.SetPosition(0, posicionInicial.position);
        linea.SetPosition(1, posicionFinal.position);
    }
}
