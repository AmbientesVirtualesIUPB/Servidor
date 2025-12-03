using UnityEngine;

public class SuspensionPorScaleFactor : MonoBehaviour
{
    [Header("Referencia al escalador")]
    public VertexRadialScalerByPaint escalador;

    [Header("Raycast hacia el suelo")]
    public LayerMask capasSuelo;
    public float distanciaRaycast = 5f;
    public float offsetRayOrigen = 0.1f;

    [Header("Ajustes de altura")]
    public float multiplicadorAltura = 1f;
    public float offsetAltura = 0f;
    public Vector3 ajuste;

    [Header("Suavizado")]
    public float tiempoSuavizado = 0.2f;

    [Header("Umbral para que no interfiera con el movimiento manual")]
    [Tooltip("Diferencia mínima para ajustar la altura. Si es menor, no se mueve.")]
    public float umbralAjuste = 0.01f;

    private float velocidadSuavizadoY;

    private void LateUpdate()
    {
        if (escalador == null)
            return;

        // --- 1) Raycast ---
        Vector3 origen = transform.position + Vector3.up * offsetRayOrigen;

        if (!Physics.Raycast(origen, Vector3.down, out RaycastHit hit, distanciaRaycast, capasSuelo))
            return;

        // --- 2) Calcular altura objetivo ---
        float baseSuelo = hit.point.y;
        float alturaRueda = (escalador.scaleFactor * 0.5f) * multiplicadorAltura;

        float yObjetivo = baseSuelo + alturaRueda + offsetAltura;

        float yActual = transform.position.y;
        float diferencia = Mathf.Abs(yActual - yObjetivo);

        // --- 3) Si ya está cerca, NO hacer nada (permite mover el objeto libremente) ---
        if (diferencia < umbralAjuste)
            return;

        // --- 4) Ajustar solo el eje Y, sin tocar X ni Z ---
        float ySuavizado = Mathf.SmoothDamp(
            yActual,
            yObjetivo,
            ref velocidadSuavizadoY,
            tiempoSuavizado
        );

        Vector3 nuevaPos = transform.position;
        nuevaPos.y = ySuavizado;
        transform.position = nuevaPos + ajuste;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origen = transform.position + Vector3.up * offsetRayOrigen;
        Gizmos.DrawLine(origen, origen + Vector3.down * distanciaRaycast);
    }
}
