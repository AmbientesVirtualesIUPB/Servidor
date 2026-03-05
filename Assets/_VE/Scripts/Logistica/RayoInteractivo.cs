using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayoInteractivo : MonoBehaviour
{
    public LayerMask capasInteractuables;


    public ObjetoInteractivo objetoActual;

    Ray rayo;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rayo = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(rayo, out hit,100, capasInteractuables))
        {
            ObjetoInteractivo oi = hit.transform.GetComponent<ObjetoInteractivo>();
            if (oi != null)
            {
                if (objetoActual != null)
                {
                    objetoActual.mouseSale.Invoke();
                }
                objetoActual = oi;
                objetoActual.mouseEntra.Invoke();
            }
            else
            {
                if (objetoActual != null)
                {
                    objetoActual.mouseSale.Invoke();
                }
            }
        }
        else
        {
            if (objetoActual != null)
            {
                objetoActual.mouseSale.Invoke();
                objetoActual = null;
            }

        }
    }
    private void LateUpdate()
    {
        if (objetoActual != null) 
            objetoActual.mouseSobre.Invoke();
    }
}
