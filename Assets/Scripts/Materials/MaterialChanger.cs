using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MaterialChanger : MonoBehaviour
{
	public bool setColor = true;
	public string targetColorLocation = "_Color";
	public Color materialColor = Color.white;

	public bool setTexture = true;
	public string targetTextureLocation = "_MainTex";
	public Texture texture;

	MeshRenderer meshRenderer;
	private void Awake()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		SetMaterial();
	}

	private void SetMaterial()
	{
		if(meshRenderer != null)
		{

			if(setColor)
				meshRenderer.material.SetColor(targetColorLocation, materialColor);
			if(setTexture)
				meshRenderer.material.SetTexture(targetTextureLocation, texture);
		}
	}
}
