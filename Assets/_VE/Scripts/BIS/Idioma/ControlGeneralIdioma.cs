using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlGeneralIdioma : MonoBehaviour
{
    public static ControlGeneralIdioma singleton;

    public string idioma = "ESP";
    public List<ControlIdioma> listaIdiomas;

    public GameObject español;
    public GameObject ingles;
    public GameObject portugues;
    public GameObject frances;

    public GameObject contenedor;

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
        español.SetActive(true);
        ingles.SetActive(false);
        portugues.SetActive(false);
        frances.SetActive(false);
        contenedor.SetActive(false);
    }
    public void Ingles()
    {
        CambiarIdioma("ENG");
        español.SetActive(false);
        ingles.SetActive(true);
        portugues.SetActive(false);
        frances.SetActive(false);
        contenedor.SetActive(false);
    }
    public void Portuges()
    {
        CambiarIdioma("POR");
        español.SetActive(false);
        ingles.SetActive(false);
        portugues.SetActive(true);
        frances.SetActive(false);
        contenedor.SetActive(false);
    }
    public void Frances()
    {
        CambiarIdioma("FRA");
        español.SetActive(false);
        ingles.SetActive(false);
        portugues.SetActive(false);
        frances.SetActive(true);
        contenedor.SetActive(false);
    }
  
}
