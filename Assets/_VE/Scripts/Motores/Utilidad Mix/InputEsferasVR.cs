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
    public bool desatornillando;
    public bool primeraInteraccion;

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

        if (desatornillando && ManagerMinijuego.singleton.sliderTorqueMinijuego.value > 0)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectLoopInt(0, 1f); // Ejecutamos el efecto de tuerca
            desatornillando = false;
        }

        if (botonPresionado && !sonidoEjecutado && ManagerMinijuego.singleton.sliderTorqueMinijuego.value < 100)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectLoopInt(0, 1f); // Ejecutamos el efecto de tuerca
            sonidoEjecutado = true;
        }

        if (ManagerMinijuego.singleton.sliderTorqueMinijuego.value >= 100 || ManagerMinijuego.singleton.sliderTorqueMinijuego.value <= 0)
        {
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.DetenerLoop(); // Detenemos el efecto 
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
                    primeraInteraccion = true;
                    botonNoPresionado = false;
                    desatornillar = false;
                }
                else
                {
                    if (AudioManagerMotores.singleton != null && primeraInteraccion) AudioManagerMotores.singleton.DetenerLoop(); // Detenemos el efecto 
                    botonPresionado = false;
                    sonidoEjecutado = false;
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
                desatornillando = true;
            }
            else
            {
                if (ManagerCanvas.singleton != null)
                {
                    if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("Error", 0.6f); // Ejecutamos el efecto nombrado

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
            primeraInteraccion = false;
        }
    }
}
