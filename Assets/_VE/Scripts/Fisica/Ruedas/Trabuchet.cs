using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trabuchet : MonoBehaviour
{
    public VertexRadialScalerByPaint[] ruedas;
    public float radio;
    public Text txtRadio;
    public void CambiarTamaño(float r)
    {
        radio = r;
        for (int i = 0; i < ruedas.Length; i++)
        {
            ruedas[i].scaleFactor = r;
        }
        txtRadio.text = r + " m";
    }
}
