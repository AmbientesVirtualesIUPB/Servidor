using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Cinematica : MonoBehaviour
{
    public GameObject[] objetosCanvas;
    public Transform[] posicionesCamara;
    public GameObject[] luces;
    public GameObject menuTutorial;
    public GameObject menuCinematica;

    public TextMeshProUGUI txtTitulo;
    public RotacionObjetoMotores[] llantasCarroNegativas;
    public RotacionObjetoMotores[] llantasCarroPositivas;
    public MoverObjeto carro;
    public MoverObjeto puerta;

    [Header("CONFIGURACION FADE IN FADE OUT ")]
    public Image imagen;          // La imagen que quieres hacer fade
    public float duracion = 1f;   // Duración del fade

    private Coroutine coroutine;

    public static Cinematica singleton;

    private void Awake()
    {
        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        CamaraOrbital.singleton.transform.position = posicionesCamara[0].transform.position;
        CamaraOrbital.singleton.transform.rotation = posicionesCamara[0].transform.rotation;

        for (int i = 0; i < objetosCanvas.Length; i++)
        {
            objetosCanvas[i].gameObject.SetActive(false);
        }

        StartCoroutine(CinematicaInicial());
    }

    IEnumerator CinematicaInicial()
    {
        IniciarFadeIn();

        yield return new WaitForSeconds(1f);

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesCamara[1], 5f);

        yield return new WaitForSeconds(3f);

        IniciarFadeOut();

        yield return new WaitForSeconds(2f);

        txtTitulo.text = "AL AMBIENTE VIRTUAL \n DE MECANICA Y MOTORES";
        CamaraOrbital.singleton.transform.position = posicionesCamara[2].transform.position;
        CamaraOrbital.singleton.transform.rotation = posicionesCamara[2].transform.rotation;

        IniciarFadeIn();

        yield return new WaitForSeconds(1f);

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesCamara[3], 5f);

        yield return new WaitForSeconds(4f);

        IniciarFadeOut();

        yield return new WaitForSeconds(2f);

        txtTitulo.text = "DESARROLLADO POR EL EQUIPO \n DE AMBIENTES VIRTUALES";
        CamaraOrbital.singleton.transform.position = posicionesCamara[4].transform.position;
        CamaraOrbital.singleton.transform.rotation = posicionesCamara[4].transform.rotation;

        IniciarFadeIn();

        yield return new WaitForSeconds(1f);

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesCamara[5], 5f);


        yield return new WaitForSeconds(4f);

        IniciarFadeOut();

        yield return new WaitForSeconds(2f);

        txtTitulo.text = "DE LA INSTITUCION UNIVERSITARIA \n PASCUAL BRAVO";
        CamaraOrbital.singleton.transform.position = posicionesCamara[6].transform.position;
        CamaraOrbital.singleton.transform.rotation = posicionesCamara[6].transform.rotation;

        IniciarFadeIn();

        yield return new WaitForSeconds(1f);

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesCamara[7], 5f);
        puerta.IniciarDesplazamientoObjeto();
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("PuertaElevadiza",0.3f); // Ejecutamos el efecto nombrado
        carro.IniciarDesplazamientoObjeto();

        for (int i = 0; i < llantasCarroNegativas.Length; i++)
        {
            llantasCarroNegativas[i].velocidadRotacion = -60;
            llantasCarroPositivas[i].velocidadRotacion = 60;
        }
    
        yield return new WaitForSeconds(8f);

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesCamara[8], 5f);
        puerta.RetornarPosicionOriginal();
        if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("PuertaElevadiza", 0.3f); // Ejecutamos el efecto nombrado


        yield return new WaitForSeconds(1f);
        for (int i = 0; i < luces.Length; i++)
        {
            yield return new WaitForSeconds(0.5f);
            if (AudioManagerMotores.singleton != null) AudioManagerMotores.singleton.PlayEfectString("Luces", 0.3f); // Ejecutamos el efecto nombrado
            luces[i].SetActive(true);
        }

        for (int i = 0; i < llantasCarroNegativas.Length; i++)
        {
            llantasCarroNegativas[i].velocidadRotacion = 0;
            llantasCarroPositivas[i].velocidadRotacion = 0;
        }

        yield return new WaitForSeconds(3f);

        IniciarFadeOut();

        yield return new WaitForSeconds(2f);

        for (int i = 0; i < objetosCanvas.Length; i++)
        {
            objetosCanvas[i].gameObject.SetActive(true);
        }

        menuTutorial.SetActive(true);
        txtTitulo.text = "BIENVENIDO";
        menuCinematica.SetActive(false);
        IniciarFadeIn();
    }

    [ContextMenu("in")]
    public void IniciarFadeIn()
    {
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(FadeIn());
    }

    public IEnumerator FadeIn()
    {
        Color c = imagen.color;
        float t = 0;

        while (t < duracion)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duracion);
            imagen.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }

    [ContextMenu("out")]
    public void IniciarFadeOut()
    {
        if (coroutine != null) StopCoroutine(coroutine);

        coroutine = StartCoroutine(FadeOut());
    }

    public IEnumerator FadeOut()
    {
        Color c = imagen.color;
        float t = 0;

        while (t < duracion)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / duracion);
            imagen.color = new Color(c.r, c.g, c.b, alpha);
            yield return null;
        }
    }

    public void DetenerCinematica()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        StopCoroutine(CinematicaInicial());

        puerta.RetornarPosicionOriginal();
        carro.RegresarPosicionOriginal();

        for (int i = 0; i < llantasCarroNegativas.Length; i++)
        {
            llantasCarroNegativas[i].velocidadRotacion = 0;
            llantasCarroPositivas[i].velocidadRotacion = 0;
        }

        for (int i = 0; i < objetosCanvas.Length; i++)
        {
            objetosCanvas[i].gameObject.SetActive(true);
        }

        if (EnvioDatosBD.singleton != null)
        {
            ManagerCanvas.singleton.btnEleccionMotor.SetActive(EnvioDatosBD.singleton.usuario.tipo_usuario == "1");

            ListaUsuariosMotores.singleton.MostrarBotonListaUsuarios();

            if (ServidorMotores.singleton.esMecanico)
            {
                ManagerCanvas.singleton.imagenBLoqueoMotor.SetActive(false);

                if (EnvioDatosBD.singleton.usuario.tipo_usuario == "1")
                {
                    ManagerCanvas.singleton.imagenBLoqueoMecanico.SetActive(false);
                }
            }
            else
            {
                ManagerCanvas.singleton.imagenBLoqueoMotor.SetActive(true);
                ManagerCanvas.singleton.btnEleccionMotor.SetActive(true);
            }
        }
        else
        {
            Debug.LogError("Falta envio datos DB en la scena");
        }
        
        
        ControlCamaraMotor.singleton.DetenerMovimientosCamara();
        menuTutorial.SetActive(true);
        txtTitulo.text = "BIENVENIDO";
        menuCinematica.SetActive(false);
        this.gameObject.SetActive(false);
    }
}
