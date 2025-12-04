using UnityEngine;

/// <summary>
/// Escala radialmente (respecto al pivote/local) los vértices pintados en rojo del mesh,
/// manteniendo como referencia la distancia base medida al iniciar.
/// </summary>
[RequireComponent(typeof(MeshFilter))]
public class VertexRadialScalerByPaint : MonoBehaviour
{
    [Header("Referencia")]
    [Tooltip("Si se deja vacío, tomará el MeshFilter del mismo GameObject.")]
    public MeshRenderer meshRenderer; // opcional; el MeshFilter se obtiene del mismo GO

    [Header("Parámetros de control")]
    [Range(0f, 3f)]
    [Tooltip("Factor que multiplica la distancia base de los vértices rojos respecto al pivote local.")]
    public float scaleFactor = 1f;

    [Tooltip("Color objetivo considerado 'rojo'. Solo el canal R se usa para el umbral.")]
    public Color targetColor = Color.red;

    [Range(0f, 1f)]
    [Tooltip("Umbral para considerar un vértice como 'rojo' (usa el canal R). Ej: 0.8 significa R>=0.8.")]
    public float redThreshold = 0.8f;

    [Header("Pivote (espacio local del mesh)")]
    [Tooltip("Pivote local desde el que se mide la distancia. Normalmente (0,0,0).")]
    public Vector3 customPivotLocal = Vector3.zero;

    [Header("Depuración")]
    public bool drawGizmos = false;
    public float gizmoSize = 0.01f;
    public Color gizmoColor = Color.cyan;

    // Internos
    MeshFilter _mf;
    Mesh _runtimeMesh;
    Vector3[] _baseVertices;    // copia de los vértices iniciales (locales)
    float[] _baseDistances;     // distancia base de cada vértice al pivote
    bool[] _isRed;              // marca si el vértice es "rojo" según el umbral

    void Awake()
    {
        // Obtener MeshFilter (obligatorio). El MeshRenderer es solo para arrastrar en el inspector si se quiere.
        _mf = GetComponent<MeshFilter>();
        if (_mf == null)
        {
            Debug.LogError("Este componente requiere un MeshFilter en el mismo GameObject.");
            enabled = false;
            return;
        }

        // Preparar un mesh de runtime para no editar el asset.
        var shared = _mf.sharedMesh;
        if (shared == null)
        {
            Debug.LogError("No hay malla en el MeshFilter.");
            enabled = false;
            return;
        }

        _runtimeMesh = Instantiate(shared);
        _runtimeMesh.name = shared.name + " (RuntimeClone)";
        _mf.sharedMesh = _runtimeMesh; // usar el clon

        // Cachear datos base
        _baseVertices = _runtimeMesh.vertices; // en local space
        _baseDistances = new float[_baseVertices.Length];
        _isRed = new bool[_baseVertices.Length];

        // Leer colores de vértice
        var colors = _runtimeMesh.colors;
        if (colors == null || colors.Length != _baseVertices.Length)
        {
            // Intentar Colors32 si no hay Colors (algunas mallas los guardan en Colors32)
            var colors32 = _runtimeMesh.colors32;
            if (colors32 == null || colors32.Length != _baseVertices.Length)
            {
                Debug.LogWarning("Este mesh no tiene colores de vértice. No se detectarán vértices rojos.");
                // En este caso, ningún vértice será tratado como rojo.
                for (int i = 0; i < _baseVertices.Length; i++)
                {
                    _isRed[i] = false;
                    _baseDistances[i] = (_baseVertices[i] - customPivotLocal).magnitude;
                }
                return;
            }
            else
            {
                // Usar Colors32 → normalizar a [0,1]
                for (int i = 0; i < _baseVertices.Length; i++)
                {
                    float r = colors32[i].r / 255f;
                    _isRed[i] = (r >= redThreshold);
                    _baseDistances[i] = (_baseVertices[i] - customPivotLocal).magnitude;
                }
            }
        }
        else
        {
            // Usar Colors (float)
            for (int i = 0; i < _baseVertices.Length; i++)
            {
                float r = colors[i].r;
                _isRed[i] = (r >= redThreshold);
                _baseDistances[i] = (_baseVertices[i] - customPivotLocal).magnitude;
            }
        }
    }

    void FixedUpdate()
    {
        if (_runtimeMesh == null || _baseVertices == null) return;

        var verts = (Vector3[])_baseVertices.Clone(); // partimos de los originales cada frame físico

        // Ajustar SOLO los "rojos": posición = pivote + dir * (distBase * scaleFactor)
        for (int i = 0; i < verts.Length; i++)
        {
            if (!_isRed[i]) continue;

            Vector3 fromPivot = _baseVertices[i] - customPivotLocal;
            float baseDist = _baseDistances[i];

            // Si el vértice coincide con el pivote, no hay dirección que normalizar
            if (baseDist > 1e-6f)
            {
                Vector3 dir = fromPivot / baseDist; // dirección normalizada
                float newDist = baseDist * scaleFactor;
                verts[i] = customPivotLocal + dir * newDist;
            }
            else
            {
                verts[i] = customPivotLocal; // queda en el pivote
            }
        }

        // Aplicar cambios
        _runtimeMesh.vertices = verts;
        _runtimeMesh.RecalculateBounds(); // opcional
        // _runtimeMesh.RecalculateNormals(); // descomenta si necesitas actualizar normales (puede ser costoso)
    }

    void OnDrawGizmosSelected()
    {
        if (!drawGizmos || _runtimeMesh == null) return;

        Gizmos.color = gizmoColor;
        var verts = _runtimeMesh.vertices;
        // Mostrar sólo puntos de vértices rojos actuales
        for (int i = 0; i < verts.Length; i++)
        {
            if (_isRed != null && i < _isRed.Length && _isRed[i])
            {
                // Convertir a world space para la vista
                Vector3 worldPos = transform.TransformPoint(verts[i]);
                Gizmos.DrawSphere(worldPos, gizmoSize);
            }
        }
    }
}
