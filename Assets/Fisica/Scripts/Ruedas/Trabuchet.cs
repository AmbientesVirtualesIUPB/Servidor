using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trabuchet : MonoBehaviour
{
    public VertexRadialScalerByPaint[] ruedas;
    public float radio;
    public void CambiarTamaño(float r)
    {
        radio = r;
        for (int i = 0; i < ruedas.Length; i++)
        {
            ruedas[i].scaleFactor = r;
        }
    }
}
