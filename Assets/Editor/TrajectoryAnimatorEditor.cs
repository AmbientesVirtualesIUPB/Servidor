using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TrajectoryAnimator))]
public class TrajectoryAnimatorEditor : Editor
{
    private TrajectoryAnimator trajectoryAnimator;

    // Estilos para los botones
    private GUIStyle buttonStyle;
    private GUIStyle largeButtonStyle;
    private GUIStyle successButtonStyle;
    private GUIStyle warningButtonStyle;
    private GUIStyle dangerButtonStyle;

    private void OnEnable()
    {
        trajectoryAnimator = (TrajectoryAnimator)target;
    }

    public override void OnInspectorGUI()
    {
        // Dibujar el inspector por defecto
        DrawDefaultInspector();

        // Inicializar estilos
        InitializeStyles();

        // Separador
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Controles de Animación", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        // Verificar si está en modo Play
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Los controles solo funcionan en modo Play", MessageType.Info);
            GUI.enabled = false;
        }

        // Verificar referencias
        bool referenciasCompletas = trajectoryAnimator.projectileFollower != null &&
                                  trajectoryAnimator.animator != null;

        if (!referenciasCompletas && Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Faltan referencias: ProjectileFollower y/o Animator", MessageType.Warning);
        }

        // Botón principal - Iniciar Animación
        EditorGUILayout.BeginHorizontal();

        if (trajectoryAnimator.EstaAnimando())
        {
            // Si está animando, mostrar botón de detener
            if (GUILayout.Button("🛑 DETENER ANIMACIÓN", dangerButtonStyle, GUILayout.Height(40)))
            {
                trajectoryAnimator.DetenerAnimacion();
            }
        }
        else
        {
            // Si no está animando, mostrar botón de iniciar
            GUI.enabled = Application.isPlaying && referenciasCompletas;
            if (GUILayout.Button("🚀 INICIAR ANIMACIÓN", successButtonStyle, GUILayout.Height(40)))
            {
                trajectoryAnimator.IniciarAnimacion();
            }
        }

        EditorGUILayout.EndHorizontal();

        // Botones secundarios
        EditorGUILayout.Space(5);
        EditorGUILayout.BeginHorizontal();

        GUI.enabled = Application.isPlaying && referenciasCompletas;
        if (GUILayout.Button("🔄 Resetear Progreso", buttonStyle))
        {
            trajectoryAnimator.ResetearProgreso();
        }

        EditorGUILayout.EndHorizontal();

        // Información de estado en tiempo real
        if (Application.isPlaying)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Estado Actual", EditorStyles.boldLabel);

            // Estado general
            string estadoGeneral = trajectoryAnimator.EstaAnimando() ? "🔄 ANIMANDO" : "⏸️ DETENIDO";
            EditorGUILayout.LabelField("Estado:", estadoGeneral, EditorStyles.helpBox);

            if (trajectoryAnimator.EstaAnimando())
            {
                EditorGUILayout.Space(5);

                // Estado específico
                if (trajectoryAnimator.EstaEnEspera())
                {
                    EditorGUILayout.LabelField("Fase:", "⏳ Reduciendo tensión", EditorStyles.helpBox);

                    float tiempoRestante = trajectoryAnimator.ObtenerTiempoRestanteEspera();
                    EditorGUILayout.LabelField("Tiempo restante:", $"{tiempoRestante:F2}s");

                    float tensionActual = trajectoryAnimator.tensionActual;
                    float tensionInicial = trajectoryAnimator.tensionInicial;
                    EditorGUILayout.LabelField("Tensión:", $"{tensionActual:F2} (inicial: {tensionInicial:F2})");

                    // Barra de progreso para la tensión
                    if (tensionInicial > 0)
                    {
                        float progresoTension = 1f - (tensionActual / tensionInicial);
                        Rect rect = EditorGUILayout.GetControlRect();
                        EditorGUI.ProgressBar(rect, progresoTension, $"Reduciendo tensión: {progresoTension * 100:F1}%");
                    }
                }
                else if (trajectoryAnimator.EstaAnimandoTrayectoria())
                {
                    EditorGUILayout.LabelField("Fase:", "🎯 Animando trayectoria", EditorStyles.helpBox);

                    float tiempoRestante = trajectoryAnimator.ObtenerTiempoRestanteTrayectoria();
                    EditorGUILayout.LabelField("Tiempo restante:", $"{tiempoRestante:F2}s");

                    // Obtener progreso del proyectil si es posible
                    if (trajectoryAnimator.projectileFollower != null)
                    {
                        float progreso = trajectoryAnimator.projectileFollower.progreso;
                        Rect rect = EditorGUILayout.GetControlRect();
                        EditorGUI.ProgressBar(rect, progreso, $"Progreso: {progreso * 100:F1}%");
                    }
                }
            }

