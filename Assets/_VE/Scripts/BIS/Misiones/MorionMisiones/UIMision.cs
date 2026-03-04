using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMision : MonoBehaviour
{
    public ControlIdioma ciNombreMision;
    public ControlIdioma ciDescripcion;
    public List<ControlIdioma> ciSubMisiones;

    public GameObject prSubMisiones;
    public Transform padreSubMisiones;
    public int mision;
    public Color colorBien = Color.green;
    public Color colorMal = Color.red;

    IEnumerator Start()
    {
        yield return null;
        yield return null;
        //Inicializar();
    }
    public void Inicializar(int cual)
    {
        mision = cual;
        Inicializar();
    }
    public void Inicializar()
    {
        ciSubMisiones = new List<ControlIdioma> ();
        Mision mis = GestioMosionesPersonales.singleton.misiones[mision];
        ciNombreMision.ActualizarTexto(mis.nombre);
        ciDescripcion.ActualizarTexto(mis.descripcion);
        for (int i = 0; i < mis.subMisions.Length; i++)
        {
            GameObject go = Instantiate(prSubMisiones,padreSubMisiones);
            go.SetActive(true);
            ControlIdioma ciSM = go.GetComponent<ControlIdioma>();
            ciSM.ActualizarTexto(mis.subMisions[i].descripcion);
            Image im = go.GetComponent<Image>();
            im.color = mis.subMisions[i].rescatada?colorBien:colorMal;
            ciSubMisiones.Add(go.GetComponent<ControlIdioma>());
        }
    }

    public void Suicidio()
    {
        Destroy(gameObject);
    }
}
