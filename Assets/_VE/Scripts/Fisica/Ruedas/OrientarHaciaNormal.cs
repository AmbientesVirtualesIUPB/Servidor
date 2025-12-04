using UnityEngine;

public class OrientarHaciaNormal : MonoBehaviour
{
    [Header("Raycast")]
    [Tooltip("Distancia del raycast hacia abajo.")]
    public float distancia = 2f;

    [Tooltip("Capas donde se va a detectar el piso.")]
    public LayerMask capasDeteccion;

    [Header("Giro")]
    [Tooltip("Factor de suavizado. 0 = rotación instantánea.")]
    public float suavizado = 10f;

    [Header("Dirección de movimiento")]
    [Tooltip("Velocidad mínima (en unidades/frame) para considerar que hay movimiento.")]
    public float umbralMovimiento = 0.001f;

    private Vector3 ultimaPosicion;

    private void Start()
    {
        ultimaPosicion = transform.position;
    }

    private void Update()
    {
        HacerRaycastYOrientar();
        ultimaPosicion = transform.position; // actualizar al final del frame
    }

    private void HacerRaycastYOrientar()
    {
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, distancia, capasDeteccion))
        {
            Vector3 normalSuelo = hit.normal;

            // === 1) Calcular dirección de movimiento ===
            Vector3 desplazamiento = transform.position - ultimaPosicion;
            Vector3 dirMovimiento = desplazamiento;

            // Proyectamos la dirección de movimiento sobre el plano del suelo
            dirMovimiento = Vector3.ProjectOnPlane(dirMovimiento, normalSuelo);

            // Si casi no se mueve, usamos el forward actual proyectado en el plano
            if (dirMovimiento.sqrMagnitude < umbralMovimiento * umbralMovimiento)
            {
                dirMovimiento = Vector3.ProjectOnPlane(transform.forward, normalSuelo);
            }

            // Si aun así es casi cero, salimos para evitar errores
            if (dirMovimiento.sqrMagnitude < 1e-6f)
            {
                return;
            }

            dirMovimiento.Normalize();

            // === 2) Rotación objetivo con UP = normal del suelo y FORWARD = dirección movimiento ===
            Quaternion orientacionObjetivo = Quaternion.LookRotation(dirMovimiento, normalSuelo);

            // === 3) Aplicar rotación suavizada ===
            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                orientacionObjetivo,
                Time.deltaTime * suavizado
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * distancia);
    }
}
