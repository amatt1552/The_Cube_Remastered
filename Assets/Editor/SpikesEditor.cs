using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Spikes))]

public class SpikesEditor : Editor
{
	public override void OnInspectorGUI()
	{

		DrawDefaultInspector();
		Spikes mySpikes = (Spikes)target;
		if (GUILayout.Button("Generate"))
		{
			mySpikes.RegenerateSpikes();
		}
	}
}
