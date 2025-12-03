using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class TrebuchetControl : MonoBehaviour
{
    public float tiempoMov;
    public int iteraciones = 200;
    public Deslizante deslizante;

    [ContextMenu("Mover")]
    public void Mover()
    {
        StartCoroutine(Moviendo());
    }

    IEnumerator Moviendo()
    {
        float t = 0;
        for (int i = 0; i < 200; i++)
        {
            yield return new WaitForSeconds(tiempoMov / iteraciones);
            t += 1f / (float)iteraciones;
            deslizante.t = t;
        }
    }
}
