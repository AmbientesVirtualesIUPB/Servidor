using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGeneralIdioma : MonoBehaviour
{
    public static ControlGeneralIdioma singleton;

    public string idioma = "ESP";
    public List<ControlIdioma> listaIdiomas;

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
        listaIdiomas = new List<ControlIdioma>();
    }

    public void AddIdioma(ControlIdioma controlIdioma)
    {
        if (!listaIdiomas.Contains(controlIdioma))
        {
            listaIdiomas .Add(controlIdioma);
        }
    }

    public void CambiarIdioma(string _idioma)
    {
        idioma = _idioma;
        ActualizarTodos();
    }
    [ContextMenu("Actualizar")]
    public void ActualizarTodos()
    {
        for (int i = 0; i < listaIdiomas.Count; i++) {
            if (listaIdiomas[i] != null) listaIdiomas[i].ActualizarTexto();
        }
    }

    public void Español() 
    {
        CambiarIdioma("ESP");
    }
    public void Ingles()
    {
        CambiarIdioma("ENG");
    }
    public void Elfico()
    {
        CambiarIdioma("ELF");
    }
}
