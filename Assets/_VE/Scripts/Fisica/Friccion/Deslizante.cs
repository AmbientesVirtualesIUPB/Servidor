using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deslizante : MonoBehaviour
{
    [Header("Configuración de la Curva")]
    public Transform[] puntos;
    [Range(0f, 1f)]
    public float t = 0f;

    [Header("Configuración de Movimiento")]
    public bool moverAutomaticamente = false;
    public float velocidad = 1f;
    public bool loop = false;

    [Header("Orientación")]
    public bool orientarSegunCurva = true;
    public Vector3 planoReferencia = Vector3.up; // Vector de referencia para calcular la normal (generalmente Vector3.up)

    [Header("Visualización")]
    public bool mostrarGizmos = true;
    public Color colorCurva = Color.green;
    public Color colorPuntos = Color.red;
    public int resolucionCurva = 50;

    private bool moviendoHaciaAdelante = true;

    private void Start()
    {
        if (puntos == null || puntos.Length < 2)
        {
            Debug.LogWarning("Se necesitan al menos 2 puntos para crear una curva de Bézier");
        }
    }

    private void Update()
    {
        if (puntos == null || puntos.Length < 2) return;

        // Movimiento automático
        if (moverAutomaticamente)
        {
            if (loop)
            {
                // Movimiento en loop continuo
                t += velocidad * Time.deltaTime;
                if (t > 1f) t = 0f;
            }
            else
            {
                // Movimiento ping-pong
                if (moviendoHaciaAdelante)
                {
                    t += velocidad * Time.deltaTime;
                    if (t >= 1f)
                    {
                        t = 1f;
                        moviendoHaciaAdelante = false;
                    }
                }
                else
                {
                    t -= velocidad * Time.deltaTime;
                    if (t <= 0f)
                    {
                        t = 0f;
                        moviendoHaciaAdelante = true;
                    }
                }
            }
        }

        // Aplicar la posición calculada de la curva de Bézier
        transform.position = CalcularPuntoEnCurva(t);

        // Aplicar la orientación según la curva
        if (orientarSegunCurva)
        {
            Vector3 tangente = ObtenerTangenteEnCurva(t);
            Vector3 normal = ObtenerNormalEnCurva(t);

            if (tangente != Vector3.zero && normal != Vector3.zero)
            {
                AplicarOrientacionConTangenteYNormal(tangente, normal);
            }
        }
    }

    private Vector3 CalcularPuntoEnCurva(float tiempo)
    {
        if (puntos.Length == 2)
        {
            // Lerp simple para 2 puntos
            return Vector3.Lerp(puntos[0].position, puntos[1].position, tiempo);
        }
        else if (puntos.Length == 3)
        {
            // Curva de Bézier cuadrática
            return CalcularBezierCuadratica(puntos[0].position, puntos[1].position, puntos[2].position, tiempo);
        }
        else if (puntos.Length == 4)
        {
            // Curva de Bézier cúbica
            return CalcularBezierCubica(puntos[0].position, puntos[1].position, puntos[2].position, puntos[3].position, tiempo);
        }
        else
        {
            // Para más de 4 puntos, usar algoritmo de De Casteljau
            return CalcularBezierDeCasteljau(tiempo);
        }
    }

    private Vector3 CalcularBezierCuadratica(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    private Vector3 CalcularBezierCubica(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float u = 1 - t;
        return u * u * u * p0 + 3 * u * u * t * p1 + 3 * u * t * t * p2 + t * t * t * p3;
    }

    private Vector3 CalcularBezierDeCasteljau(float t)
    {
        Vector3[] puntosTemp = new Vector3[puntos.Length];

        // Copiar las posiciones iniciales
        for (int i = 0; i < puntos.Length; i++)
        {
            puntosTemp[i] = puntos[i].position;
        }

        // Algoritmo de De Casteljau
        for (int r = 1; r < puntos.Length; r++)
        {
            for (int i = 0; i < puntos.Length - r; i++)
            {
                puntosTemp[i] = Vector3.Lerp(puntosTemp[i], puntosTemp[i + 1], t);
            }
        }

        return puntosTemp[0];
    }

    // Método público para obtener un punto específico en la curva
    public Vector3 ObtenerPuntoEnCurva(float tiempo)
    {
        return CalcularPuntoEnCurva(Mathf.Clamp01(tiempo));
    }

    // Método para obtener la tangente en un punto específico
    public Vector3 ObtenerTangenteEnCurva(float tiempo, float delta = 0.01f)
    {
        tiempo = Mathf.Clamp01(tiempo);

        // Para evitar problemas en los extremos, ajustamos el delta
        float t1 = Mathf.Clamp01(tiempo - delta * 0.5f);
        float t2 = Mathf.Clamp01(tiempo + delta * 0.5f);

        Vector3 p1 = CalcularPuntoEnCurva(t1);
        Vector3 p2 = CalcularPuntoEnCurva(t2);

        Vector3 tangente = (p2 - p1).normalized;

        // Si la tangente es muy pequeña, usar método alternativo
        if (tangente.magnitude < 0.001f)
        {
            tangente = CalcularTangenteDirecta(tiempo);
        }

        return tangente;
    }

    // Método alternativo para calcular tangente usando derivadas
    private Vector3 CalcularTangenteDirecta(float tiempo)
    {
        if (puntos.Length == 2)
        {
            return (puntos[1].position - puntos[0].position).normalized;
        }
        else if (puntos.Length == 3)
        {
            // Derivada de Bézier cuadrática
            float t = tiempo;
            Vector3 p0 = puntos[0].position;
            Vector3 p1 = puntos[1].position;
            Vector3 p2 = puntos[2].position;

            return (2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1)).normalized;
        }
        else if (puntos.Length == 4)
        {
            // Derivada de Bézier cúbica
            float t = tiempo;
            Vector3 p0 = puntos[0].position;
            Vector3 p1 = puntos[1].position;
            Vector3 p2 = puntos[2].position;
            Vector3 p3 = puntos[3].position;

            float u = 1 - t;
            return (3 * u * u * (p1 - p0) + 6 * u * t * (p2 - p1) + 3 * t * t * (p3 - p2)).normalized;
        }
        else
        {
            // Para curvas de orden superior, usar aproximación numérica
            float deltaSmall = 0.001f;
            Vector3 p1 = CalcularPuntoEnCurva(Mathf.Clamp01(tiempo - deltaSmall));
            Vector3 p2 = CalcularPuntoEnCurva(Mathf.Clamp01(tiempo + deltaSmall));
            return (p2 - p1).normalized;
        }
    }

    // Método para obtener la normal en un punto específico de la curva
    public Vector3 ObtenerNormalEnCurva(float tiempo)
    {
        Vector3 tangente = ObtenerTangenteEnCurva(tiempo);
        Vector3 binormal = ObtenerBinormalEnCurva(tiempo);

        // La normal es perpendicular tanto a la tangente como a la binormal
        Vector3 normal = Vector3.Cross(binormal, tangente).normalized;

        // Si no se puede calcular una normal válida, usar el plano de referencia
        if (normal.magnitude < 0.001f)
        {
            // Crear una normal perpendicular a la tangente usando el plano de referencia
            Vector3 derecha = Vector3.Cross(tangente, planoReferencia).normalized;
            if (derecha.magnitude < 0.001f)
            {
                // Si la tangente es paralela al plano de referencia, usar otro vector
                derecha = Vector3.Cross(tangente, Vector3.right).normalized;
                if (derecha.magnitude < 0.001f)
                {
                    derecha = Vector3.Cross(tangente, Vector3.forward).normalized;
                }
            }
            normal = Vector3.Cross(tangente, derecha).normalized;
        }

        return normal;
    }

    // Método para obtener la binormal en un punto específico de la curva
    public Vector3 ObtenerBinormalEnCurva(float tiempo, float delta = 0.01f)
    {
        // La binormal se calcula usando la segunda derivada (curvatura)
        Vector3 tangente1 = ObtenerTangenteEnCurva(Mathf.Clamp01(tiempo - delta * 0.5f));
        Vector3 tangente2 = ObtenerTangenteEnCurva(Mathf.Clamp01(tiempo + delta * 0.5f));

        Vector3 segundaDerivada = (tangente2 - tangente1).normalized;

        if (segundaDerivada.magnitude < 0.001f)
        {
            // Si no hay curvatura, usar el plano de referencia
            Vector3 tangente = ObtenerTangenteEnCurva(tiempo);
            Vector3 binormal = Vector3.Cross(tangente, planoReferencia).normalized;

            if (binormal.magnitude < 0.001f)
            {
                binormal = Vector3.Cross(tangente, Vector3.right).normalized;
                if (binormal.magnitude < 0.001f)
                {
                    binormal = Vector3.Cross(tangente, Vector3.forward).normalized;
                }
            }
            return binormal;
        }

        return segundaDerivada;
    }

    // Método para aplicar la orientación usando tangente y normal
    private void AplicarOrientacionConTangenteYNormal(Vector3 tangente, Vector3 normal)
    {
        // La tangente apunta hacia adelante (forward)
        Vector3 adelante = tangente.normalized;

        // La normal apunta hacia arriba (up)
        Vector3 arriba = normal.normalized;

        // El vector derecha se calcula como el producto cruz de arriba y adelante
        Vector3 derecha = Vector3.Cross(arriba, adelante).normalized;

        // Recalcular arriba para asegurar ortogonalidad perfecta
        arriba = Vector3.Cross(adelante, derecha).normalized;

        // Aplicar la rotación usando LookRotation
        if (adelante != Vector3.zero && arriba != Vector3.zero)
        {
            //transform.rotation = Quaternion.LookRotation(adelante, arriba);
        }
    }

    private void OnDrawGizmos()
    {
        if (!mostrarGizmos || puntos == null || puntos.Length < 2) return;

        // Dibujar los puntos de control
        Gizmos.color = colorPuntos;
        for (int i = 0; i < puntos.Length; i++)
        {
            if (puntos[i] != null)
            {
                Gizmos.DrawSphere(puntos[i].position, 0.1f);

                // Mostrar números de los puntos
#if UNITY_EDITOR
                UnityEditor.Handles.Label(puntos[i].position + Vector3.up * 0.3f, i.ToString());
#endif
            }
        }

        // Dibujar líneas entre puntos de control
        Gizmos.color = Color.gray;
        for (int i = 0; i < puntos.Length - 1; i++)
        {
            if (puntos[i] != null && puntos[i + 1] != null)
            {
                Gizmos.DrawLine(puntos[i].position, puntos[i + 1].position);
            }
        }

        // Dibujar la curva de Bézier
        Gizmos.color = colorCurva;
        Vector3 puntoAnterior = CalcularPuntoEnCurva(0);

        for (int i = 1; i <= resolucionCurva; i++)
        {
            float tiempo = (float)i / resolucionCurva;
            Vector3 puntoActual = CalcularPuntoEnCurva(tiempo);
            Gizmos.DrawLine(puntoAnterior, puntoActual);
            puntoAnterior = puntoActual;
        }

        // Dibujar la posición actual del objeto
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(CalcularPuntoEnCurva(t), 0.15f);
    }

    private void OnDrawGizmosSelected()
    {
        if (!mostrarGizmos || puntos == null || puntos.Length < 2) return;

        // Dibujar información adicional cuando está seleccionado
        Vector3 posicionActual = CalcularPuntoEnCurva(t);

        // Dibujar tangente (dirección de movimiento)
        Gizmos.color = Color.cyan;
        Vector3 tangente = ObtenerTangenteEnCurva(t);
        Gizmos.DrawRay(posicionActual, tangente * 1f);

        // Dibujar normal (hacia arriba de la curva)
        Gizmos.color = Color.magenta;
        Vector3 normal = ObtenerNormalEnCurva(t);
        Gizmos.DrawRay(posicionActual, normal * 0.7f);

        // Dibujar binormal (lateral de la curva)
        Gizmos.color = Color.yellow;
        Vector3 binormal = ObtenerBinormalEnCurva(t);
        Gizmos.DrawRay(posicionActual, binormal * 0.5f);

        // Dibujar los ejes del objeto para visualizar la orientación final
        if (orientarSegunCurva)
        {
            Gizmos.color = Color.red;   // X - Derecha
            Gizmos.DrawRay(posicionActual, transform.right * 0.4f);

            Gizmos.color = Color.green; // Y - Arriba  
            Gizmos.DrawRay(posicionActual, transform.up * 0.4f);

            Gizmos.color = Color.blue;  // Z - Adelante
            Gizmos.DrawRay(posicionActual, transform.forward * 0.4f);
        }
    }
}