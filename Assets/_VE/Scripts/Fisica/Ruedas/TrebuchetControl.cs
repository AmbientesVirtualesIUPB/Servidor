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
    public Transform    castilloEnemigo;

    [ContextMenu("Mover")]
    public void Mover()
    {
        if (trebuchet.radio > 1.5f && trebuchet.radio < 2)
        {
            StartCoroutine(Moviendo());
        }
        else
        {
            MSGBox.singleton.Mensaje("¡Rayos!", "Ese radio es muy complicado de mover para las cabras... :(");
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
        go.transform.position = trebuchet.transform.position;
        go.transform.LookAt(target);
        cabra1.SetBool("caminando", false);
        cabra2.SetBool("caminando", false);
        trebuchet.gameObject.GetComponent<OrientarHaciaNormal>().enabled = false;
        Quaternion q = trebuchet.transform.rotation;
        go.transform.forward = (castilloEnemigo.position - trebuchet.transform.position).normalized;
        for (int i = 0; i < 100; i++)
        {
            trebuchet.transform.rotation = Quaternion.Lerp(q, go.transform.rotation, i / 100f);
            yield return new WaitForSeconds(0.1f);
        }
        botonDisparo.interactable = true;
    }

    public void Disparar()
    {
        animTrebuchet.SetTrigger("disparar");
        Invoke("DestruirCastillo", 10);
    }

    void DestruirCastillo()
    {
        CastilloDestructor.instance.Destruir();
        Invoke("MensajeFinal", 10);
    }
    void MensajeFinal()
    {
        MSGBox.singleton.Mensaje("¡Victoria!", "Acabaste con la amenaza de los orcos, Felicitaciones");
    }
}
