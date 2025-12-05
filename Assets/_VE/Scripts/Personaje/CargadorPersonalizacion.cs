using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CargadorPersonalizacion : MonoBehaviour
{
    [InfoMessage("Este es un campo OBLIGATORIO para que funcione, asegúrate de configurarlo correctamente.", MessageTypeCustom.Error)]
    public Personalizacion3 basePersonalizacion;
    private void Start()
    {

        Cargar();
    }

    [ContextMenu("Cargar")]
    public void Cargar()
    {
        if (EnvioDatosBD.singleton != null)
        {
            basePersonalizacion.CargarDesdeTexto(EnvioDatosBD.singleton.usuario.personalizacion);
        }
        else
        {
            basePersonalizacion.CuerpoAleatorio();
        }
    }
}
