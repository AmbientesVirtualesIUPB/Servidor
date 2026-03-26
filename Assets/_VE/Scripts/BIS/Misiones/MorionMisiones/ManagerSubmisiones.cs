using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class ManagerSubmisiones : MonoBehaviour
{
    public MovimientoJugador movimientoJugador; // Referencia al movimiento jugador principal
    [Header("Submision Microscopio")]
    public Sprite[] iconosMuestra;
    public Image imagenMuestra;
    public GameObject imgError;
    public Slider sliderZoom;
    public Slider sliderEnfoque;
    [HideInInspector]
    public float zoomObjetivo;
    [HideInInspector]
    public float enfoqueObjetivo;
    [HideInInspector]
    public InteractuadorMisiones muestraActiva;
    public GameObject btnSalirMicroscopio;

    [HideInInspector]
    public float rangoZoom = 0.04f;
    [HideInInspector]
    public float rangoEnfoque = 0.04f;

    [HideInInspector]
    public Vector3 posInicialCamara;
    [HideInInspector]
    public Quaternion rotInicialCamara;
    public Transform posicionCamara;
    public Transform posicionFinalCamara;
    public GameObject UIMicroscopio;
    [HideInInspector]
    public Transform camara; // Referencia a la cámara
    public float duracionMuvCam = 2f; // tiempo del movimiento
    private Coroutine movimiento;

    [Header("Fade Pantalla")]
    public Image imagenFade; // Imagen negra (UI)
    public float duracionFade = 0.5f;


    public static ManagerSubmisiones singleton;

    private void Awake()
    {
        singleton = this;
    }

    void Start()
    {
        if (camara == null)
        {
            camara = Camera.main.transform; // Toma la cámara automáticamente
        }

        movimientoJugador = ControlUsuarios.singleton.usuarioLocal.GetComponent<MovimientoJugador>();
    }

    public void HabilitarMicroscopio()
    {
        CamaraOrbital.singleton.DetenerCamara();
        movimientoJugador.DeneterJugador();
        if (movimiento != null)
        {
            StopCoroutine(movimiento);
        }

        movimiento = StartCoroutine(MoverCamaraInicialCoroutine());
    }

    public void DeshabilitarMicroscopio()
    {
        if (movimiento != null)
        {
            StopCoroutine(movimiento);
        }

        movimiento = StartCoroutine(MoverCamaraFinallCoroutine());
    }

    IEnumerator MoverCamaraInicialCoroutine()
    {
        posInicialCamara = camara.position;
        rotInicialCamara = camara.rotation;

        Vector3 posicionInicial = camara.position;
        Quaternion rotacionInicial = camara.rotation;

        float tiempo = 0f;

        while (tiempo < duracionMuvCam)
        {
            float t = tiempo / duracionMuvCam;

            camara.position = Vector3.Lerp(posicionInicial, posicionCamara.position, t);
            camara.rotation = Quaternion.Lerp(rotacionInicial, posicionCamara.rotation, t);

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Asegurar posición final exacta
        camara.position = posicionCamara.position;
        camara.rotation = posicionCamara.rotation;

        yield return StartCoroutine(Fade(0f, 1f));

        // Asegurar posición final exacta
        camara.position = posicionFinalCamara.position;
        camara.rotation = posicionFinalCamara.rotation;
        UIMicroscopio.SetActive(true);

        yield return StartCoroutine(Fade(1f, 0f));

        movimiento = null;
    }

    IEnumerator MoverCamaraFinallCoroutine()
    {
        yield return StartCoroutine(Fade(0f, 1f));

        camara.position = posicionCamara.position;
        camara.rotation = posicionCamara.rotation;
        UIMicroscopio.SetActive(false);

        yield return StartCoroutine(Fade(1f, 0f));

        Vector3 posicionInicial = camara.position;
        Quaternion rotacionInicial = camara.rotation;

        float tiempo = 0f;

        while (tiempo < duracionMuvCam)
        {
            float t = tiempo / duracionMuvCam;

            camara.position = Vector3.Lerp(posicionInicial, posInicialCamara, t);
            camara.rotation = Quaternion.Lerp(rotacionInicial, rotInicialCamara, t);

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Asegurar posición final exacta
        camara.position = posInicialCamara;
        camara.rotation = rotInicialCamara;
        CamaraOrbital.singleton.HabilitarCamara();
        movimientoJugador.HabilitarJugador();
        movimiento = null;
    }

    IEnumerator Fade(float alphaInicial, float alphaFinal)
    {
        float tiempo = 0f;

        Color color = imagenFade.color;

        while (tiempo < duracionFade)
        {
            float t = tiempo / duracionFade;
            float alpha = Mathf.Lerp(alphaInicial, alphaFinal, t);

            color.a = alpha;
            imagenFade.color = color;

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Asegurar valor final exacto
        color.a = alphaFinal;
        imagenFade.color = color;
    }

    public void AsignarObjetivos(int idMision)
    {
        if (idMision == 1)
        {
            zoomObjetivo = 0.4f;
            enfoqueObjetivo = 0.35f;
            imagenMuestra.sprite = iconosMuestra[0];
        }
        else if (idMision == 2)
        {
            zoomObjetivo = 0.7f;
            enfoqueObjetivo = 0.6f;
            imagenMuestra.sprite = iconosMuestra[1];
        }
    }

    /// <summary>
    /// Meotodo invocado en las submisiones
    /// </summary>
    /// <param name="muestra"></param>
    public void AsignarMuestraActiva(InteractuadorMisiones muestra)
    {
        muestraActiva = muestra;
    }

    public void ValidarMicroscopio()
    {
        float absolutoZoom = Mathf.Abs(zoomObjetivo - sliderZoom.value);
        float absolutoEnfoque = Mathf.Abs(enfoqueObjetivo - sliderEnfoque.value);

        if (absolutoZoom <= rangoZoom && absolutoEnfoque <= rangoEnfoque)
        {
            muestraActiva.Activar();
            btnSalirMicroscopio.gameObject.SetActive(true);
        }
        else
        {
            imgError.gameObject.SetActive(true);
        }
    }
}
