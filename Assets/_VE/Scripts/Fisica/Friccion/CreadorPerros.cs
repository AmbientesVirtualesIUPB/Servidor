using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreadorPerros : MonoBehaviour
{
    [Header("Esquinas del cuadrilátero (en orden alrededor)")]
    public Transform A;
    public Transform B;
    public Transform C;
    public Transform D;

    [Header("Prefab a instanciar")]
    public GameObject prefabGrande;
    public GameObject prefabMediano;
    public GameObject prefabPeque;

    [Header("Opciones de creación")]
    [Min(0)] public int cantidad = 50;

    public int cantidadGrandes  = 10;
    public int cantidadMedianos = 20;
    public int cantidadPeques   = 20;

    public Text txtGrande, txtMediano, txtPeque;

    public Transform contenedor;     // opcional: parent de las instancias
    public bool rotacionAleatoriaY = true;
    public Vector2 escalaUniforme = Vector2.one; // x=min, y=max (1,1 = sin cambio)

    [Header("Separación posterior (anti-solapes)")]
    [Tooltip("Distancia mínima entre centros de instancias")]
    public float radioMinimo = 0.5f;
    [Tooltip("Número de iteraciones de relajación")]
    [Range(0, 200)] public int iteracionesRelax = 40;
    [Tooltip("Factor de paso de cada corrección (0..1). 1 = corrige todo el faltante por iteración")]
    [Range(0.01f, 1f)] public float pasoRelax = 0.5f;

    public static CreadorPerros singleton;
    List<Transform> instancias;

    public void CambioPerroGrande(int cuanto)
    {
        cantidadGrandes += cuanto;
        if (cantidadGrandes < 0) cantidadGrandes = 0;
        txtGrande.text = cantidadGrandes.ToString();
    }
    public void CambioPerroMediano(int cuanto)
    {
        cantidadMedianos += cuanto;
        if (cantidadMedianos < 0) cantidadMedianos = 0;
        txtMediano.text = cantidadMedianos.ToString();
    }
    public void CambioPerroPeque(int cuanto)
    {
        cantidadPeques += cuanto;
        if (cantidadPeques < 0) cantidadPeques = 0;
        txtPeque.text = cantidadPeques.ToString();
    }


    private void Awake() => singleton = this;

    [ContextMenu("Generar")]
    public void Generar()
    {
        if (!ValidarEntradas()) return;

        // posiciones de las esquinas (usamos diagonal AC)
        Vector3 pA = A.position;
        Vector3 pB = B.position;
        Vector3 pC = C.position;
        Vector3 pD = D.position;

        // normal del plano (para mantener los puntos sobre el cuadrilátero)
        Vector3 n = Vector3.Cross(pB - pA, pC - pA).normalized;

        // áreas de los dos triángulos para muestreo proporcional
        float areaABC = AreaTriangulo(pA, pB, pC);
        float areaACD = AreaTriangulo(pA, pC, pD);
        float areaTotal = areaABC + areaACD;

        // Instanciar y guardar referencias
        if(instancias != null && instancias.Count > 0) {
            for (int i = 0; i < instancias.Count; i++)
            {
                Destroy(instancias[i].gameObject);
            }
        }
        instancias = new List<Transform>(cantidad);

        for (int i = 0; i < cantidadGrandes; i++)
        {
            bool usarABC = Random.value < (areaABC / areaTotal);
            Vector3 punto = usarABC
                ? PuntoAleatorioEnTriangulo(pA, pB, pC)
                : PuntoAleatorioEnTriangulo(pA, pC, pD);

            Quaternion rot = rotacionAleatoriaY
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : transform.rotation;

            float s = Mathf.Clamp(Random.Range(escalaUniforme.x, escalaUniforme.y), 0.0001f, 1000f);
            Vector3 scale = new Vector3(s, s, s);

            GameObject go = Instantiate(prefabGrande, punto, rot, contenedor ? contenedor : transform);
            go.transform.localScale = Vector3.Scale(prefabGrande.transform.localScale, scale);
            instancias.Add(go.transform);
        }

        for (int i = 0; i < cantidadMedianos; i++)
        {
            bool usarABC = Random.value < (areaABC / areaTotal);
            Vector3 punto = usarABC
                ? PuntoAleatorioEnTriangulo(pA, pB, pC)
                : PuntoAleatorioEnTriangulo(pA, pC, pD);

            Quaternion rot = rotacionAleatoriaY
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : transform.rotation;

            float s = Mathf.Clamp(Random.Range(escalaUniforme.x, escalaUniforme.y), 0.0001f, 1000f);
            Vector3 scale = new Vector3(s, s, s);

            GameObject go = Instantiate(prefabMediano, punto, rot, contenedor ? contenedor : transform);
            go.transform.localScale = Vector3.Scale(prefabMediano.transform.localScale, scale);
            instancias.Add(go.transform);
        }

        for (int i = 0; i < cantidadPeques; i++)
        {
            bool usarABC = Random.value < (areaABC / areaTotal);
            Vector3 punto = usarABC
                ? PuntoAleatorioEnTriangulo(pA, pB, pC)
                : PuntoAleatorioEnTriangulo(pA, pC, pD);

            Quaternion rot = rotacionAleatoriaY
                ? Quaternion.Euler(0f, Random.Range(0f, 360f), 0f)
                : transform.rotation;

            float s = Mathf.Clamp(Random.Range(escalaUniforme.x, escalaUniforme.y), 0.0001f, 1000f);
            Vector3 scale = new Vector3(s, s, s);

            GameObject go = Instantiate(prefabPeque, punto, rot, contenedor ? contenedor : transform);
            go.transform.localScale = Vector3.Scale(prefabPeque.transform.localScale, scale);
            instancias.Add(go.transform);
        }





        // Relajación por repulsión (anti-solapes)
        if (radioMinimo > 0f && iteracionesRelax > 0)
            SepararSinSolapes(instancias, pA, pB, pC, pD, n);
    }

    // ------------------- Anti-solapes -------------------

    private void SepararSinSolapes(List<Transform> ps, Vector3 a, Vector3 b, Vector3 c, Vector3 d, Vector3 normal)
    {
        float r = Mathf.Max(0.0001f, radioMinimo);
        float r2 = r * r;

        for (int it = 0; it < iteracionesRelax; it++)
        {
            bool huboEmpujes = false;

            for (int i = 0; i < ps.Count; i++)
            {
                for (int j = i + 1; j < ps.Count; j++)
                {
                    Vector3 diff = ps[i].position - ps[j].position;

                    // proyectar el vector de empuje al plano del quad
                    diff -= Vector3.Dot(diff, normal) * normal;

                    float dist2 = diff.sqrMagnitude;
                    if (dist2 < r2 && dist2 > 1e-8f)
                    {
                        float dist = Mathf.Sqrt(dist2);
                        // cuánto falta para llegar al radio mínimo
                        float faltante = (r - dist);
                        // mitad para cada punto, con paso
                        Vector3 push = diff.normalized * (faltante * 0.5f * pasoRelax);

                        ps[i].position += push;
                        ps[j].position -= push;
                        huboEmpujes = true;
                    }
                }
            }

            // Mantener cada punto sobre el plano y dentro del cuadrilátero
            for (int i = 0; i < ps.Count; i++)
            {
                // quitar componente fuera del plano
                Vector3 pi = ps[i].position;
                float h = Vector3.Dot(pi - a, normal);
                pi -= h * normal;

                // clamp al quad (unión de triángulos ABC y ACD)
                if (!PuntoEnTriangulo(pi, a, b, c) && !PuntoEnTriangulo(pi, a, c, d))
                {
                    // re-proyectar al punto más cercano de la unión
                    Vector3 q1 = PuntoCercanoEnTriangulo(pi, a, b, c);
                    Vector3 q2 = PuntoCercanoEnTriangulo(pi, a, c, d);
                    pi = (Vector3.SqrMagnitude(pi - q1) < Vector3.SqrMagnitude(pi - q2)) ? q1 : q2;
                }

                ps[i].position = pi;
            }

            if (!huboEmpujes) break; // ya no hay solapes
        }
    }

    // ------------------- Utilidades geométricas -------------------

    // Punto uniforme en un triángulo usando el truco de "reflexión" (u+v>1)
    private static Vector3 PuntoAleatorioEnTriangulo(Vector3 a, Vector3 b, Vector3 c)
    {
        float u = Random.value;
        float v = Random.value;
        if (u + v > 1f) { u = 1f - u; v = 1f - v; }
        return a + u * (b - a) + v * (c - a);
    }

    // Área de un triángulo 3D (|cross|/2)
    private static float AreaTriangulo(Vector3 a, Vector3 b, Vector3 c)
    {
        return Vector3.Cross(b - a, c - a).magnitude * 0.5f;
    }

    // Test punto en triángulo (barycentric, sin división)
    private static bool PuntoEnTriangulo(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 v0 = c - a;
        Vector3 v1 = b - a;
        Vector3 v2 = p - a;

        float dot00 = Vector3.Dot(v0, v0);
        float dot01 = Vector3.Dot(v0, v1);
        float dot02 = Vector3.Dot(v0, v2);
        float dot11 = Vector3.Dot(v1, v1);
        float dot12 = Vector3.Dot(v1, v2);

        float invDen = 1f / (dot00 * dot11 - dot01 * dot01);
        float u = (dot11 * dot02 - dot01 * dot12) * invDen;
        float v = (dot00 * dot12 - dot01 * dot02) * invDen;

        return (u >= 0f) && (v >= 0f) && (u + v <= 1f);
    }

    // Punto más cercano a P sobre el triángulo ABC (clásico)
    private static Vector3 PuntoCercanoEnTriangulo(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
    {
        // Basado en "Real-Time Collision Detection" (Christer Ericson)
        // 1) vértices
        Vector3 ab = b - a, ac = c - a, ap = p - a;
        float d1 = Vector3.Dot(ab, ap);
        float d2 = Vector3.Dot(ac, ap);
        if (d1 <= 0f && d2 <= 0f) return a;

        Vector3 bp = p - b;
        float d3 = Vector3.Dot(ab, bp);
        float d4 = Vector3.Dot(ac, bp);
        if (d3 >= 0f && d4 <= d3) return b;

        float vc = d1 * d4 - d3 * d2;
        if (vc <= 0f && d1 >= 0f && d3 <= 0f)
        {
            float v = d1 / (d1 - d3);
            return a + v * ab;
        }

        Vector3 cp = p - c;
        float d5 = Vector3.Dot(ab, cp);
        float d6 = Vector3.Dot(ac, cp);
        if (d6 >= 0f && d5 <= d6) return c;

        float vb = d5 * d2 - d1 * d6;
        if (vb <= 0f && d2 >= 0f && d6 <= 0f)
        {
            float w = d2 / (d2 - d6);
            return a + w * ac;
        }

        float va = d3 * d6 - d5 * d4;
        if (va <= 0f && (d4 - d3) >= 0f && (d5 - d6) >= 0f)
        {
            float w = (d4 - d3) / ((d4 - d3) + (d5 - d6));
            return b + w * (c - b);
        }

        // Interior: proyectar por baricéntricas
        float denom = 1f / (va + vb + vc);
        float v2 = vb * denom;
        float w2 = vc * denom;
        return a + ab * v2 + ac * w2;
    }

    private bool ValidarEntradas()
    {
        if (A == null || B == null || C == null || D == null)
        {
            Debug.LogError("[CreadorPerros] Faltan referencias a las 4 esquinas (A, B, C, D).");
            return false;
        }
        if (prefabGrande == null || prefabMediano == null || prefabPeque == null)
        {
            Debug.LogError("[CreadorPerros] Falta asignar el prefab.");
            return false;
        }
        return cantidad > 0;
    }

    private void OnDrawGizmos()
    {
        if (A && B && C && D)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(A.position, B.position);
            Gizmos.DrawLine(B.position, C.position);
            Gizmos.DrawLine(C.position, D.position);
            Gizmos.DrawLine(D.position, A.position);

            Gizmos.color = Color.yellow; // diagonal de triangulación
            Gizmos.DrawLine(A.position, C.position);
        }
    }
}

