using UnityEngine;

public class OffsetAnimadorResultados : MonoBehaviour
{
    [Header("Material / Textura")]
    [Tooltip("Material que se va a modificar (offset de la textura).")]
    public Material materialObjetivo;

    [Tooltip("Propiedad de textura a modificar (por defecto _MainTex).")]
    public string nombrePropiedadTextura = "_MainTex";

    [Header("Parámetros de animación")]
    [Tooltip("Velocidad base que se multiplicará por la curva.")]
    public float velocidadBase = 1f;

    [Tooltip("Número de repeticiones del ciclo en la corrutina (por ejemplo 100).")]
    public int repeticiones = 100;

    [Tooltip("Tiempo de pausa entre cada repetición (segundos).")]
    public float pausaSegundos = 0.05f;

    [Header("Curvas")]
    [Tooltip("Curva para animación de ACIERTO (se multiplica a la velocidad).")]
    public AnimationCurve curvaAcierto = AnimationCurve.Linear(0f, 1f, 1f, 1f);

    [Tooltip("Curva para animación de FALLO (se multiplica a la velocidad).")]
    public AnimationCurve curvaFallo = AnimationCurve.Linear(0f, 1f, 1f, 0f);

    private Coroutine _corutinaActual;

    // ==========================
    // MÉTODOS PÚBLICOS
    // ==========================

    /// <summary>
    /// Inicia la animación usando la curva de ACIERTO.
    /// </summary>
    public void IniciarAnimacionAcierto()
    {
        IniciarAnimacion(curvaAcierto);
    }

    /// <summary>
    /// Inicia la animación usando la curva de FALLO.
    /// </summary>
    public void IniciarAnimacionFallo()
    {
        IniciarAnimacion(curvaFallo);
    }

    // ==========================
    // MÉTODOS DE CONTEXT MENU
    // ==========================

    [ContextMenu("Probar animación de ACIERTO")]
    private void ProbarAnimacionAcierto()
    {
        IniciarAnimacionAcierto();
    }

    [ContextMenu("Probar animación de FALLO")]
    private void ProbarAnimacionFallo()
    {
        IniciarAnimacionFallo();
    }

    // ==========================
    // LÓGICA INTERNA
    // ==========================

    private void IniciarAnimacion(AnimationCurve curva)
    {
        if (materialObjetivo == null)
        {
            Debug.LogWarning($"[{nameof(OffsetAnimadorResultados)}] No hay material asignado.", this);
            return;
        }

        if (_corutinaActual != null)
            StopCoroutine(_corutinaActual);

        _corutinaActual = StartCoroutine(CorutinaAnimarOffset(curva));
    }

    private System.Collections.IEnumerator CorutinaAnimarOffset(AnimationCurve curva)
    {
        if (!materialObjetivo.HasProperty(nombrePropiedadTextura))
        {
            Debug.LogWarning($"[{nameof(OffsetAnimadorResultados)}] El material no tiene la propiedad de textura '{nombrePropiedadTextura}'.", this);
            yield break;
        }

        // Tomamos el offset inicial por si quieres partir desde ahí
        Vector2 offset = materialObjetivo.GetTextureOffset(nombrePropiedadTextura);

        int pasos = Mathf.Max(1, repeticiones);

        for (int i = 0; i < pasos; i++)
        {
            // tNormalizado entre 0 y 1 según la iteración
            float t = (pasos > 1) ? (float)i / (pasos - 1) : 0f;

            // Factor que da la curva (por ejemplo, sube o baja la velocidad)
            float factor = curva.Evaluate(t);

            // Velocidad actual = velocidad base * valor de la curva en este punto
            float velocidadActual = velocidadBase * factor;

            // Sumamos al offset en Y: velocidad (unidades/seg) * deltaTiempo (pausa)
            // para que tenga sentido de "velocidad"
            offset.y += velocidadActual * pausaSegundos;

            // Aplicamos al material
            materialObjetivo.SetTextureOffset(nombrePropiedadTextura, offset);

            // Esperamos la pausa indicada
            yield return new WaitForSeconds(pausaSegundos);
        }

        _corutinaActual = null;
    }
}
