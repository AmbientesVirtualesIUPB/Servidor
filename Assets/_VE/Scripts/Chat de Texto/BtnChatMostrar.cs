using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BtnChatMostrar : MonoBehaviour
{
    public string nombre;

    public void MostrarOcultar()
    {
        GestorChatTxtUI.singleton.MostrarOcultar(nombre);
    }
}
