using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
	[Tooltip("its default position")]
	public Vector3 defaultOffset = Vector3.zero;
	static Vector3 zoomOffset;

	[Tooltip("its default rotation")]
	public Vector3 defaultRotation;
	[HideInInspector]
	public Vector3 offsetRotation;

	Vector3 offset;

	public GameObject FollowedObject;
	bool smoothMovement;
	public float cameraSpeed = 1;
	public float rotateSpeed = 25;
	public bool followObject = true;
	const float DISTANCE_TO_POINT = 0.1f;
	bool snapingToObject; //makes sure it always goes to point before smooth movement
	public bool inverted;
	public float sensitivity;

	public int minZoom = -12;
	public int maxZoom = -5;

	//public delegate void OnTransformChanged();
	//public event OnTransformChanged TransformChanged;

	void Start ()
	{
		if (FollowedObject == null)
		{
			FollowedObject = TheCubeGameManager.player;
		}
	}
	
	void Update ()
	{
		
		//if (transform.hasChanged)
		//{
			//print("I... have.. changed.");
		//	TransformChanged?.Invoke();
		//}
		
		//SnapToObjectCheck();
		FollowObject();
	}

	public void SetToDefault()
	{
		offset = Vector3.zero;
	}

	public void FollowObject()
	{
		//Debug.Log("zoom" + zoomOffset);
		
		/*if (Vector3.Distance(transform.position, targetPosition) < DISTANCE_TO_POINT)
		{
			_smoothMovement = true;
		}
		else if (Vector3.Distance(transform.position, targetPosition) > DISTANCE_TO_POINT + 1f)
		{
			_smoothMovement = false;
		}
		*/
		if (followObject && FollowedObject != null)
		{
			Vector3 targetPosition = FollowedObject.transform.position + defaultOffset + offset + zoomOffset;

			if (smoothMovement)
			{
				//Debug.Log("too slow. >:(");
				transform.position = Vector3.MoveTowards(transform.position, targetPosition, cameraSpeed * Time.deltaTime);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(defaultRotation + offsetRotation), rotateSpeed * Time.deltaTime);
			}
			else
			{
				//Debug.Log("too fast!");
				transform.position = targetPosition;
				transform.rotation = Quaternion.Euler(defaultRotation + offsetRotation);
			}
		}
	}

	//zooming
	/// <summary>
	/// This zooms based on a direction. has no limits as of now.
	/// </summary>
	/// <param name="zoom"></param>
	/// <param name="direction"></param>
	public void Zoom(float zoom, Vector3 direction)
	{
		zoom *= sensitivity;
		zoomOffset += zoom * direction;
	}
	/// <summary>
	/// This zooms based on the z. has limits.
	/// </summary>
	/// <param name="zoom"></param>
	public void Zoom(float zoom)
	{
		//Makes sure this only runs when necessary
		if (zoom != 0)
		{
			zoom *= sensitivity;
			if (zoomOffset.z + zoom + defaultOffset.z > maxZoom)
			{
				zoomOffset.z = maxZoom - defaultOffset.z;
				return;
			}
			if (zoomOffset.z + zoom + defaultOffset.z < minZoom)
			{
				zoomOffset.z = minZoom - defaultOffset.z ;
				return;
			}
		}
		zoomOffset.z += !inverted ? zoom : -zoom;
	}

	public void SetZoom(Vector3 zoom)
	{
		zoomOffset = zoom;
	}

	public void SetOffset(float x = 0, float y = 0, float z = 0)
	{
		offset.x = x;
		offset.y = y;
		offset.z = z;
	}
	public void SetOffset(Vector3 newOffset)
	{
		offset = newOffset;
	}

	public void SnapToObject(bool snap)
	{
		smoothMovement = !snap;
	}

	void SnapToObjectCheck()
	{
		Vector3 targetPosition = FollowedObject.transform.position + defaultOffset + offset;
		
		if (Vector3.Distance(transform.position, targetPosition) <= 0.01f)
		{
			snapingToObject = false;
		}
		if(snapingToObject)
		{
			transform.position = targetPosition;
		}
	}
}
