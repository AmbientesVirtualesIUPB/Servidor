using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;

public class Miccroscopio : MonoBehaviour
{
    public Transform[] lente;
    public Transform Zoom;
    public Transform Enfoque;
    public Transform p0;
    public Transform p1;

    [Range(0f, 1f)] public float t0;
    [Range(0f, 1f)] public float t1;
    public Slider slider0;
    public Slider slider1;

    public Material m;
    float desenfoque;

    public float velocidadRotacion = 180f;

    [Header("Luz")]
    public Light luzMicroscopio;
    public float intensidadMin = 0.8f;
    public float intensidadMax = 2.5f;
    public float suavizadoIntensidad = 10f;

    public bool puedoActualizar;
    public void SetT0(float value) => slider0.value = Mathf.Clamp01(value);
    public void SetT1(float value) => t1 = Mathf.Clamp01(value);

    void Update()
    {
        for (int i = 0; i < lente.Length; i++)
        {
            lente[i].position = Vector3.Lerp(p0.position, p1.position, slider0.value); 

            desenfoque = (Mathf.Abs(slider1.value - slider0.value)) * 0.03f;
            m.SetFloat("_Desenfoque", desenfoque);
        }

        if (puedoActualizar)
        {
            GestionMensajesServidor.singeton.EnviarMensaje("BS01", slider0.value.ToString("F3") + "*" + slider1.value.ToString("F3"));
        }

        float anguloZoom = slider0.value * 180f;
        float anguloEnfoque = slider1.value * 180f;

        Quaternion rotZoomObj = Quaternion.Euler(anguloZoom, 0f, 0f);
        Quaternion rotEnfObj = Quaternion.Euler(anguloEnfoque, 0f, 0f);

        Zoom.localRotation = Quaternion.RotateTowards(Zoom.localRotation, rotZoomObj, velocidadRotacion * Time.deltaTime);
        Enfoque.localRotation = Quaternion.RotateTowards(Enfoque.localRotation, rotEnfObj, velocidadRotacion * Time.deltaTime);

        if (luzMicroscopio != null)
        {
            float objetivo = Mathf.Lerp(intensidadMin, intensidadMax, slider0.value);
            luzMicroscopio.intensity = Mathf.Lerp(
                luzMicroscopio.intensity,
                objetivo,
                1f - Mathf.Exp(-suavizadoIntensidad * Time.deltaTime)
            );
        }
    }
}
