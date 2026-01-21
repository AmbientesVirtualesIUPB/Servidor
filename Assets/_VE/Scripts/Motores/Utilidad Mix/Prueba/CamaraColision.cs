
using UnityEngine;

public class CamaraColision : MonoBehaviour
{
    public Transform target;          // Player o pivot
    public float distanciaDeseada = 4f;
    public float distanciaMinima = 1f;
    public float suavizado = 10f;
    public LayerMask capasColision;

    private float distanciaActual;

    void Start()
    {
        distanciaActual = distanciaDeseada;
    }

    void LateUpdate()
    {
        Vector3 direccion = (transform.position - target.position).normalized;

        RaycastHit hit;
        if (Physics.Raycast(target.position, direccion, out hit, distanciaDeseada, capasColision))
        {
            distanciaActual = Mathf.Clamp(hit.distance - 0.2f, distanciaMinima, distanciaDeseada);
        }
        else
        {
            distanciaActual = Mathf.Lerp(distanciaActual, distanciaDeseada, Time.deltaTime * suavizado);
        }

        transform.position = target.position + direccion * distanciaActual;
    }
}
