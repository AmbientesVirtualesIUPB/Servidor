using UnityEngine;
using UnityEngine.EventSystems;

public class MicroscopioUI : MonoBehaviour
{
    [Header("Referencia al Microscopio")]
    public Miccroscopio microscopio;

    [Header("Velocidad de cambio (mantener presionado)")]
    public float velocidadCambio = 0.35f; // unidades por segundo (0..1)

    [Header("Perillas UI (RectTransform de la imagen que gira)")]
    public RectTransform perillaZoomUI;
    public RectTransform perillaEnfoqueUI;

    [Header("Rotación visual")]
    public float anguloMax = 180f;     // 0..1 -> 0..180 grados
    public bool invertirGiro = true;   // si gira al revés, cámbialo
    public float suavizadoGiro = 25f;  // visual (solo UI)

    int dirZoom = 0;     // -1, 0, +1
    int dirEnfoque = 0;  // -1, 0, +1

    float anguloZoomActual;
    float anguloEnfoqueActual;

    void Start()
    {
        if (microscopio != null)
        {
            anguloZoomActual = microscopio.t0 * anguloMax;
            anguloEnfoqueActual = microscopio.t1 * anguloMax;
            AplicarRotacionInstantanea();
        }
    }

    void Update()
    {
        if (microscopio == null) return;

        float dt = Time.deltaTime * velocidadCambio;

        // 1) Cambiar valores por dirección (mantener presionado)
        if (dirZoom != 0)
            microscopio.SetT0(microscopio.t0 + dirZoom * dt);

        if (dirEnfoque != 0)
            microscopio.SetT1(microscopio.t1 + dirEnfoque * dt);

        // 2) Sincronizar perillas con el valor real (t0/t1)
        float objetivoZoom = microscopio.t0 * anguloMax;
        float objetivoEnf = microscopio.t1 * anguloMax;

        anguloZoomActual = Mathf.Lerp(anguloZoomActual, objetivoZoom, 1f - Mathf.Exp(-suavizadoGiro * Time.deltaTime));
        anguloEnfoqueActual = Mathf.Lerp(anguloEnfoqueActual, objetivoEnf, 1f - Mathf.Exp(-suavizadoGiro * Time.deltaTime));

        float signo = invertirGiro ? -1f : 1f;

        if (perillaZoomUI != null)
            perillaZoomUI.localRotation = Quaternion.Euler(0f, 0f, signo * anguloZoomActual);

        if (perillaEnfoqueUI != null)
            perillaEnfoqueUI.localRotation = Quaternion.Euler(0f, 0f, signo * anguloEnfoqueActual);
    }

    void AplicarRotacionInstantanea()
    {
        if (microscopio == null) return;

        float signo = invertirGiro ? -1f : 1f;

        if (perillaZoomUI != null)
            perillaZoomUI.localRotation = Quaternion.Euler(0f, 0f, signo * (microscopio.t0 * anguloMax));

        if (perillaEnfoqueUI != null)
            perillaEnfoqueUI.localRotation = Quaternion.Euler(0f, 0f, signo * (microscopio.t1 * anguloMax));
    }

    // ====== Estos métodos los usan los botones (no tú manualmente) ======
    public void SetDirZoom(int dir) => dirZoom = Mathf.Clamp(dir, -1, 1);
    public void SetDirEnfoque(int dir) => dirEnfoque = Mathf.Clamp(dir, -1, 1);

    // ===================================================================
    // COMPONENTE PARA BOTONES (EN EL MISMO ARCHIVO)
    // Se lo pones a cada botón y eliges qué hace (Zoom+/Zoom-/Enfoque+/Enfoque-)
    // ===================================================================
    public class BotonMantenerPresionado : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public MicroscopioUI ui;

        public enum Accion
        {
            ZoomMas,
            ZoomMenos,
            EnfoqueMas,
            EnfoqueMenos
        }

        public Accion accion;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (ui == null) return;

            switch (accion)
            {
                case Accion.ZoomMas: ui.SetDirZoom(+1); break;
                case Accion.ZoomMenos: ui.SetDirZoom(-1); break;
                case Accion.EnfoqueMas: ui.SetDirEnfoque(+1); break;
                case Accion.EnfoqueMenos: ui.SetDirEnfoque(-1); break;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Soltar();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // IMPORTANTÍSIMO: si sales del botón, que se suelte igual
            Soltar();
        }

        void Soltar()
        {
            if (ui == null) return;

            // Solo soltamos la dirección correspondiente (para no apagar la otra perilla)
            switch (accion)
            {
                case Accion.ZoomMas:
                case Accion.ZoomMenos:
                    ui.SetDirZoom(0);
                    break;

                case Accion.EnfoqueMas:
                case Accion.EnfoqueMenos:
                    ui.SetDirEnfoque(0);
                    break;
            }
        }
    }
}
    