            // Actualizar la vista cada frame cuando está animando
            if (trajectoryAnimator.EstaAnimando())
            {
                EditorUtility.SetDirty(target);
                Repaint();
            }
        }

        // Separador final
        EditorGUILayout.Space(10);

        // Información de ayuda
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("💡 Presiona Play para usar los controles de animación", MessageType.Info);
        }
        else if (!referenciasCompletas)
        {
            EditorGUILayout.HelpBox("⚠️ Asigna ProjectileFollower y Animator para usar los controles", MessageType.Warning);
        }

        GUI.enabled = true;
    }

    private void InitializeStyles()
    {
        if (buttonStyle == null)
        {
            buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.fontSize = 11;
            buttonStyle.fontStyle = FontStyle.Bold;
        }

        if (largeButtonStyle == null)
        {
            largeButtonStyle = new GUIStyle(GUI.skin.button);
            largeButtonStyle.fontSize = 14;
            largeButtonStyle.fontStyle = FontStyle.Bold;
        }

        if (successButtonStyle == null)
        {
            successButtonStyle = new GUIStyle(GUI.skin.button);
            successButtonStyle.fontSize = 14;
            successButtonStyle.fontStyle = FontStyle.Bold;
            successButtonStyle.normal.textColor = Color.white;

            // Color verde para el botón de iniciar
            Texture2D greenTexture = MakeTexture(2, 2, new Color(0.2f, 0.7f, 0.2f, 1f));
            successButtonStyle.normal.background = greenTexture;
            successButtonStyle.hover.background = greenTexture;
            successButtonStyle.active.background = greenTexture;
        }

        if (warningButtonStyle == null)
        {
            warningButtonStyle = new GUIStyle(GUI.skin.button);
            warningButtonStyle.fontSize = 12;
            warningButtonStyle.fontStyle = FontStyle.Bold;
            warningButtonStyle.normal.textColor = Color.white;

            // Color naranja para botones de advertencia
            Texture2D orangeTexture = MakeTexture(2, 2, new Color(0.9f, 0.6f, 0.1f, 1f));
            warningButtonStyle.normal.background = orangeTexture;
            warningButtonStyle.hover.background = orangeTexture;
            warningButtonStyle.active.background = orangeTexture;
        }

        if (dangerButtonStyle == null)
        {
            dangerButtonStyle = new GUIStyle(GUI.skin.button);
            dangerButtonStyle.fontSize = 14;
            dangerButtonStyle.fontStyle = FontStyle.Bold;
            dangerButtonStyle.normal.textColor = Color.white;

            // Color rojo para el botón de detener
            Texture2D redTexture = MakeTexture(2, 2, new Color(0.8f, 0.2f, 0.2f, 1f));
            dangerButtonStyle.normal.background = redTexture;
            dangerButtonStyle.hover.background = redTexture;
            dangerButtonStyle.active.background = redTexture;
        }
    }

    private Texture2D MakeTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = color;

        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();
        return texture;
    }
}