using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MSGBox : MonoBehaviour
{

    public UIAutoAnimation animationUI;
    public Text txtTitulo;
    public Text txtMensaje;


    public static MSGBox singleton;

    private void Awake()
    {
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            DestroyImmediate(this);
        }
    }

    public void Aceptar()
    {
        animationUI.ExitAnimation();
    }

    public void Mensaje(string titulo, string mensaje)
    {
        txtTitulo.text = titulo;
        txtMensaje.text = mensaje;
        animationUI.EntranceAnimation();
    }
}
