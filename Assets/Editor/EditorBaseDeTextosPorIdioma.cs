#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseDeTextosPorIdioma))]
public class EditorBaseDeTextosPorIdioma : Editor
{
    SerializedProperty csvProp;
    SerializedProperty separadorProp;

    // Props privados serializados dentro del SO
    SerializedProperty idiomasProp;
    SerializedProperty datosPorIdiomaProp;

    bool mostrarDatos = true;
    bool modoPreview = true;
    int maxPreviewPorIdioma = 15;

    void OnEnable()
    {
        csvProp = serializedObject.FindProperty("csv");
        separadorProp = serializedObject.FindProperty("separador");

        // Nombres exactos de los campos en el ScriptableObject
        idiomasProp = serializedObject.FindProperty("idiomas");
        datosPorIdiomaProp = serializedObject.FindProperty("datosPorIdioma");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        BaseDeTextosPorIdioma so = (BaseDeTextosPorIdioma)target;

        // --------- Seccion CSV ----------
        EditorGUILayout.LabelField("Fuente CSV", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(csvProp, new GUIContent("CSV"));

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Configuracion", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(separadorProp, new GUIContent("Separador"));

        EditorGUILayout.Space(10);

        // --------- Boton actualizar ----------
        using (new EditorGUI.DisabledScope(so.csv == null))
        {
            if (GUILayout.Button("Actualizar arreglos desde CSV"))
            {
                ActualizarDesdeCsv(so);
            }
        }

        if (so.csv == null)
        {
            EditorGUILayout.HelpBox("Asigna un TextAsset CSV para poder actualizar los arreglos.", MessageType.Info);
        }

        EditorGUILayout.Space(10);

        // --------- Mostrar datos generados ----------
        mostrarDatos = EditorGUILayout.Foldout(mostrarDatos, "Ver datos generados", true);
        if (mostrarDatos)
        {
            // Resumen
            int cantidadIdiomas = (idiomasProp != null) ? idiomasProp.arraySize : 0;
            int cantidadBloques = (datosPorIdiomaProp != null) ? datosPorIdiomaProp.arraySize : 0;

            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("Idiomas detectados", cantidadIdiomas.ToString());
            EditorGUILayout.LabelField("Bloques por idioma", cantidadBloques.ToString());
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(6);

            // Controles de visualizacion
            EditorGUILayout.BeginHorizontal();
            modoPreview = EditorGUILayout.ToggleLeft("Modo preview", modoPreview, GUILayout.Width(120));
            using (new EditorGUI.DisabledScope(!modoPreview))
            {
                maxPreviewPorIdioma = EditorGUILayout.IntField("Max textos", maxPreviewPorIdioma);
                if (maxPreviewPorIdioma < 1) maxPreviewPorIdioma = 1;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(6);

            if (datosPorIdiomaProp == null)
            {
                EditorGUILayout.HelpBox("No se encontro la propiedad 'datosPorIdioma'. Verifica el nombre del campo en el ScriptableObject.", MessageType.Warning);
            }
            else if (datosPorIdiomaProp.arraySize == 0)
            {
                EditorGUILayout.HelpBox("Aun no hay datos generados. Pulsa 'Actualizar arreglos desde CSV'.", MessageType.Info);
            }
            else
            {
                // Mostrar cada idioma como foldout
                for (int i = 0; i < datosPorIdiomaProp.arraySize; i++)
                {
                    SerializedProperty item = datosPorIdiomaProp.GetArrayElementAtIndex(i);
                    if (item == null) continue;

                    SerializedProperty idioma = item.FindPropertyRelative("idioma");
                    SerializedProperty textos = item.FindPropertyRelative("textos");

                    string nombreIdioma = (idioma != null && !string.IsNullOrEmpty(idioma.stringValue))
                        ? idioma.stringValue
                        : $"idioma_{i}";

                    int cantidadTextos = (textos != null) ? textos.arraySize : 0;

                    // Foldout por idioma (estado persistente por sesion con EditorPrefs no necesario)
                    item.isExpanded = EditorGUILayout.Foldout(
                        item.isExpanded,
                        $"{nombreIdioma}  ({cantidadTextos} textos)",
                        true
                    );

                    if (!item.isExpanded) continue;

                    EditorGUILayout.BeginVertical("box");

                    // Mostrar campo idioma (solo lectura opcional)
                    using (new EditorGUI.DisabledScope(true))
                    {
                        if (idioma != null) EditorGUILayout.PropertyField(idioma);
                    }

                    if (textos == null)
                    {
                        EditorGUILayout.HelpBox("No se encontro el arreglo 'textos' en este idioma.", MessageType.Warning);
                    }
                    else
                    {
                        if (!modoPreview)
                        {
                            // Mostrar arreglo completo (puede ser pesado)
                            EditorGUILayout.PropertyField(textos, new GUIContent("Textos"), true);
                        }
                        else
                        {
                            // Preview: mostramos solo los primeros N
                            int mostrar = Mathf.Min(cantidadTextos, maxPreviewPorIdioma);

                            EditorGUILayout.LabelField("Textos (preview)", EditorStyles.boldLabel);

                            for (int t = 0; t < mostrar; t++)
                            {
                                SerializedProperty txt = textos.GetArrayElementAtIndex(t);
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField($"[{t}]", GUILayout.Width(38));
                                EditorGUILayout.SelectableLabel(
                                    txt != null ? (txt.stringValue ?? "") : "",
                                    GUILayout.Height(EditorGUIUtility.singleLineHeight)
                                );
                                EditorGUILayout.EndHorizontal();
                            }

                            if (cantidadTextos > mostrar)
                            {
                                EditorGUILayout.HelpBox(
                                    $"Mostrando {mostrar} de {cantidadTextos}. Desactiva 'Modo preview' para ver todo.",
                                    MessageType.None
                                );
                            }
                        }
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space(6);
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }

    void ActualizarDesdeCsv(BaseDeTextosPorIdioma so)
    {
        Undo.RecordObject(so, "Actualizar arreglos desde CSV");
        so.ConvertirCsvAArreglos();

        EditorUtility.SetDirty(so);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // Sincronizar SerializedObject despues de cambios
        serializedObject.Update();

        Debug.Log("BaseDeTextosPorIdiomaSO: arreglos actualizados desde CSV.");
    }
}
#endif
