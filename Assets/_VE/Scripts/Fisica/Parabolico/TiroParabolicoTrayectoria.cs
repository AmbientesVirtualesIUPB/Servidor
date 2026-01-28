using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class TiroParabolicoTrayectoria : MonoBehaviour
{
    public TiroParabolico referenciaTiro;
    public int divisiones = 20;

    private LineRenderer lineRenderer;
    private Vector3 origenInicial;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = divisiones + 1;
    }

    void Start()
    {
        if (referenciaTiro != null)
        {
            origenInicial = referenciaTiro.transform.position;
        }
    }

    void Update()
    {
        if (referenciaTiro == null) return;

        float gravedad = Mathf.Abs(Physics.gravity.y);
        float anguloRad = referenciaTiro.anguloGrados * Mathf.Deg2Rad;
        float v0 = referenciaTiro.velocidadInicial;
        float v0x = v0 * Mathf.Cos(anguloRad);
        float v0y = v0 * Mathf.Sin(anguloRad);
        float tiempoTotal = (2 * v0y) / gravedad;

        Vector3 frente = referenciaTiro.transform.forward;

        for (int i = 0; i <= divisiones; i++)
        {
            float t = (i / (float)divisiones) * tiempoTotal;
            float x = v0x * t;
            float y = v0y * t - 0.5f * gravedad * t * t;

            Vector3 desplazamiento = frente * x + Vector3.up * y;
            lineRenderer.SetPosition(i, origenInicial + desplazamiento);
        }
    }
}
