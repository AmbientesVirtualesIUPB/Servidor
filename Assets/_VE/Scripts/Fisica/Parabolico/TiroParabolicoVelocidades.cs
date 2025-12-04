using UnityEngine;

public class TiroParabolicoVelocidades : MonoBehaviour
{
    public TiroParabolico referenciaTiro;
    public float escalaVelocidad = 0.5f;
    public LineRenderer lineaHorizontal;
    public LineRenderer lineaVertical;

    void Update()
    {
        if (referenciaTiro == null || lineaHorizontal == null || lineaVertical == null) return;

        float gravedad = Mathf.Abs(Physics.gravity.y);
        float anguloRad = referenciaTiro.anguloGrados * Mathf.Deg2Rad;
        float v0 = referenciaTiro.velocidadInicial;
        float v0x = v0 * Mathf.Cos(anguloRad);
        float v0y = v0 * Mathf.Sin(anguloRad);
        float tiempoTotal = (2 * v0y) / gravedad;

        float t = referenciaTiro.tiempoSlider * tiempoTotal;
        float vy = v0y - gravedad * t;

        Vector3 posicion = referenciaTiro.transform.position;
        Vector3 frente = referenciaTiro.transform.forward;

        // Actualizar línea horizontal
        lineaHorizontal.positionCount = 2;
        lineaHorizontal.SetPosition(0, posicion);
        lineaHorizontal.SetPosition(1, posicion + frente * v0x * escalaVelocidad);

        // Actualizar línea vertical
        lineaVertical.positionCount = 2;
        lineaVertical.SetPosition(0, posicion);
        lineaVertical.SetPosition(1, posicion + Vector3.up * vy * escalaVelocidad);
    }
}
