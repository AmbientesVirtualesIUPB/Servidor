using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AtadorACuerpo))]
public class AtadorACuerpoEditor : Editor
{
    SerializedProperty modo;
    SerializedProperty parteCuerpo;
    SerializedProperty suavizado;
    SerializedProperty seguirRotacion;
    SerializedProperty offsetPosicion;
    SerializedProperty offsetRotacion;

    private void OnEnable()
    {
        modo = serializedObject.FindProperty("modo");
        parteCuerpo = serializedObject.FindProperty("parteCuerpo");
        suavizado = serializedObject.FindProperty("suavizado");
        seguirRotacion = serializedObject.FindProperty("seguirRotacion");
        offsetPosicion = serializedObject.FindProperty("offsetPosicion");
        offsetRotacion = serializedObject.FindProperty("offsetRotacion");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Atador a Cuerpo VR", EditorStyles.boldLabel);
        EditorGUILayout.Space(5);

        EditorGUILayout.PropertyField(modo);
        EditorGUILayout.PropertyField(parteCuerpo);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Offsets", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(offsetPosicion);
        EditorGUILayout.PropertyField(offsetRotacion);

        if ((AtadorACuerpo.ModoAtado)modo.enumValueIndex == AtadorACuerpo.ModoAtado.Seguir)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Seguimiento", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(suavizado);
            EditorGUILayout.PropertyField(seguirRotacion);
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Modo Emparentar: el objeto se vuelve hijo en Start.",
                MessageType.Info
            );
        }

        serializedObject.ApplyModifiedProperties();
    }
}
