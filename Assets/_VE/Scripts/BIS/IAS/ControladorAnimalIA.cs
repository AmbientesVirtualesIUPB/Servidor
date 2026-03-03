using UnityEngine;
using UnityEngine.AI;

public class ControladorAnimalIA : MonoBehaviour
{
    public enum EstadoAnimal
    {
        Quieto = 0,
        Caminar = 1,
        Alerta = 3,
        Huir = 2
    }

    [Header("Referencias")]
    public NavMeshAgent agente;
    public string tagJugador = "Player";
    public Animator animator;

    [Header("Puntos de interes (asignar en inspector)")]
    public Transform[] puntosInteres;

    [Header("Tiempos")]
    [Tooltip("Tiempo quieto antes de decidir moverse (rango aleatorio).")]
    public Vector2 tiempoQuieto = new Vector2(2f, 6f);

    [Tooltip("Cada cuanto reevalua mientras esta en alerta (para no spamear logica).")]
    public float intervaloChequeoAlerta = 0.2f;

    [Header("Distancias (configurable por animal)")]
    [Tooltip("Si el jugador entra en este radio, pasa a alerta.")]
    public float distanciaAlerta = 12f;

    [Tooltip("Si el jugador entra en este radio, pasa a huir.")]
    public float distanciaHuir = 7f;

    [Tooltip("Distancia minima a la que quiere quedar del jugador al huir.")]
    public float distanciaMinimaHuida = 18f;

    [Tooltip("Maximo de intentos para encontrar punto de huida valido.")]
    public int intentosHuida = 10;

    [Header("Velocidades (configurable por animal)")]
    public float velocidadCaminar = 2.0f;
    public float velocidadHuir = 5.5f;

    [Header("Llegada")]
    public float distanciaLlegada = 0.6f;

    [Header("Debug")]
    public bool dibujarGizmos = true;

    // -------------------- Estado interno --------------------
    public EstadoAnimal estadoActual = EstadoAnimal.Quieto;

    Transform jugador;
    int indiceObjetivo = -1;
    float tiempoProximaDecision = 0f;
    float proximoChequeoAlerta = 0f;

    // -------------------- Unity --------------------
    void Reset()
    {
        agente = GetComponent<NavMeshAgent>();
    }

    void Awake()
    {
        if (agente == null) agente = GetComponent<NavMeshAgent>();
        BuscarJugador();
    }

    void Start()
    {
        if (puntosInteres == null || puntosInteres.Length == 0)
            Debug.LogWarning("ControladorAnimalIA: No hay puntosInteres asignados. El animal solo reaccionara al jugador.");

        CambiarEstado(EstadoAnimal.Quieto);
    }

    void Update()
    {
        if (jugador == null) BuscarJugador();

        // Chequeo de distancias al jugador (si existe)
        if (jugador != null)
        {
            float d = Vector3.Distance(transform.position, jugador.position);

            // Prioridad: huir > alerta > resto
            if (d <= distanciaHuir && estadoActual != EstadoAnimal.Huir)
            {
                CambiarEstado(EstadoAnimal.Huir);
            }
            else if (d <= distanciaAlerta && estadoActual != EstadoAnimal.Huir && estadoActual != EstadoAnimal.Alerta)
            {
                CambiarEstado(EstadoAnimal.Alerta);
            }
            else if (d > distanciaAlerta && estadoActual == EstadoAnimal.Alerta)
            {
                // Si ya no esta cerca, vuelve a su rutina
                CambiarEstado(EstadoAnimal.Quieto);
            }
        }

        // Actualizar estado
        switch (estadoActual)
        {
            case EstadoAnimal.Quieto: ActualizarQuieto(); break;
            case EstadoAnimal.Caminar: ActualizarCaminar(); break;
            case EstadoAnimal.Alerta: ActualizarAlerta(); break;
            case EstadoAnimal.Huir: ActualizarHuir(); break;
        }
    }

