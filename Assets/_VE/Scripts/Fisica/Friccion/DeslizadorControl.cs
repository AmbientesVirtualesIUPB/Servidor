using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeslizadorControl : MonoBehaviour
{
    [Range(0,1)]
    public float t;
    public Deslizante[] deslizante;

    public static DeslizadorControl singleton;

    private void Awake()
    {
        singleton = this;
    }

    private void Update()
    {
        for (int i = 0; i < deslizante.Length; i++)
        {
            deslizante[i].t = t;
        }
    }
}
