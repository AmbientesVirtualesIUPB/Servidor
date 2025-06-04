using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MorionSubID))]

public class MorionSubIDEditor : Editor
{
	public override void OnInspectorGUI()
	{

		base.OnInspectorGUI();
		MorionSubID mId = (MorionSubID)target;
		GUIStyle estilo = new GUIStyle();
		estilo.richText = true;
		estilo.normal.textColor = Color.white;
		GUILayout.Label("");
		GUILayout.Label("ID: <color=#009000> " + mId.GetID() + "</color>", estilo);

		if (GUILayout.Button("Generar ID"))
		{
			mId.GenerarID();
		}
	}
}
