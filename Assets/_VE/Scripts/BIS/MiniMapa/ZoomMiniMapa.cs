using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class ZoomMiniMapa : MonoBehaviour
{
    [Header("UI")]
    public RectTransform miniMapUI;
    public PuntoMisionMiniMapa puntoMisionMiniMapa;
    public Vector2 smallSize = new Vector2(200, 200);
    public Vector2 fullSize = new Vector2(900, 900);

    [Header("Cámara del minimapa")]
    public Camera miniMapCamera;
    public float smallOrthoSize = 30f;
    public float fullOrthoSize = 200f;

    [Header("Suavizado")]
    public float uiLerpSpeed = 8f;
    public float cameraLerpSpeed = 5f;

    [Header("Estado")]
    public bool estaExpandido;

    Vector2 originalAnchorMin;
    Vector2 originalAnchorMax;
    Vector2 originalPosition;

    Coroutine zoomRoutine;

    void Awake()
    {
        originalAnchorMin = miniMapUI.anchorMin;
        originalAnchorMax = miniMapUI.anchorMax;
        originalPosition = miniMapUI.anchoredPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMiniMap();
        }
    }

    public void ToggleMiniMap()
    {
        estaExpandido = !estaExpandido;

        if (!estaExpandido)
        {
            puntoMisionMiniMapa.mapMundoRadio = 50;
        }
        else
        {
            puntoMisionMiniMapa.mapMundoRadio = 203;
        }

        if (zoomRoutine != null) StopCoroutine(zoomRoutine);

        zoomRoutine = StartCoroutine(AnimarZoom());
    }

    IEnumerator AnimarZoom()
    {
        // Target UI
        Vector2 targetSize = estaExpandido ? fullSize : smallSize;
        Vector2 targetPos = estaExpandido ? Vector2.zero : originalPosition;

        Vector2 targetAnchor = estaExpandido ? new Vector2(0.5f, 0.5f) : originalAnchorMin;

        // Fijamos anclas UNA VEZ (no se interpolan)
        miniMapUI.anchorMin = targetAnchor;
        miniMapUI.anchorMax = targetAnchor;

        // Target cámara
        float targetOrtho = estaExpandido ? fullOrthoSize : smallOrthoSize;

        while (
            Vector2.Distance(miniMapUI.sizeDelta, targetSize) > 0.5f ||
            Mathf.Abs(miniMapCamera.orthographicSize - targetOrtho) > 0.5f
        )
        {
            // UI suavizada
            miniMapUI.sizeDelta = Vector2.Lerp(miniMapUI.sizeDelta, targetSize, Time.deltaTime * uiLerpSpeed);

            miniMapUI.anchoredPosition = Vector2.Lerp(miniMapUI.anchoredPosition, targetPos, Time.deltaTime * uiLerpSpeed);

            // Cámara suavizada
            miniMapCamera.orthographicSize = Mathf.Lerp(miniMapCamera.orthographicSize, targetOrtho, Time.deltaTime * cameraLerpSpeed);

            yield return null;
        }

        // Asegurar valores finales exactos
        miniMapUI.sizeDelta = targetSize;
        miniMapUI.anchoredPosition = targetPos;
        miniMapCamera.orthographicSize = targetOrtho;
    }
}
