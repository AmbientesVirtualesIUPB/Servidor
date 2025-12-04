using UnityEngine;
using System.Collections;

public class TrajectoryAnimator : MonoBehaviour
{
    [Header("Referencias")]
    public ProjectileFollower projectileFollower;
    public Animator animator;

    [Header("Configuración de Tiempos")]
    public float tiempoEspera = 0.5f;

    [Header("Estado de la Animación")]
    [SerializeField] private bool animandoEspera = false;
    [SerializeField] private bool animandoTrayectoria = false;
    [SerializeField] private float tiempoRestanteEspera = 0f;
    [SerializeField] private float tiempoRestanteTrayectoria = 0f;
    [SerializeField] private float duracionTotalTrayectoria = 0f;
    [SerializeField] public float tensionInicial = 0f;
    [SerializeField] public float tensionActual = 0f;
    public GameObject flechaTension, flechaSimulacion;

    private Coroutine corutinaEspera;
    private Coroutine corutinaTrayectoria;

    [ContextMenu("Iniciar Animación")]
    public void IniciarAnimacion()
    {
        // Detener cualquier animación en curso
        DetenerAnimacion();

        // Verificar que tenemos las referencias necesarias
        if (projectileFollower == null)
        {
            Debug.LogError("ProjectileFollower no asignado!");
            return;
        }

        if (projectileFollower.trajectoryCalculator == null)
        {
            Debug.LogError("TrajectoryCalculator no asignado en ProjectileFollower!");
            return;
        }

        if (animator == null)
        {
            Debug.LogError("Animator no asignado!");
            return;
        }

        // Resetear progreso al inicio
        projectileFollower.EstablecerProgreso(0f);

        // Obtener el valor inicial de Tens del Animator
        tensionInicial = animator.GetFloat("Tens");
        tensionActual = tensionInicial;

        Debug.Log($"Valor inicial de Tens: {tensionInicial}");

        // Iniciar la secuencia de animación
        corutinaEspera = StartCoroutine(CorutinaEspera());
    }

    [ContextMenu("Detener Animación")]
    public void DetenerAnimacion()
    {
        // Detener ambas corrutinas si están ejecutándose
        if (corutinaEspera != null)
        {
            StopCoroutine(corutinaEspera);
            corutinaEspera = null;
        }

        if (corutinaTrayectoria != null)
        {
            StopCoroutine(corutinaTrayectoria);
            corutinaTrayectoria = null;
        }

        // Resetear estados
        animandoEspera = false;
        animandoTrayectoria = false;
        tiempoRestanteEspera = 0f;
        tiempoRestanteTrayectoria = 0f;
    }

    [ContextMenu("Resetear Progreso")]
    public void ResetearProgreso()
    {
        if (projectileFollower != null)
        {
            projectileFollower.EstablecerProgreso(0f);
        }
        DetenerAnimacion();
        flechaSimulacion.SetActive(false);
        flechaTension.SetActive(true);
        animator.SetFloat("Tens", tensionInicial);
    }

    private IEnumerator CorutinaEspera()
    {
        animandoEspera = true;
        tiempoRestanteEspera = tiempoEspera;

        Debug.Log($"Iniciando espera de {tiempoEspera} segundos...");

        // Esperar el tiempo especificado
        float tiempoInicio = Time.time;
        flechaSimulacion.SetActive(false);
        flechaTension.SetActive(true);
        while (Time.time - tiempoInicio < tiempoEspera)
        {
            tiempoRestanteEspera = tiempoEspera - (Time.time - tiempoInicio);
            animator.SetFloat("Tens", Mathf.Lerp(tensionInicial, 0, (Time.time - tiempoInicio) / tiempoEspera));
            yield return null;
        }

        animandoEspera = false;
        tiempoRestanteEspera = 0f;

        flechaSimulacion.SetActive(true);
        flechaTension.SetActive(false);
        Debug.Log("Espera completada. Iniciando animación de trayectoria...");

        // Iniciar la segunda corrutina
        corutinaTrayectoria = StartCoroutine(CorutinaTrayectoria());
    }

    private IEnumerator CorutinaTrayectoria()
    {
        animandoTrayectoria = true;

        // Obtener el tiempo de vuelo total del ProjectileFollower
        duracionTotalTrayectoria = ObtenerTiempoVueloTotal();

        if (duracionTotalTrayectoria <= 0f)
        {
            Debug.LogWarning("No se pudo obtener el tiempo de vuelo total. Usando duración por defecto de 2 segundos.");
            duracionTotalTrayectoria = 2f;
        }

        Debug.Log($"Animando trayectoria durante {duracionTotalTrayectoria} segundos...");

        tiempoRestanteTrayectoria = duracionTotalTrayectoria;
        float tiempoInicio = Time.time;

        // Animar el progreso de 0 a 1
        while (Time.time - tiempoInicio < duracionTotalTrayectoria)
        {
            float tiempoTranscurrido = Time.time - tiempoInicio;
            float progreso = tiempoTranscurrido / duracionTotalTrayectoria;

            // Asegurar que el progreso esté entre 0 y 1
            progreso = Mathf.Clamp01(progreso);

            // Enviar el progreso al ProjectileFollower
            projectileFollower.EstablecerProgreso(progreso);

            // Actualizar tiempo restante para debug
            tiempoRestanteTrayectoria = duracionTotalTrayectoria - tiempoTranscurrido;

            yield return null;
        }

        // Asegurar que termine exactamente en 1
        projectileFollower.EstablecerProgreso(1f);

        animandoTrayectoria = false;
        tiempoRestanteTrayectoria = 0f;

        Debug.Log("Animación de trayectoria completada!");
    }

    private float ObtenerTiempoVueloTotal()
    {
        // Acceder al campo privado tiempoVueloTotal del ProjectileFollower
        // Como es un campo privado, necesitamos usar reflection o hacer que sea público
        // Para simplicidad, calcularemos el tiempo de vuelo aquí también

        if (projectileFollower.trajectoryCalculator == null ||
            projectileFollower.trajectoryCalculator.mira == null)
        {
            return 0f;
        }

        var trajectory = projectileFollower.trajectoryCalculator;
        Vector3 posicionInicial = trajectory.mira.position;
        Vector3 direccionMira = trajectory.mira.forward.normalized;
        Vector3 velocidadInicial = direccionMira * trajectory.velocidadInicial();

        return CalcularTiempoVuelo(posicionInicial.y, velocidadInicial.y, trajectory.gravedad);
    }

    private float CalcularTiempoVuelo(float alturaInicial, float velocidadY, float gravedad)
    {
        // Usar la fórmula cuadrática para encontrar cuando y = 0
        float a = 0.5f * gravedad;
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

    // Métodos públicos para control externo
    public bool EstaAnimando()
    {
        return animandoEspera || animandoTrayectoria;
    }

    public bool EstaEnEspera()
    {
        return animandoEspera;
    }

    public bool EstaAnimandoTrayectoria()
    {
        return animandoTrayectoria;
    }

    public float ObtenerTiempoRestanteEspera()
    {
        return tiempoRestanteEspera;
    }

    public float ObtenerTiempoRestanteTrayectoria()
    {
        return tiempoRestanteTrayectoria;
    }

    // Validación en el inspector
    private void OnValidate()
    {
        if (tiempoEspera < 0f)
            tiempoEspera = 0f;
    }

    // Información de debug en el inspector
    private void Update()
    {
        // Este método se ejecuta para mantener actualizada la información de debug
        // No es necesario hacer nada aquí ya que las corrutinas actualizan los valores
    }
}