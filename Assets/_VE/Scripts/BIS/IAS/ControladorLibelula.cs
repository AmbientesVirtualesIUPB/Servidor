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

    [Tooltip("Tiempo entre elecciones de destinos aleatorios dentro del area (si no se usa la pausa, esto solo limita cambios).")]
    public Vector2 tiempoEntreDestinos = new Vector2(1.2f, 3.0f);

    [Header("Pausa al llegar a un punto (hover)")]
    [Tooltip("Tiempo que se queda quieta al llegar a un destino en el aire.")]
    public Vector2 tiempoPausaEnPunto = new Vector2(0.6f, 2.0f);

    [Tooltip("Que tan rapido frena para quedar quieta en la pausa.")]
    public float fuerzaFreno = 10f;

    [Header("Decision de posarse")]
    public Vector2 tiempoEntreIntentosPosado = new Vector2(3f, 7f);
    [Range(0f, 1f)] public float probabilidadIrAPosarse = 0.35f;

    [Header("Tiempo posada")]
    public Vector2 tiempoPosada = new Vector2(1.5f, 4.0f);

    [Header("Rotacion (opcional)")]
    public bool rotarHaciaMovimiento = true;
    public float velocidadRotacion = 10f;

    [Header("Efecto de flotacion (opcional)")]
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

    // Pausa en aire
    bool enPausaEnAire = false;
    float finPausaEnAire = 0f;

    Transform puntoPosadoActual;
    float semillaFlotacion;

    void Start()
    {
        semillaFlotacion = Random.Range(0f, 1000f);

        proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);
        proximaDecisionPosado = Time.time + Random.Range(tiempoEntreIntentosPosado.x, tiempoEntreIntentosPosado.y);

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

        switch (estadoActual)
        {
            case EstadoLibelula.Volando: SalirVolando(); break;
            case EstadoLibelula.Posada: SalirPosada(); break;
        }

        estadoActual = nuevoEstado;

        switch (estadoActual)
        {
            case EstadoLibelula.Volando: EntrarVolando(); break;
            case EstadoLibelula.Posada: EntrarPosada(); break;
        }
    }

    // ------------------ Estado: Volando ------------------
    void EntrarVolando()
    {
        puntoPosadoActual = null;

        // Reiniciar pausa
        enPausaEnAire = false;
        finPausaEnAire = 0f;

        // Asegurar destino
        destinoActual = PuntoAleatorioEnArea(puntoMinimoArea, puntoMaximoArea);
        proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);

        if (velocidadActual.sqrMagnitude < 0.01f)
            velocidadActual = Random.onUnitSphere * 0.1f;
    }

    void ActualizarVolando()
    {
        float dt = Time.deltaTime;

        // Si esta en pausa en aire, frena y espera
        if (enPausaEnAire)
        {
            velocidadActual = Vector3.Lerp(velocidadActual, Vector3.zero, 1f - Mathf.Exp(-fuerzaFreno * dt));
            transform.position += velocidadActual * dt;

            AplicarFlotacion(dt);

            if (Time.time >= finPausaEnAire)
            {
                enPausaEnAire = false;

                // Al terminar la pausa, elegir nuevo destino (o ir a posarse si ya habia decision)
                if (puntoPosadoActual != null)
                {
                    destinoActual = puntoPosadoActual.position;
                }
                else
                {
                    destinoActual = PuntoAleatorioEnArea(puntoMinimoArea, puntoMaximoArea);
                }

                proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);
            }

            return;
        }

        // Decision de posado (frecuencia configurable)
        if (Time.time >= proximaDecisionPosado)
        {
            proximaDecisionPosado = Time.time + Random.Range(tiempoEntreIntentosPosado.x, tiempoEntreIntentosPosado.y);

            if (puntosPosado != null && puntosPosado.Length > 0 && Random.value < probabilidadIrAPosarse)
            {
                Transform elegido = puntosPosado[Random.Range(0, puntosPosado.Length)];
                if (elegido != null)
                {
                    puntoPosadoActual = elegido;
                    destinoActual = puntoPosadoActual.position;
                }
            }
        }

        // Si NO va a posarse, puede cambiar destino por tiempo
        if (puntoPosadoActual == null && Time.time >= proximoCambioDestino)
        {
            destinoActual = PuntoAleatorioEnArea(puntoMinimoArea, puntoMaximoArea);
            proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);
        }

        // Movimiento hacia destino
        Vector3 dir = destinoActual - transform.position;
        float dist = dir.magnitude;

        if (dist <= distanciaLlegada)
        {
            // Al llegar: iniciar pausa real
            enPausaEnAire = true;
            finPausaEnAire = Time.time + Random.Range(tiempoPausaEnPunto.x, tiempoPausaEnPunto.y);

            // Si llego a un punto de posado, cambiar de estado a Posada
            if (puntoPosadoActual != null)
            {
                CambiarEstado(EstadoLibelula.Posada);
            }

            return;
        }

        Vector3 velDeseada = dir.normalized * velocidadVuelo;
        velocidadActual = Vector3.Lerp(velocidadActual, velDeseada, 1f - Mathf.Exp(-suavidadDireccion * dt));
        transform.position += velocidadActual * dt;

        AplicarFlotacion(dt);

        if (rotarHaciaMovimiento && velocidadActual.sqrMagnitude > 0.0005f)
        {
            Vector3 dir2 = velocidadActual.normalized;
            dir2.y = 0f; // ignorar inclinacion vertical

            if (dir2.sqrMagnitude > 0.0001f)
            {
                Quaternion rotObjetivo = Quaternion.LookRotation(dir2, Vector3.up);

                Quaternion rotSuave = Quaternion.Slerp(
                    transform.rotation,
                    rotObjetivo,
                    1f - Mathf.Exp(-velocidadRotacion * dt)
                );

                // Forzar X = 0 y Z = 0
                Vector3 euler = rotSuave.eulerAngles;
                euler.x = 0f;
                euler.z = 0f;

                transform.rotation = Quaternion.Euler(euler);
            }
        }

    }

    void SalirVolando()
    {
        // espacio para animaciones/sonidos
    }

    // ------------------ Estado: Posada ------------------
    void EntrarPosada()
    {
        if (puntoPosadoActual == null)
        {
            CambiarEstado(EstadoLibelula.Volando);
            return;
        }

        transform.position = puntoPosadoActual.position;
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

        transform.position = puntoPosadoActual.position;

        if (Time.time >= finPosada)
        {
            CambiarEstado(EstadoLibelula.Volando);
        }
    }

    void SalirPosada()
    {
        // Reprogramar timers
        proximoCambioDestino = Time.time + Random.Range(tiempoEntreDestinos.x, tiempoEntreDestinos.y);
        proximaDecisionPosado = Time.time + Random.Range(tiempoEntreIntentosPosado.x, tiempoEntreIntentosPosado.y);

        puntoPosadoActual = null;

        // Al salir de posada, puedes forzar una pausa breve si quieres:
        // enPausaEnAire = true; finPausaEnAire = Time.time + 0.3f;
    }

    // ------------------ Utilidades ------------------
    void AplicarFlotacion(float dt)
    {
        float flotacion = Mathf.Sin((Time.time + semillaFlotacion) * frecuenciaFlotacion) * amplitudFlotacion;
        transform.position += new Vector3(0f, flotacion * dt, 0f);
    }

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

        Vector3 centro = (puntoMinimoArea + puntoMaximoArea) * 0.5f;
        Vector3 tamano = new Vector3(
            Mathf.Abs(puntoMaximoArea.x - puntoMinimoArea.x),
            Mathf.Abs(puntoMaximoArea.y - puntoMinimoArea.y),
            Mathf.Abs(puntoMaximoArea.z - puntoMinimoArea.z)
        );

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(centro, tamano);

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
    