using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideMesh : MonoBehaviour
{
	[Header("If both true mesh will be hidden on Awake")]
	public bool showAwake;
	public bool hideAwake = true;
	public MeshRenderer mesh;
	bool foundMesh = true;
    void Awake()
    {
		if (mesh == null)
		{
			TrySetMesh(GetComponent<MeshRenderer>());
		}
		
		if (showAwake)
		{
			Reveal();
		}
		if (hideAwake)
		{
			Hide();
		}
	}

	public void Hide()
	{
		if (foundMesh)
		{
			mesh.enabled = false;
			
		}
	}

	public void Reveal()
	{
		if (foundMesh)
		{
			mesh.enabled = true;
		}
	}

	public void TrySetMesh(MeshRenderer meshRenderer)
	{
		if (meshRenderer != null)
		{
			mesh = meshRenderer;
			foundMesh = true;
		}
		else
		{
			foundMesh = false;
		}
	}
}
