using UnityEditor.Rendering;
using UnityEngine;

public class VelocidadesIndicador : MonoBehaviour
{
    [Header("Referencias")]
    public ProjectileFollower follower;
    public LineRenderer lineaHorizontal;
    public LineRenderer lineaVertical;

    [Header("Escala visual")]
    public float escala = 1f;

    void LateUpdate()
    {
        if (follower == null || lineaHorizontal == null || lineaVertical == null)
            return;
        escala = follower.ObtenerVelocidadActual()/10;
        Vector3 origen = transform.position;
        Vector3 tangente = follower.ObtenerDireccionActual(); // usa la tangente del movimiento

        // Componentes
        Vector3 velHorizontal = new Vector3(tangente.x, 0f, tangente.z) * escala;
        Vector3 velVertical = new Vector3(0f, tangente.y, 0f) * escala;

        // Línea horizontal (roja, por ejemplo)
        lineaHorizontal.positionCount = 50;
        for (int i = 0; i < 50; i++)
        {
            lineaHorizontal.SetPosition(i, Vector3.Lerp(origen, origen + velHorizontal, (float)i/49f));
        }

        // Línea vertical (azul, por ejemplo)
        lineaVertical.positionCount = 50;
        for (int i = 0; i < 50; i++)
        {
            lineaVertical.SetPosition(i, Vector3.Lerp(origen, origen + velVertical, (float)i / 49f));
        }
    }
}
