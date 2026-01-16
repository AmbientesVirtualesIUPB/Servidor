using System.Collections;
using TMPro;
using UnityEngine;

public class TooltipManagerCanvas : MonoBehaviour
{
    [Header("REFERENCIAS")]
    public GameObject baseToolTip;
    public TextMeshProUGUI tooltipText;

    [Header("CONFIGURACIÓN")]
    public float delay = 0.5f;
    public Vector3 offset = new Vector3(0, 0.1f, 0);

    private Coroutine tooltipCoroutine;
    private Transform posicion;
    private CanvasGroup canvasGroup;

    public static TooltipManagerCanvas singleton;

    void Awake()
    {
        canvasGroup = baseToolTip.GetComponent<CanvasGroup>();

        // Configurar Singleton
        if (singleton == null)
        {
            singleton = this;
        }
        else
        {
            Destroy(this);
        }

        baseToolTip.SetActive(false);
    }

    public void MostrarToolTip(string text, Transform objetivo)
    {
        EsconderToolTIp();

        posicion = objetivo;
        tooltipCoroutine = StartCoroutine(MostrarDespuesDe(text));
    }

    public void EsconderToolTIp()
    {
        if (tooltipCoroutine != null)
            StopCoroutine(tooltipCoroutine);

        baseToolTip.SetActive(false);
        tooltipCoroutine = null;
    }

    IEnumerator MostrarDespuesDe(string text)
    {
        tooltipText.text = text;
        baseToolTip.SetActive(true);
        yield return new WaitForSeconds(delay);



        float t = 0;
        while (t < 2)
        {
            t += Time.deltaTime * 6;
            canvasGroup.alpha = t;
            yield return null;
        }
    }

    public void RestablecerAlfaCanvasGroup()
    {
        canvasGroup.alpha = 0;
    }

    void Update()
    {
        if (baseToolTip.activeSelf && posicion != null)
        {
            baseToolTip.transform.position = posicion.position + offset;
        }
    }
}
