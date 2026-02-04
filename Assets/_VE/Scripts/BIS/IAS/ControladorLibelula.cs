using UnityEngine;

public class ControladorLibelula : MonoBehaviour
{
    public enum EstadoLibelula
    {
        Volando,
        Posada
    }

    [Header("Area de vuelo (cubo entre dos puntos)")]
    public Vector3 puntoMinimoArea = new Vector3(-5f, 1f, -5f);
    public Vector3 puntoMaximoArea = new Vector3(5f, 4f, 5f);

    [Header("Puntos de posado (asignar en inspector)")]
    public Transform[] puntosPosado;

    [Header("Movimiento en el aire")]
    public float velocidadVuelo = 3.5f;
    public float suavidadDireccion = 6f;
    public float distanciaLlegada = 0.35f;

    [Tooltip("Tiempo entre elecciones de destinos aleatorios dentro del area.")]
    public Vector2 tiempoEntreDestinos = new Vector2(1.2f, 3.0f);

    [Header("Decision de posarse")]
    [Tooltip("Cada cuanto tiempo intenta decidir ir a posarse (rango aleatorio).")]
    public Vector2 tiempoEntreIntentosPosado = new Vector2(3f, 7f);

    [Tooltip("Probabilidad de ir a posarse cuando llega el momento de decidir.")]
    [Range(0f, 1f)] public float probabilidadIrAPosarse = 0.35f;

    [Header("Tiempo posada")]
    public Vector2 tiempoPosada = new Vector2(1.5f, 4.0f);

    [Header("Rotacion (opcional)")]
    public bool rotarHaciaMovimiento = true;
    public float velocidadRotacion = 10f;

    [Header("Efecto de aleteo / flotacion (opcional)")]
    public float amplitudFlotacion = 0.12f;
    public float frecuenciaFlotacion = 2.2f;

    [Header("Debug")]
    public bool dibujarGizmos = true;

    // ------------------ Estado interno ------------------
    public EstadoLibelula estadoActual = EstadoLibelula.Volando;

    Vector3 destinoActual;
    Vector3 velocidadActual;

    float proximoCambioDestino;
    float proximaDecisionPosado;
    float finPosada;

    Transform puntoPosadoActual;
    Vector3 posicionLocalPosado; // para mantener la posicion exacta al posarse
    float semillaFlotacion;

    void Start()
    {
        semillaFlotacion = Random.Range(0f, 1000f);

        // Inicializar timers
        proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);
        proximaDecisionPosado = Time.time + Random.Range(tiempoEntreIntentosPosado.x, tiempoEntreIntentosPosado.y);

        // Primer destino de vuelo
        destinoActual = PuntoAleatorioEnArea(puntoMinimoArea, puntoMaximoArea);

