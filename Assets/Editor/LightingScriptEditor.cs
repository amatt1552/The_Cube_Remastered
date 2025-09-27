using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LightingSetter))]

public class LightingScriptEditor : Editor 
{

	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();
		LightingSetter myLightingSetter = (LightingSetter)target;
		if(GUILayout.Button("Update"))
		{
			myLightingSetter.UpdateSettings();
		}
		if (GUILayout.Button("CopyCurrentSettings"))
		{
			myLightingSetter.CopyCurrentSettings();
		}
		if (GUILayout.Button("FindSun"))
		{
			myLightingSetter.FindSun();
		}
	}
}
