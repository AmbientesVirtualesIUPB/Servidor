using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControladorCamaras : MonoBehaviour
{
    public Transform camaraPrincipal;
    public Transform[] posiciones;
    public RotacionAvatar rotacionAvatar;
    public float velocidadRotacion = 3;
    public float tiempoEspera = 2;

    private bool puedoRotar;
    private int posicionActual = 0;
    private void Update()
    {
        if (puedoRotar)
        {
            // Interpolación de posición
            camaraPrincipal.transform.position = Vector3.Lerp(camaraPrincipal.transform.position, posiciones[posicionActual].position, Time.deltaTime * velocidadRotacion);

            // Interpolación de rotación
            camaraPrincipal.transform.rotation = Quaternion.Lerp(camaraPrincipal.transform.rotation, posiciones[posicionActual].rotation, Time.deltaTime * velocidadRotacion);
            // Alternativamente para una rotación más natural puedes usar:
            // transform.rotation = Quaternion.Slerp(transform.rotation, objetivo.rotation, Time.deltaTime * velocidad);   
        }
    }

    public void CambiarCamara(int id)
    {
        posicionActual = id;

        if (id == 0 || id == 2 || id == 3)
        {  
            StartCoroutine(PuedoRotar());
        }
        else
        {
            puedoRotar = true;
            rotacionAvatar.enabled = false;        
        }
    }

    public IEnumerator PuedoRotar()
    {
        puedoRotar = true;
        rotacionAvatar.enabled = false;

        yield return new WaitForSeconds(tiempoEspera);

        puedoRotar = false;
        rotacionAvatar.enabled = true;    
    }
}
