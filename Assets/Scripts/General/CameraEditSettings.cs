using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class CameraEditSettings : MonoBehaviour
{
	public enum Shape
	{
		sphere,
		box,
		custom
	}
	[Tooltip("if using custom you add your own colliders.")]
	public Shape currentShape;
	public float radius = 1;
	float newRadius;
	//add editor script to hide more later
	public Vector3 boxSize = Vector3.one;
	Vector3 newBoxSize = Vector3.one;
	public Vector3 newCameraOffset;
	[Tooltip("If true setting values of radius and boxsize will have no effect on collider")]
	public bool editFromCollider;

	BoxCollider _boxCollider;
	SphereCollider _sphereCollider;
	private void Awake()
	{
		newRadius = radius - 1;
		newBoxSize = boxSize - Vector3.one;
	}
	void OnGUI()
    {
		switch (currentShape)
		{
			case Shape.sphere:
				if (_boxCollider != null)
				{
					DestroyImmediate(_boxCollider);
				}
				if (_sphereCollider == null)
				{
					_sphereCollider = GetComponent<SphereCollider>();
					if (_sphereCollider == null)
					{
						_sphereCollider = gameObject.AddComponent<SphereCollider>();
						_sphereCollider.isTrigger = true;
					}
				}
				if (!editFromCollider && radius != newRadius)
				{
					_sphereCollider.radius = radius;
				}
				
				break;

			case Shape.box:
				if (_sphereCollider != null)
				{
					DestroyImmediate(_sphereCollider);
				}
				if (_boxCollider == null)
				{
					_boxCollider = GetComponent<BoxCollider>();
					if (_boxCollider == null)
					{
						_boxCollider = gameObject.AddComponent<BoxCollider>();
						_boxCollider.isTrigger = true;
					}
				}
				if (!editFromCollider && boxSize != newBoxSize)
				{
					_boxCollider.size = boxSize;
					newBoxSize = boxSize;
				}

				break;

			case Shape.custom:
				break;
		}
    }
}