    // -------------------- Maquina de estados --------------------
    void CambiarEstado(EstadoAnimal nuevoEstado)
    {
        if (nuevoEstado == estadoActual) return;

        if (animator != null) animator.SetInteger("estado", ((int)nuevoEstado) % 3);

        // Salir estado actual
        switch (estadoActual)
        {
            case EstadoAnimal.Quieto: SalirQuieto(); break;
            case EstadoAnimal.Caminar: SalirCaminar(); break;
            case EstadoAnimal.Alerta: SalirAlerta(); break;
            case EstadoAnimal.Huir: SalirHuir(); break;
        }

        estadoActual = nuevoEstado;

        // Entrar nuevo estado
        switch (estadoActual)
        {
            case EstadoAnimal.Quieto: EntrarQuieto(); break;
            case EstadoAnimal.Caminar: EntrarCaminar(); break;
            case EstadoAnimal.Alerta: EntrarAlerta(); break;
            case EstadoAnimal.Huir: EntrarHuir(); break;
        }
    }

    // -------------------- Estado: Quieto --------------------
    void EntrarQuieto()
    {
        if (agente != null)
        {
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
        }

        tiempoProximaDecision = Time.time + Random.Range(tiempoQuieto.x, tiempoQuieto.y);
        // Aqui luego puedes activar animacion "Idle"
    }

    void ActualizarQuieto()
    {
        if (Time.time < tiempoProximaDecision) return;

        // Decide caminar a otro punto
        if (puntosInteres != null && puntosInteres.Length > 0)
        {
            SeleccionarNuevoObjetivo();
            CambiarEstado(EstadoAnimal.Caminar);
        }
        else
        {
            // Si no hay puntos, solo vuelve a esperar
            tiempoProximaDecision = Time.time + Random.Range(tiempoQuieto.x, tiempoQuieto.y);
        }
    }

    void SalirQuieto()
    {
        // Aqui luego puedes apagar animacion Idle si quieres
    }

    // -------------------- Estado: Caminar --------------------
    void EntrarCaminar()
    {
        if (agente == null) return;

        agente.isStopped = false;
        agente.speed = velocidadCaminar;

        Vector3 destino = ObtenerDestinoObjetivoActual();
        agente.SetDestination(destino);

        // Aqui luego puedes activar animacion "Walk"
    }

    void ActualizarCaminar()
    {
        if (agente == null) return;

        if (!agente.pathPending && agente.remainingDistance <= Mathf.Max(distanciaLlegada, agente.stoppingDistance))
        {
            CambiarEstado(EstadoAnimal.Quieto);
        }
    }

    void SalirCaminar()
    {
        // Aqui luego puedes apagar animacion Walk
    }

    // -------------------- Estado: Alerta --------------------
    void EntrarAlerta()
    {
        if (agente != null)
        {
            // Se queda quieto pero atento (puedes decidir que camine lento, etc.)
            agente.isStopped = true;
            agente.velocity = Vector3.zero;
        }

        proximoChequeoAlerta = Time.time;
        // Aqui luego puedes activar animacion "Alert" o "Look"
    }

