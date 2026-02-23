using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputEsferasVR : MonoBehaviour
{
    public string nombreMano;
    public InputActionProperty agarrarBoton;
    public bool agarrarPresionado;
    public bool botonPresionado;
    public bool botonNoPresionado;
    public bool desatornillar;

    public Slider slider;
    public float velocidad = 0.5f; // unidades por segundo
    private bool sonidoEjecutado = false;

    private void Start()
    {
        slider = ManagerMinijuego.singleton.sliderTorqueMinijuego;
    }
    private void Update()
    {
        agarrarPresionado = agarrarBoton.action.ReadValue<float>() > 0.5f;

        if (botonPresionado)
        {
            slider.value += velocidad * Time.deltaTime;
        }
        else if (desatornillar)
        {
            slider.value += - velocidad * Time.deltaTime;
        }

        if (botonPresionado && !sonidoEjecutado)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectLoopInt(0, 1f); // Ejecutamos el efecto de tuerca
            sonidoEjecutado = true;
        }

        if (desatornillar && !sonidoEjecutado)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectLoopInt(0, 1f); // Ejecutamos el efecto de tuerca
            sonidoEjecutado = true;
        }
    }
    private void OnTriggerStay(Collider collision)
    {
        if (collision.CompareTag("ManoVR"))
        {
            if (InventarioUI.singleton.tamanoHerramienta == ManagerMinijuego.singleton.sizeHerramienta)
            {
                if (agarrarPresionado)
                {
                    botonPresionado = true;
                    botonNoPresionado = false;
                    desatornillar = false;
                    sonidoEjecutado = false;
                }
                else
                {
                    if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.DetenerLoop(); // Detenemos el efecto 
                    botonPresionado = false;
                    botonNoPresionado = true;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("ManoVR"))
        {
            if (InventarioUI.singleton.tamanoHerramienta == ManagerMinijuego.singleton.sizeHerramienta)
            {
                desatornillar = true;
                sonidoEjecutado = false;
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
                        for (int i = 0; i < RotacionAngularObjeto.singleton.herramientasManipulables.Length; i++)
                        {
                            if (RotacionAngularObjeto.singleton.herramientaTomada)
                            {
                                string texto = "Estas utilizando el tamaño de herramienta incorrecto, necesitas la llave o copa de " + ManagerMinijuego.singleton.sizeHerramienta + " mm, vuelve a intentarlo";
                                ManagerCanvas.singleton.AlertarMensaje(texto);
                            }
                            else
                            {
                                string texto = "No tienes nada en tu mano, debes ir a la caja caja de herramientas, necesitas la llave o copa de " + ManagerMinijuego.singleton.sizeHerramienta + " mm";
                                ManagerCanvas.singleton.AlertarMensaje(texto);
                            }
                        }
                    }
                }
            }
        }
    }

    private IEnumerator OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("ManoVR"))
        {
            yield return new WaitUntil(() => !agarrarPresionado);
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.DetenerLoop(); // Detenemos el efecto 
            sonidoEjecutado = false;
            botonPresionado = false;
            botonNoPresionado = false;
            desatornillar = false;
        }
    }
}
