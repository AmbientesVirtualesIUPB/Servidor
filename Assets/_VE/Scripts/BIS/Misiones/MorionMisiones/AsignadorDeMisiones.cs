using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsignadorDeMisiones : MonoBehaviour
{
    public int idMision;

    private void Start()
    {
        ///////////temporal
        ///
        AsignarMision();
    }
    [ContextMenu("Asignar")]
    public void AsignarMision()
    {
        GestioMosionesPersonales.singleton.AdquirirMision(idMision);
    }
}