    void ActualizarAlerta()
    {
        if (jugador == null) return;
        if (Time.time < proximoChequeoAlerta) return;

        proximoChequeoAlerta = Time.time + intervaloChequeoAlerta;

        // Mirar al jugador (solo giro en Y)
        Vector3 dir = jugador.position - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            Quaternion rot = Quaternion.LookRotation(dir.normalized);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, 1f - Mathf.Exp(-8f * Time.deltaTime));
        }

        // Las transiciones por distancia ya las maneja Update() arriba
    }

    void SalirAlerta()
    {
        // Aqui luego puedes apagar animacion Alert
    }

    // -------------------- Estado: Huir --------------------
    void EntrarHuir()
    {
        if (agente == null) return;

        agente.isStopped = false;
        agente.speed = velocidadHuir;

        Vector3 destino = CalcularDestinoHuida();
        agente.SetDestination(destino);

        // Aqui luego puedes activar animacion "Run"
    }

    void ActualizarHuir()
    {
        if (agente == null) return;

        // Si llega al destino de huida, vuelve a quieto (o caminar) segun prefieras
        if (!agente.pathPending && agente.remainingDistance <= Mathf.Max(distanciaLlegada, agente.stoppingDistance))
        {
            CambiarEstado(EstadoAnimal.Quieto);
            return;
        }

        // Si el jugador sigue demasiado cerca, replantea destino (opcional)
        if (jugador != null)
        {
            float d = Vector3.Distance(transform.position, jugador.position);
            if (d <= distanciaHuir * 0.9f)
            {
                Vector3 nuevoDestino = CalcularDestinoHuida();
                agente.SetDestination(nuevoDestino);
            }
        }
    }

    void SalirHuir()
    {
        // Aqui luego puedes apagar animacion Run
    }

    // -------------------- Utilidades --------------------
    void BuscarJugador()
    {
        GameObject go = GameObject.FindGameObjectWithTag(tagJugador);
        jugador = go != null ? go.transform : null;
    }

    void SeleccionarNuevoObjetivo()
    {
        if (puntosInteres == null || puntosInteres.Length == 0) return;

        int nuevo = indiceObjetivo;

        // Evitar repetir el mismo punto si hay mas de 1
        if (puntosInteres.Length > 1)
        {
            int seguridad = 0;
            while (nuevo == indiceObjetivo && seguridad++ < 10)
                nuevo = Random.Range(0, puntosInteres.Length);
        }
        else
        {
            nuevo = 0;
        }

        indiceObjetivo = nuevo;
    }

    Vector3 ObtenerDestinoObjetivoActual()
    {
        if (puntosInteres == null || puntosInteres.Length == 0) return transform.position;

        if (indiceObjetivo < 0 || indiceObjetivo >= puntosInteres.Length)
            indiceObjetivo = Random.Range(0, puntosInteres.Length);

        Transform t = puntosInteres[indiceObjetivo];
        return t != null ? t.position : transform.position;
    }

    Vector3 CalcularDestinoHuida()
    {
        // Si no hay jugador, huye a un punto aleatorio de interes (o se queda)
        if (jugador == null)
        {
            if (puntosInteres != null && puntosInteres.Length > 0)
            {
                SeleccionarNuevoObjetivo();
                return ObtenerDestinoObjetivoActual();
            }
            return transform.position;
        }

        Vector3 pos = transform.position;
        Vector3 dirFuera = (pos - jugador.position);
        dirFuera.y = 0f;

        if (dirFuera.sqrMagnitude < 0.001f)
            dirFuera = transform.forward;

        dirFuera.Normalize();

        // Intentar encontrar un punto en NavMesh suficientemente alejado del jugador
        for (int i = 0; i < Mathf.Max(1, intentosHuida); i++)
        {
            float extra = Random.Range(0f, 6f);
            Vector3 candidato = pos + dirFuera * (distanciaMinimaHuida + extra);

            // Variacion lateral para que no huya siempre en linea recta
            Vector3 lateral = Vector3.Cross(Vector3.up, dirFuera).normalized;
            candidato += lateral * Random.Range(-6f, 6f);

            if (NavMesh.SamplePosition(candidato, out NavMeshHit hit, 4f, NavMesh.AllAreas))
            {
                // Validar que quede lejos del jugador
                if (Vector3.Distance(hit.position, jugador.position) >= distanciaMinimaHuida)
                    return hit.position;
            }
        }

        // Fallback: buscar punto de interes mas lejano al jugador
        if (puntosInteres != null && puntosInteres.Length > 0)
        {
            Transform mejor = null;
            float mejorDist = -1f;

            for (int i = 0; i < puntosInteres.Length; i++)
            {
                Transform t = puntosInteres[i];
                if (t == null) continue;

                float d = Vector3.Distance(t.position, jugador.position);
                if (d > mejorDist)
                {
                    mejorDist = d;
                    mejor = t;
                }
            }

            if (mejor != null)
            {
                if (NavMesh.SamplePosition(mejor.position, out NavMeshHit hit, 4f, NavMesh.AllAreas))
                    return hit.position;
                return mejor.position;
            }
        }

        // Ultimo recurso: huir hacia adelante
        return pos + dirFuera * distanciaMinimaHuida;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!dibujarGizmos) return;

        // Radios al rededor del animal
        Gizmos.color = new Color(1f, 1f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, distanciaAlerta);

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawWireSphere(transform.position, distanciaHuir);

        // Puntos de interes
        if (puntosInteres != null)
        {
            Gizmos.color = Color.cyan;
            foreach (var p in puntosInteres)
            {
                if (p != null) Gizmos.DrawSphere(p.position, 0.15f);
            }
        }
    }
#endif
}
