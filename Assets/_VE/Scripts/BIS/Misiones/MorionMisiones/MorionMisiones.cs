using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

[System.Serializable]
public class SubMision
{
    public int   descripcion;
    public bool     rescatada;
}