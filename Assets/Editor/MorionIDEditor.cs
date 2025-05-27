using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MorionID))]

public class MorionIDEditor : Editor
{
	public override void OnInspectorGUI()
	{

		base.OnInspectorGUI();
		MorionID mId = (MorionID)target;
		GUIStyle estilo = new GUIStyle();
		estilo.richText = true;
		estilo.normal.textColor = Color.white;
		GUILayout.Label("");
		GUILayout.Label("ID: <color=#009000> " + mId.ID + "</color>", estilo);

		if (GUILayout.Button("Generar ID"))
		{
			mId.GenerarID();
		}
	}
}
