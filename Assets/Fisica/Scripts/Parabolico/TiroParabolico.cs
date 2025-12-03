using UnityEngine;

public class TiroParabolico : MonoBehaviour
{
    [Header("Parámetros del tiro")]
    public float anguloGrados = 45f;
    public float velocidadInicial = 10f;

    [Header("Control del tiempo")]
    [Range(0f, 1f)]
    public float tiempoSlider = 0f;

    [Header("Objeto hijo para rotación visual")]
    public Transform objetoVisual;

    private Vector3 posicionInicial;
    private float tiempoTotal;
    private float gravedad;

    void Start()
    {
        gravedad = Mathf.Abs(Physics.gravity.y);
        posicionInicial = transform.position;

        CalcularTiempoTotal();
    }

    void Update()
    {
        float t = tiempoSlider * tiempoTotal;

        float anguloRad = anguloGrados * Mathf.Deg2Rad;
        float v0x = velocidadInicial * Mathf.Cos(anguloRad);
        float v0y = velocidadInicial * Mathf.Sin(anguloRad);

        // Ecuaciones del tiro parabólico
        float x = v0x * t;
        float y = v0y * t - 0.5f * gravedad * t * t;

        Vector3 desplazamiento = new Vector3(x, y, 0);
        Vector3 nuevaPosicion = posicionInicial + desplazamiento.x * transform.forward + desplazamiento.y * Vector3.up;
        transform.position = nuevaPosicion;

        // Calcular la dirección de movimiento para orientar el objeto visual hijo
        float vy = v0y - gravedad * t;
        Vector3 velocidad = transform.forward * v0x + Vector3.up * vy;
        if (velocidad != Vector3.zero && objetoVisual != null)
        {
            objetoVisual.rotation = Quaternion.LookRotation(velocidad.normalized);
        }
    }

    private void CalcularTiempoTotal()
    {
        float anguloRad = anguloGrados * Mathf.Deg2Rad;
        float v0y = velocidadInicial * Mathf.Sin(anguloRad);
        tiempoTotal = (2 * v0y) / gravedad;
    }

    public void SetAngulo(float nuevoAngulo)
    {
        anguloGrados = nuevoAngulo;
        CalcularTiempoTotal();
    }

    public void SetVelocidad(float nuevaVelocidad)
    {
        velocidadInicial = nuevaVelocidad;
        CalcularTiempoTotal();
    }

    public void SetTiempoSlider(float nuevoTiempoSlider)
    {
        tiempoSlider = Mathf.Clamp01(nuevoTiempoSlider);
    }
}
