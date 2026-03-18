using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBotonMision : MonoBehaviour
{
    public int indice;
    public Button btm;
    public void Activar()
    {
        UIBotonera.singleton.VerMision(indice);
    }
}
