using UnityEngine;
using UnityEngine.EventSystems;

public class ControlSliderMinijuego : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public RotacionAngularObjeto rotacionAngularObjeto;

    public bool presionado = false;
    private bool sonidoEjecutado = false;

    private void Start()
    {
        rotacionAngularObjeto = RotacionAngularObjeto.singleton;
    }

    
    void Update()
    {
        if (presionado && !sonidoEjecutado)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectLoopInt(0, 1f); // Ejecutamos el efecto de tuerca
            sonidoEjecutado = true;
        }
    }

    /// <summary>
    /// Metodo invocado al momento de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (Atornillar.singleton != null)
        {
            if (InventarioUI.singleton.tamanoHerramienta == ManagerMinijuego.singleton.sizeHerramienta)
            {
                presionado = true;
                rotacionAngularObjeto.estaManipulando = true;
            }
            else
            {
                if (ManagerCanvas.singleton != null)
                {
                    if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("Error", 1f); // Ejecutamos el efecto nombrado

                    if (ManagerMinijuego.singleton.sizeHerramienta == 1)
                    {
                        string texto = "Estas utilizando la herramienta incorrecta, necesitas prensa de valvulas para generar presión ";
                        ManagerCanvas.singleton.AlertarMensaje(texto);
                    }
                    else
                    {
                        if (InventarioHerramientas.singleton.herramientasTomadas.Count == 3 && InventarioHerramientas.singleton.herramientasIndividuales == null || InventarioHerramientas.singleton.herramientasIndividuales.Count == 0)
                        {
                            string texto = "No tienes nada en tu mano, debes ir a la caja caja de herramientas, necesitas la llave o copa de " + ManagerMinijuego.singleton.sizeHerramienta + " mm";
                            ManagerCanvas.singleton.AlertarMensaje(texto);
                        }
                        else
                        {
                            string texto = "Estas utilizando el tamaño de herramienta incorrecto, necesitas la llave o copa de " + ManagerMinijuego.singleton.sizeHerramienta + " mm, vuelve a intentarlo";
                            ManagerCanvas.singleton.AlertarMensaje(texto);
                        }
                    }               
                }         
            }
        }          
    }

    /// <summary>
    /// Metodo invocado al momento de dejar de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (Atornillar.singleton != null)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.DetenerLoop(); // Detenemos el efecto 
            presionado = false;
            sonidoEjecutado = false;
            rotacionAngularObjeto.estaManipulando = false;
        }        
    }
}
