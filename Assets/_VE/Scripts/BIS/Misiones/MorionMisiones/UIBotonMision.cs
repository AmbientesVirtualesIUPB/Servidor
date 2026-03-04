using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBotonMision : MonoBehaviour
{
    public int indice;
    public void Activar()
    {
        UIBotonera.singleton.VerMision(indice);
    }
}
