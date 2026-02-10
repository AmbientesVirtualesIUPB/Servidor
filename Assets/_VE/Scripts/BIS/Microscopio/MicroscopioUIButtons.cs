using UnityEngine;

public class MicroscopioUIButtons : MonoBehaviour

{
    [Header("Referencia")]
    public Miccroscopio microscopio;

    [Header("Botones: velocidad")]
    public float velocidadCambio = 0.35f; // cuánto cambia t0 por segundo

    [Header("Perilla UI (gira esta)")]
    public RectTransform perillaZoomUI; // pon aquí el HIJO ImgZoomKnob (no el root)
    public float anguloMax = 180f;
    public float suavizadoGiro = 25f;

    bool zoomMas;
    bool zoomMenos;
    float anguloZoomActual;

    void Start()
    {
        if (microscopio != null)
            anguloZoomActual = microscopio.t0 * anguloMax;
        AplicarRotacionInstantanea();
    }

    void Update()
    {
        if (microscopio == null) return;

        float dt = Time.deltaTime * velocidadCambio;

        if (zoomMas) microscopio.SetT0(microscopio.t0 + dt);
        if (zoomMenos) microscopio.SetT0(microscopio.t0 - dt);

        // rotación sincronizada al valor real
        float objetivo = microscopio.t0 * anguloMax;
        anguloZoomActual = Mathf.Lerp(anguloZoomActual, objetivo, 1f - Mathf.Exp(-suavizadoGiro * Time.deltaTime));

        // Si te gira al revés, cambia el signo: (anguloZoomActual) en vez de (-anguloZoomActual)
        if (perillaZoomUI != null)
            perillaZoomUI.localRotation = Quaternion.Euler(0f, 0f, -anguloZoomActual);
    }

    void AplicarRotacionInstantanea()
    {
        if (microscopio == null || perillaZoomUI == null) return;
        float objetivo = microscopio.t0 * anguloMax;
        anguloZoomActual = objetivo;
        perillaZoomUI.localRotation = Quaternion.Euler(0f, 0f, -anguloZoomActual);
    }

    // ----- Estos son los que vas a conectar en EventTrigger -----
    public void ZoomMas_Down() => zoomMas = true;
    public void ZoomMas_Up() => zoomMas = false;

    public void ZoomMenos_Down() => zoomMenos = true;
    public void ZoomMenos_Up() => zoomMenos = false;
}
