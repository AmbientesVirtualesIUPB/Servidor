using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControlSliderVelocidadMotor : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public ControlVelocidadAnimacion controlVelocidadAnimacion;
    public TextMeshProUGUI txtbotonEncender;
    public float duracionApagado;
    public bool esDinamometro;
    private bool motorEjecutadoDinamometro;

    private Slider sliderVelocidad;
    private bool motorEncendido;
    private bool apagandoMotor;
    private Coroutine coroutine;

    private void Awake()
    {
        sliderVelocidad = GetComponent<Slider>();
    }

    private void Update()
    {
        if (motorEncendido && sliderVelocidad.value == 0f)
        {
            sliderVelocidad.interactable = false;
            sliderVelocidad.value = 0f; // aseguramos el valor final
            txtbotonEncender.text = "Encender";
            motorEncendido = false;
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.DetenerLoop(); // Detenemos el sonido loop
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("MotorApagado", 0.2f); // Ejecutamos el efecto nombrado
        }

        if (esDinamometro && sliderVelocidad.value > 0f && !motorEjecutadoDinamometro && !MesaMotor.singleton.estoyArmando)
        {
            motorEjecutadoDinamometro = true;
            StartCoroutine(IniciarEncendido());
        }

        if (esDinamometro && sliderVelocidad.value == 0f && motorEjecutadoDinamometro && !MesaMotor.singleton.estoyArmando)
        {
            motorEjecutadoDinamometro = false;
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.DetenerLoop(); // Detenemos el sonido loop
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("MotorApagado", 0.2f); // Ejecutamos el efecto nombrado
        }
    }

    /// <summary>
    /// Metodo invocado al momento de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!apagandoMotor)
        {
            controlVelocidadAnimacion.puedoValidar = true;
        }
        
    }

    /// <summary>
    /// Metodo invocado al momento de dejar de manipular el slider
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!apagandoMotor)
        {
            controlVelocidadAnimacion.puedoValidar = false;
        }    
    }

    /// <summary>
    /// Metodo invocado desde btnEncenderMotor en el canvas principal
    /// </summary>
    public void EncenderMotor()
    {
        if (ManagerMinijuego.singleton.minijuegoValidadoCorrectamente)
        {
            if (!motorEncendido)
            {
                sliderVelocidad.interactable = true;
                sliderVelocidad.value = 0.1f;
                txtbotonEncender.text = "Apagar";
                controlVelocidadAnimacion.puedoValidar = true;
                StartCoroutine(IniciarEncendido());
            }
        }
        else
        {
            sliderVelocidad.value = 0.1f;
        }
    }

    private IEnumerator IniciarEncendido()
    {
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("MotorEncendido", 0.5f); // Ejecutamos el efecto nombrado

        yield return new WaitForSeconds(0.5f);

        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectLoopInt(2, 0.08f); // Ejecutamos el efecto de Motor encendido
    }

    /// <summary>
    /// Metodo invocado desde btnEncenderMotor en el canvas principal
    /// </summary>
    public void ApagarMotor()
    {
        if (ManagerMinijuego.singleton.minijuegoValidadoCorrectamente)
        {
            if (motorEncendido)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
                if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.DetenerLoop(); // Detenemos el sonido loop
                if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("MotorApagado", 0.2f); // Ejecutamos el efecto nombrado
                apagandoMotor = true;
                coroutine = StartCoroutine(BajarValorSlider());
                return;
            }
            motorEncendido = true;
        }     
    }

     private IEnumerator BajarValorSlider()
    {
        controlVelocidadAnimacion.puedoValidar = true;
        sliderVelocidad.interactable = false;
        txtbotonEncender.text = "Apagando";

        float inicial = sliderVelocidad.value;
        float tiempo = 0f;

        while (tiempo < duracionApagado)
        {
            sliderVelocidad.value = Mathf.Lerp(inicial, 0f, tiempo / duracionApagado);
            tiempo += Time.deltaTime;
            yield return null;
        }

        
        controlVelocidadAnimacion.puedoValidar = false;
        sliderVelocidad.value = 0f; // aseguramos el valor final
        txtbotonEncender.text = "Encender";
        motorEncendido = false;
        apagandoMotor = false;
    }
}
