using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrebuchetControl : MonoBehaviour
{
    public float        tiempoMov;
    public int          iteraciones = 200;
    public Deslizante   deslizante;
    public Transform    target;
    public Animator     cabra1;
    public Animator     cabra2;
    public Transform    huesoRuedas;
    public float        velRuedas = 70;
    public Button       botonDisparo;
    public Animator     animTrebuchet;
    public Trabuchet    trebuchet;
    public Transform[]  llantas;

    [ContextMenu("Mover")]
    public void Mover()
    {
        if (trebuchet.radio > 1.5f && trebuchet.radio < 2)
        {
            StartCoroutine(Moviendo());
        }
    }

    IEnumerator Moviendo()
    {
        GameObject go = new GameObject();
        float t = 0;
        cabra1.SetBool("caminando", true);
        cabra2.SetBool("caminando", true);
        for (int i = 0; i < iteraciones; i++)
        {
            yield return new WaitForSeconds(tiempoMov / iteraciones);
            t += 1f / (float)iteraciones;
            deslizante.t = t;
            huesoRuedas.Rotate(0, (tiempoMov / iteraciones) * velRuedas, 0);
            for (int j = 0; j < llantas.Length; j++)
            {
                llantas[j].Rotate((tiempoMov / iteraciones) * velRuedas, 0, 0);
            }
        }
        go.transform.position = transform.position;
        go.transform.LookAt(target);
        cabra1.SetBool("caminando", false);
        cabra2.SetBool("caminando", false);

        Quaternion q = transform.rotation;
        for (int i = 0; i < 100; i++)
        {
            transform.rotation = Quaternion.Lerp(q, go.transform.rotation, i / 100f);
            yield return new WaitForSeconds(0.1f);
        }
        botonDisparo.interactable = true;
    }

    public void Disparar()
    {
        animTrebuchet.SetTrigger("disparar");
    }
}
