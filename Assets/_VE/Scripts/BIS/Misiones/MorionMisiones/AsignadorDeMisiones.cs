using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsignadorDeMisiones : MonoBehaviour
{
    public bool acutoAsignar = false;
    public int idMision;

    private void Start()
    {
        if (acutoAsignar)
        AsignarMision();
    }
    [ContextMenu("Asignar")]
    public void AsignarMision()
    {
        GestioMosionesPersonales.singleton.AdquirirMision(idMision);
    }
}
