using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PingPong : MonoBehaviour
{
	public string targetMaterialLocation = "_Color";
	Color currentColor;
	public float speed = 1;
	public float minPong = 0.2f;
	public float maxPong = 1;
	MeshRenderer meshRenderer;

	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		SetMaterial();
	}

	private void SetMaterial()
	{
		if (meshRenderer != null)
		{
			currentColor = meshRenderer.material.GetColor(targetMaterialLocation);
			currentColor.a = minPong + Mathf.PingPong(Time.time * speed, maxPong - minPong);
			meshRenderer.material.SetColor(targetMaterialLocation, currentColor);
		}
	}
}
