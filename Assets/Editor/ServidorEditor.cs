using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Servidor))]
public class ServidorEditor : Editor
{
	public override void OnInspectorGUI()
	{

		base.OnInspectorGUI();
		Servidor s = (Servidor)target;
		GUIStyle estilo = new GUIStyle();
		estilo.richText = true;
		estilo.normal.textColor = Color.white;
		GUILayout.Label("Servidor: " + s.estado, estilo);
		GUILayout.Label("                " + s.GetURL());
		if (GUILayout.Button("Conectar"))
		{
			s.Conectar();
		}
		if (GUILayout.Button("Configuración Default"))
		{
			Selection.activeObject = AssetDatabase.LoadAssetAtPath("Assets/Resources/ConfiguracionGeneral.asset", typeof(ConfiguracionGeneral));
		}
	}
}