        CambiarEstado(EstadoLibelula.Volando);
    }

    void Update()
    {
        switch (estadoActual)
        {
            case EstadoLibelula.Volando:
                ActualizarVolando();
                break;

            case EstadoLibelula.Posada:
                ActualizarPosada();
                break;
        }
    }

    // ------------------ Maquina de estados ------------------
    void CambiarEstado(EstadoLibelula nuevoEstado)
    {
        if (nuevoEstado == estadoActual) return;

        // Salir
        switch (estadoActual)
        {
            case EstadoLibelula.Volando:
                SalirVolando();
                break;
            case EstadoLibelula.Posada:
                SalirPosada();
                break;
        }

        estadoActual = nuevoEstado;

        // Entrar
        switch (estadoActual)
        {
            case EstadoLibelula.Volando:
                EntrarVolando();
                break;
            case EstadoLibelula.Posada:
                EntrarPosada();
                break;
        }
    }

    // ------------------ Estado: Volando ------------------
    void EntrarVolando()
    {
        puntoPosadoActual = null;

        // Asegurar un destino valido
        destinoActual = PuntoAleatorioEnArea(puntoMinimoArea, puntoMaximoArea);
        proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);

        // Si vienes de posada, dale un pequeño impulso
        if (velocidadActual.sqrMagnitude < 0.01f)
            velocidadActual = Random.onUnitSphere * 0.1f;
    }

    void ActualizarVolando()
    {
        float dt = Time.deltaTime;

        // Cambiar destino dentro del area cada cierto tiempo o cuando llega
        float dist = Vector3.Distance(transform.position, destinoActual);

        if (Time.time >= proximoCambioDestino || dist <= distanciaLlegada)
        {
            destinoActual = PuntoAleatorioEnArea(puntoMinimoArea, puntoMaximoArea);
            proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);
        }

        // Decision de posado (frecuencia configurable)
        if (Time.time >= proximaDecisionPosado)
        {
            proximaDecisionPosado = Time.time + Random.Range(tiempoEntreIntentosPosado.x, tiempoEntreIntentosPosado.y);

            if (puntosPosado != null && puntosPosado.Length > 0 && Random.value < probabilidadIrAPosarse)
            {
                puntoPosadoActual = puntosPosado[Random.Range(0, puntosPosado.Length)];
                if (puntoPosadoActual != null)
                {
                    // Destino pasa a ser el punto de posado
                    destinoActual = puntoPosadoActual.position;
                }
            }
        }

        // Movimiento suave hacia destino
        Vector3 dir = (destinoActual - transform.position);
        Vector3 velDeseada = dir.normalized * velocidadVuelo;

        velocidadActual = Vector3.Lerp(velocidadActual, velDeseada, 1f - Mathf.Exp(-suavidadDireccion * dt));
        transform.position += velocidadActual * dt;

        // Flotacion vertical ligera
        float flotacion = Mathf.Sin((Time.time + semillaFlotacion) * frecuenciaFlotacion) * amplitudFlotacion;
        transform.position += new Vector3(0f, flotacion * dt, 0f);

        // Rotacion hacia movimiento
        if (rotarHaciaMovimiento && velocidadActual.sqrMagnitude > 0.0005f)
        {
            Quaternion rotObjetivo = Quaternion.LookRotation(velocidadActual.normalized, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotObjetivo, 1f - Mathf.Exp(-velocidadRotacion * dt));
        }

        // Si iba a posarse y ya llego suficientemente cerca, cambia de estado
        if (puntoPosadoActual != null)
        {
            if (Vector3.Distance(transform.position, puntoPosadoActual.position) <= Mathf.Max(distanciaLlegada, 0.25f))
            {
                CambiarEstado(EstadoLibelula.Posada);
            }
        }
    }

    void SalirVolando()
    {
        // espacio para animaciones/sonidos si luego lo necesitas
    }

    // ------------------ Estado: Posada ------------------
    void EntrarPosada()
    {
        // Si por algun motivo no hay punto, vuelve a volar
        if (puntoPosadoActual == null)
        {
            CambiarEstado(EstadoLibelula.Volando);
            return;
        }

        // Congelar en el punto exacto (o con un offset pequeñito si quieres)
        posicionLocalPosado = puntoPosadoActual.position;
        transform.position = posicionLocalPosado;

        // Mantener una rotacion agradable (mirando al frente del punto si lo deseas)
        // transform.rotation = puntoPosadoActual.rotation;

        velocidadActual = Vector3.zero;

        finPosada = Time.time + Random.Range(tiempoPosada.x, tiempoPosada.y);
    }

    void ActualizarPosada()
    {
        if (puntoPosadoActual == null)
        {
            CambiarEstado(EstadoLibelula.Volando);
            return;
        }

        // Mantenerse pegada al punto aunque el punto se mueva (opcional)
        transform.position = puntoPosadoActual.position;

        if (Time.time >= finPosada)
        {
            CambiarEstado(EstadoLibelula.Volando);
        }
    }

    void SalirPosada()
    {
        // Al salir, programa proximas decisiones
        proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);
        proximaDecisionPosado = Time.time + Random.Range(tiempoEntreIntentosPosado.x, tiempoEntreIntentosPosado.y);

        // Al despegar, quita referencia para que no reingrese inmediatamente
        puntoPosadoActual = null;
    }

    // ------------------ Utilidades ------------------
    Vector3 PuntoAleatorioEnArea(Vector3 a, Vector3 b)
    {
        float minX = Mathf.Min(a.x, b.x);
        float maxX = Mathf.Max(a.x, b.x);
        float minY = Mathf.Min(a.y, b.y);
        float maxY = Mathf.Max(a.y, b.y);
        float minZ = Mathf.Min(a.z, b.z);
        float maxZ = Mathf.Max(a.z, b.z);

        return new Vector3(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY),
            Random.Range(minZ, maxZ)
        );
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!dibujarGizmos) return;

        // Dibujar cubo del area
        Vector3 centro = (puntoMinimoArea + puntoMaximoArea) * 0.5f;
        Vector3 tamano = new Vector3(
            Mathf.Abs(puntoMaximoArea.x - puntoMinimoArea.x),
            Mathf.Abs(puntoMaximoArea.y - puntoMinimoArea.y),
            Mathf.Abs(puntoMaximoArea.z - puntoMinimoArea.z)
        );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(centro, tamano);

        // Puntos de posado
        if (puntosPosado != null)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < puntosPosado.Length; i++)
            {
                if (puntosPosado[i] != null)
                    Gizmos.DrawSphere(puntosPosado[i].position, 0.12f);
            }
        }
    }
#endif
}
