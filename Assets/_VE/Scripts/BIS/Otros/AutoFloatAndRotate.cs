using UnityEngine;

public class AutoFloatAndRotate : MonoBehaviour
{
    [Header("Rotación")]
    public float rotationSpeed = 50f;   // Grados por segundo

    [Header("Oscilación vertical")]
    public float altura = 0.2f;         // Qué tanto sube y baja
    public float velocidad = 2f;        // Qué tan rápido oscila

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardamos la posición desde donde empieza a flotar
        posicionInicial = transform.position;
    }

    void Update()
    {
        // 1. Rotar sobre su propio eje (aquí eje Y, puedes cambiarlo)
        transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);

        // 2. Movimiento de subida y bajada suave
        float offsetY = Mathf.Sin(Time.time * velocidad) * altura;
        transform.position = posicionInicial + new Vector3(0f, offsetY, 0f);
    }
}
