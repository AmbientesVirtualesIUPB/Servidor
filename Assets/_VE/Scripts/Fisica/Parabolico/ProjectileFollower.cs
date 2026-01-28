using UnityEngine;

public class ProjectileFollower : MonoBehaviour
{
    [Header("Referencias")]
    public TrajectoryCalculator trajectoryCalculator;
    public Transform objetoAMover;

    [Header("Configuración")]
    [Range(0f, 1f)]
    public float progreso = 0f;

    [Header("Información de Debug")]
    [SerializeField] private Vector3 posicionActual;
    [SerializeField] private Vector3 direccionTangente;
    [SerializeField] private float tiempoVueloTotal;

    private void Start()
    {
        // Si no se asigna objeto a mover, usar este mismo transform
        if (objetoAMover == null)
            objetoAMover = transform;
    }

    private void Update()
    {
        if (trajectoryCalculator != null && objetoAMover != null && trajectoryCalculator.mira != null)
        {
            ActualizarPosicionYRotacion();
        }
    }

    private void ActualizarPosicionYRotacion()
    {
        // Obtener datos de la trayectoria
        Vector3 posicionInicial = trajectoryCalculator.mira.position;
        Vector3 direccionMira = trajectoryCalculator.mira.forward.normalized;
        Vector3 velocidadInicial = direccionMira * trajectoryCalculator.velocidadInicial();

        // Calcular tiempo de vuelo total
        tiempoVueloTotal = CalcularTiempoVuelo(posicionInicial.y, velocidadInicial.y);

        // Calcular tiempo actual basado en el progreso
        float tiempoActual = progreso * tiempoVueloTotal;

        // Calcular posición actual
        posicionActual = CalcularPosicionEnTiempo(posicionInicial, velocidadInicial, tiempoActual);

        // Calcular dirección tangente (velocidad en este momento)
        direccionTangente = CalcularVelocidadEnTiempo(velocidadInicial, tiempoActual).normalized;

        // Aplicar posición y rotación al objeto
        objetoAMover.position = posicionActual;

        // Rotar el objeto para que apunte en la dirección de la tangente
        if (direccionTangente != Vector3.zero)
        {
            objetoAMover.rotation = Quaternion.LookRotation(direccionTangente);
        }
    }

    private Vector3 CalcularPosicionEnTiempo(Vector3 posInicial, Vector3 velocidadInicial, float tiempo)
    {
        float x = posInicial.x + velocidadInicial.x * tiempo;
        float y = posInicial.y + velocidadInicial.y * tiempo - 0.5f * trajectoryCalculator.gravedad * tiempo * tiempo;
        float z = posInicial.z + velocidadInicial.z * tiempo;

        return new Vector3(x, y, z);
    }

    private Vector3 CalcularVelocidadEnTiempo(Vector3 velocidadInicial, float tiempo)
    {
        // La velocidad en cualquier momento es:
        // vx = vx0 (constante)
        // vy = vy0 - g*t
        // vz = vz0 (constante)

        float vx = velocidadInicial.x;
        float vy = velocidadInicial.y - trajectoryCalculator.gravedad * tiempo;
        float vz = velocidadInicial.z;

        return new Vector3(vx, vy, vz);
    }

    private float CalcularTiempoVuelo(float alturaInicial, float velocidadY)
    {
        // Usar la fórmula cuadrática para encontrar cuando y = 0
        float g = trajectoryCalculator.gravedad;
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

    // Métodos públicos para controlar el progreso desde otros scripts
    public void EstablecerProgreso(float nuevoProgreso)
    {
        progreso = Mathf.Clamp01(nuevoProgreso);
    }

    public void IniciarDesdeInicio()
    {
        progreso = 0f;
    }

    public void IrAlFinal()
    {
        progreso = 1f;
    }

    // Método para animar el progreso automáticamente
    public void AnimarTrayectoria(float duracion)
    {
        StartCoroutine(AnimarProgresoCoroutine(duracion));
    }

    private System.Collections.IEnumerator AnimarProgresoCoroutine(float duracion)
    {
        float tiempoInicio = Time.time;
        float progresoInicial = progreso;

        while (Time.time - tiempoInicio < duracion)
        {
            float tiempoTranscurrido = Time.time - tiempoInicio;
            progreso = Mathf.Lerp(progresoInicial, 1f, tiempoTranscurrido / duracion);
            yield return null;
        }

        progreso = 1f;
    }

    // Obtener información actual del proyectil
    public Vector3 ObtenerPosicionActual()
    {
        return posicionActual;
    }

    public Vector3 ObtenerDireccionActual()
    {
        return direccionTangente;
    }

    public float ObtenerVelocidadActual()
    {
        if (trajectoryCalculator != null)
        {
            Vector3 direccionMira = trajectoryCalculator.mira.forward.normalized;
            Vector3 velocidadInicial = direccionMira * trajectoryCalculator.velocidadInicial();
            float tiempoActual = progreso * tiempoVueloTotal;

            return CalcularVelocidadEnTiempo(velocidadInicial, tiempoActual).magnitude;
        }
        return 0f;
    }

    // Validación en el inspector
    private void OnValidate()
    {
        progreso = Mathf.Clamp01(progreso);
    }

    // Dibujar información de debug en la escena
    private void OnDrawGizmos()
    {
        if (trajectoryCalculator != null && Application.isPlaying)
        {
            // Dibujar posición actual
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(posicionActual, 0.1f);

            // Dibujar dirección tangente
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(posicionActual, direccionTangente * 2f);
        }
    }
}