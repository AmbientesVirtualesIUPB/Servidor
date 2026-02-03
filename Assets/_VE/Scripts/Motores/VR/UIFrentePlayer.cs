using UnityEngine;

public class UIFrentePlayer : MonoBehaviour
{
    [Header("Referencia")]
    public Transform camara;   // Main Camera 

    [Header("Distancia y Offset")]
    public float distancia = 2f;
    public Vector3 offset;     // Ajustable en X, Y, Z

    [Header("Rotación")]
    public bool soloRotarEnY = true;
    public bool suavizarMovimiento = true;
    public float suavizado = 8f;

    private void Start()
    {
        camara = Camera.main.transform;
    }
    void LateUpdate()
    {
        if (!camara) return;

        // Dirección hacia adelante de la cámara
        Vector3 forward = camara.forward;

        if (soloRotarEnY)
        {
            forward.y = 0;
            forward.Normalize();
        }

        // Posición objetivo
        Vector3 posicionObjetivo = camara.position + forward * distancia + camara.right * offset.x + camara.up * offset.y + forward * offset.z;


        // Movimiento
        if (suavizarMovimiento)
        {
            transform.position = Vector3.Lerp(transform.position, posicionObjetivo, Time.deltaTime * suavizado);
        }
        else
        {
            transform.position = posicionObjetivo;
        }

        // Rotación para mirar a la cámara
        Vector3 mirar = camara.position - transform.position;
        if (soloRotarEnY) mirar.y = 0;

        transform.rotation = Quaternion.LookRotation(mirar);
    }
}
