using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrebuchetControl : MonoBehaviour
{
    public float        tiempoMov;
    public int          iteraciones = 200;
    public Deslizante   deslizante;
    public Transform    target;

    [ContextMenu("Mover")]
    public void Mover()
    {
        StartCoroutine(Moviendo());
    }

    IEnumerator Moviendo()
    {
        float t = 0;
        for (int i = 0; i < iteraciones; i++)
        {
            yield return new WaitForSeconds(tiempoMov / iteraciones);
            t += 1f / (float)iteraciones;
            deslizante.t = t;
        }
        GameObject go = new GameObject();
        go.transform.position = transform.position;
        go.transform.LookAt(target);
        Quaternion q = transform.rotation;
        for (int i = 0; i < 100; i++)
        {
            transform.rotation = Quaternion.Lerp(q, go.transform.rotation, i / 100f);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
