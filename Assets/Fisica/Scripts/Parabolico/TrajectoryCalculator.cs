using UnityEngine;

public class TrajectoryCalculator : MonoBehaviour
{
    [Header("Referencias")]
    public LineRenderer lineRenderer;
    public Transform mira;

    [Header("Configuración")]
    public Vector2 velocidadInicial2D;
    public float velInicialT = 0;
    public int segmentos = 50;
    public float gravedad = 9.81f;

    [Header("Información de Ángulo")]
    [SerializeField] private float anguloActual;

    // Variable para mostrar el ángulo público (solo lectura)
    public float AnguloEnGrados => anguloActual;

    private void Start()
    {
        // Configurar LineRenderer si no está configurado
        if (lineRenderer == null)
            lineRenderer = GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.useWorldSpace = true;
            lineRenderer.positionCount = segmentos + 1;
        }
    }

    public float velocidadInicial()
    {
        return Mathf.Lerp(velocidadInicial2D.x, velocidadInicial2D.y, velInicialT);
    }

    private void Update()
    {
        if (mira != null && lineRenderer != null)
        {
            CalcularYDibujarParabola();
        }
    }

    private void CalcularYDibujarParabola()
    {
        // Obtener la dirección de la mira
        Vector3 direccionMira = mira.forward.normalized;

        // Calcular el ángulo en Y (elevación) basándose en la dirección de la mira
        anguloActual = Mathf.Round(Mathf.Asin(direccionMira.y) * Mathf.Rad2Deg * 10f) / 10f;

        // Obtener la posición inicial
        Vector3 posicionInicial = mira.position;

        // Calcular la velocidad inicial usando la dirección completa de la mira
        Vector3 velocidadInicial3D = direccionMira * velocidadInicial();

        // Calcular el tiempo de vuelo (cuando Y llega a 0)
        float tiempoVuelo = CalcularTiempoVuelo(posicionInicial.y, velocidadInicial3D.y);

        // Configurar el número de puntos del LineRenderer
        lineRenderer.positionCount = segmentos + 1;

        // Calcular y asignar puntos de la parábola
        for (int i = 0; i <= segmentos; i++)
        {
            float t = (float)i / segmentos * tiempoVuelo;
            Vector3 punto = CalcularPosicionEnTiempo(posicionInicial, velocidadInicial3D, t);
            lineRenderer.SetPosition(i, punto);
        }
    }

    private float CalcularTiempoVuelo(float alturaInicial, float velocidadY)
    {
        // Usar la fórmula cuadrática para encontrar cuando y = 0
        // y = y0 + vy*t - 0.5*g*t²
        // 0 = y0 + vy*t - 0.5*g*t²
        // 0.5*g*t² - vy*t - y0 = 0

        float g = gravedad;
        float a = 0.5f * g;
        float b = -velocidadY;
        float c = -alturaInicial;

        // Fórmula cuadrática: t = (-b ± √(b²-4ac)) / 2a
        float discriminante = b * b - 4 * a * c;

        if (discriminante < 0)
            return 0f; // No hay solución real

        float t1 = (-b + Mathf.Sqrt(discriminante)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminante)) / (2 * a);

        // Tomar el tiempo positivo mayor (el tiempo de vuelo)
        return Mathf.Max(t1, t2);
    }

    private Vector3 CalcularPosicionEnTiempo(Vector3 posInicial, Vector3 velocidadInicial, float tiempo)
    {
        float x = posInicial.x + velocidadInicial.x * tiempo;
        float y = posInicial.y + velocidadInicial.y * tiempo - 0.5f * gravedad * tiempo * tiempo;
        float z = posInicial.z + velocidadInicial.z * tiempo;

        return new Vector3(x, y, z);
    }

    // Método para obtener información de la trayectoria (opcional)
    public Vector3 ObtenerPuntoImpacto()
    {
        if (mira == null) return Vector3.zero;

        Vector3 posicionInicial = mira.position;
        Vector3 direccionMira = mira.forward.normalized;
        Vector3 velocidadInicial3D = direccionMira * velocidadInicial();

        float tiempoVuelo = CalcularTiempoVuelo(posicionInicial.y, velocidadInicial3D.y);

        return CalcularPosicionEnTiempo(posicionInicial, velocidadInicial3D, tiempoVuelo);
    }

    // Validación en el inspector
    private void OnValidate()
    {
        if (segmentos < 10)
            segmentos = 10;
        if (velocidadInicial() < 0.1f)
            velocidadInicial2D.x = 0.1f;
        if (gravedad < 0.1f)
            gravedad = 0.1f;
    }
}