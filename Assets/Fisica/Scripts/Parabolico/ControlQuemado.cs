using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ControlQuemado : MonoBehaviour
{
    [Header("Configuración de animación")]
    [Tooltip("Duración total de la animación en segundos.")]
    public float duracion = 1f;

    [Tooltip("Curva que define cómo progresa la animación en el tiempo (0 a 1).")]
    public AnimationCurve curva = AnimationCurve.Linear(0f, 0f, 1f, 1f);

    [Header("Modo")]
    [Tooltip("Si está activado, se restaura (0 → 1). Si no, se quema (1 → 0).")]
    public bool restaurar = false;

    [Header("Propiedad del material")]
    [Tooltip("Nombre de la propiedad de shader que controla el quemado.")]
    public string nombrePropiedad = "_quemado";

    [Tooltip("Índice del material en el Renderer (0 si solo tiene uno).")]
    public int indiceMaterial = 0;

    private Renderer _renderer;
    private Material _material;
    private Coroutine _corutinaActual;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        ObtenerMaterial();
    }

    /// <summary>
    /// Obtiene el material del Renderer según el índice.
    /// </summary>
    private void ObtenerMaterial()
    {
        if (_renderer == null) return;

        // Usamos .materials para no modificar el material compartido en todos los objetos
        var mats = _renderer.materials;

        if (mats == null || mats.Length == 0)
        {
            Debug.LogWarning($"[{nameof(ControlQuemado)}] El Renderer no tiene materiales.", this);
            return;
        }

        if (indiceMaterial < 0 || indiceMaterial >= mats.Length)
        {
            Debug.LogWarning($"[{nameof(ControlQuemado)}] Índice de material fuera de rango, usando 0.", this);
            indiceMaterial = 0;
        }

        _material = mats[indiceMaterial];

        if (!_material.HasProperty(nombrePropiedad))
        {
            Debug.LogWarning($"[{nameof(ControlQuemado)}] El material no tiene la propiedad '{nombrePropiedad}'.", this);
        }
    }

    /// <summary>
    /// Método público para lanzar la animación desde otros scripts.
    /// </summary>
    public void EjecutarAnimacion()
    {
        if (_material == null)
        {
            ObtenerMaterial();
            if (_material == null) return;
        }

        if (_corutinaActual != null)
            StopCoroutine(_corutinaActual);

        _corutinaActual = StartCoroutine(AnimarQuemado());
    }

    /// <summary>
    /// Método accesible desde el menú contextual del componente (click en los 3 punticos del script).
    /// </summary>
    [ContextMenu("Ejecutar animación de quemado")]
    private void EjecutarDesdeContextMenu()
    {
        EjecutarAnimacion();
    }

    private System.Collections.IEnumerator AnimarQuemado()
    {
        float inicio = restaurar ? 0f : 1f;  // 0→1 restaura, 1→0 quema
        float fin = restaurar ? 1f : 0f;

        float tiempo = 0f;

        while (tiempo < duracion)
        {
            float t = tiempo / duracion;           // 0 → 1
            float tCurva = curva.Evaluate(t);      // Aplicamos curva
            float valor = Mathf.Lerp(inicio, fin, tCurva);

            _material.SetFloat(nombrePropiedad, valor);

            tiempo += Time.deltaTime;
            yield return null;
        }

        // Aseguramos valor final exacto
        _material.SetFloat(nombrePropiedad, fin);
        _corutinaActual = null;
    }
}
