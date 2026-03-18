using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MorionMisiones : MonoBehaviour
{
    public Mision[] misiones;
    public static MorionMisiones singleton;

    private void Awake()
    {
        singleton = this;
    }
}

[System.Serializable]
public class Mision
{
    public int       nombre;
    public int       descripcion;
    public SubMision[]  subMisions;
    public UnityEvent misionCumplida;
    public bool VerificaroCompletado() 
    {
        for (int i = 0; i < subMisions.Length; i++)
        {
            if (!subMisions[i].rescatada)
            {
                return false;
            }
        }
    return true;
    }
}

[System.Serializable]
public class SubMision
{
    public int          descripcion;
    public bool         rescatada;
    public Transform    puntoObjetivo;
}