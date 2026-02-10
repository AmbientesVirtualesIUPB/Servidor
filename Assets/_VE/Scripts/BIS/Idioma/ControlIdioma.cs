using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControlIdioma : MonoBehaviour
{
    public Text txt;
    public TextMeshProUGUI tmp;
    public int texto;
    
    private void Start()
    {
        ActualizarTexto();
        if(ControlGeneralIdioma.singleton != null)
        {
            ControlGeneralIdioma.singleton.AddIdioma(this);
        }
    }

    public void ActualizarTexto()
    {
        if (txt != null)
        {
            txt.text = BaseDeTextosPorIdioma.configuracionDefault.ObtenerTexto(texto);
        }
        if(tmp != null)
        {
            tmp.text = BaseDeTextosPorIdioma.configuracionDefault.ObtenerTexto(texto);
        }
    }

    private void OnDrawGizmosSelected()
    {
        ActualizarTexto();
    }

}
