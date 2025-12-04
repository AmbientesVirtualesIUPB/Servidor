using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Cinematica : MonoBehaviour
{
    public Transform[] posicionesCamara;
    public RotacionObjetoMotores[] llantasCarro;
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

        StartCoroutine(CinematicaInicial());
    }

    IEnumerator CinematicaInicial()
    {
        yield return new WaitForSeconds(0.5f);
        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesCamara[1], 5f);

        yield return new WaitForSeconds(3f);

        IniciarFadeOut();

        yield return new WaitForSeconds(2f);
        CamaraOrbital.singleton.transform.position = posicionesCamara[2].transform.position;
        CamaraOrbital.singleton.transform.rotation = posicionesCamara[2].transform.rotation;

        IniciarFadeIn();

        yield return new WaitForSeconds(1f);

        ControlCamaraMotor.singleton.IniciarMovimientoCamara(posicionesCamara[3], 5f);

        yield return new WaitForSeconds(3f);

        IniciarFadeOut();
    }

    // Update is called once per frame
    void Update()
    {
        
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
}
