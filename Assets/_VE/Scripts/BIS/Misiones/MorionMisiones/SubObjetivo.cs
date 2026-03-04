using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubObjetivo : MonoBehaviour
{
    public int mision;
    public int subMision;

    public bool CompletarObjetivo()
    {
        return GestioMosionesPersonales.singleton.CompletarSubMision(mision,subMision);
    }
}
