using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotadorMaztil : MonoBehaviour
{
    public float desfase = -90;
    public float rotActual;
    Vector3 rotInicial;

    private void Start()
    {
        rotInicial = transform.localEulerAngles;
    }

    public void CambiarAngulo(float a)
    {
        rotActual = a;
    }

    // Update is called once per frame
    void Update()
    {
        rotInicial.y = transform.localEulerAngles.y;
        rotInicial.x = desfase - rotActual;
        transform.localEulerAngles = rotInicial;
    }
}
