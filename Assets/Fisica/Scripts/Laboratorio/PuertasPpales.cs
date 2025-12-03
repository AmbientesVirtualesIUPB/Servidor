using UnityEngine;

public class PuertasPpales : MonoBehaviour
{
    [Header("Puertas")]
    public Transform puertaSuperior;
    public Transform puertaInferior;

    [Header("Posiciones de la Puerta Superior")]
    public Transform superiorCerrada;
    public Transform superiorAbierta;

    [Header("Posiciones de la Puerta Inferior")]
    public Transform inferiorCerrada;
    public Transform inferiorAbierta;

    [Header("Control")]
    public bool abierta = false;
    public float velocidad = 2f;

    void Update()
    {
        // Determina las posiciones objetivo
        Vector3 objetivoSuperior = abierta ? superiorAbierta.position : superiorCerrada.position;
        Vector3 objetivoInferior = abierta ? inferiorAbierta.position : inferiorCerrada.position;

        // Interpolación suave de posición
        puertaSuperior.position = Vector3.Lerp(puertaSuperior.position, objetivoSuperior, Time.deltaTime * velocidad);
        puertaInferior.position = Vector3.Lerp(puertaInferior.position, objetivoInferior, Time.deltaTime * velocidad);
    }
}
