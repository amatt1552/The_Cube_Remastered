
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider))]
public class BoxCollisionHelp : MonoBehaviour
{
	public BoxCollider boxCollider;

	[Header("Just a little debugging.")]
	public bool debugging = true;

	//gizmo vars
	
	public Color gizmoBoxColor = Color.white;
	Vector3 _gizmoBoxCenter;
	Vector3 _gizmoBoxSize;

	void Start()
	{
		boxCollider = GetComponent<BoxCollider>();
	}
	/// <summary>
	/// checks if its colliding with anything in a direction based on a distance.
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="layerMask"></param>
	/// <param name="bias"></param>
	/// <param name="distance"></param>
	/// <returns></returns>
	public bool CollidingFromDirection(Vector3 direction, LayerMask layerMask, float bias = 1, float extraDistance = 0.01f, QueryTriggerInteraction triggerActive = QueryTriggerInteraction.Ignore)
	{
		
		RaycastHit hit;
		direction.Normalize();
		Vector3 size = boxCollider.size;
		
		//get size in direction then sets to distance.
		//an example.
		// direction is (1,0,0) size is (3,2,2).
		//after multiplying each xyz to the other xyz the new vector will be  (3,0,0)
		//you get the magnitude * 0.5 of that  and should get 1.5f
		float distance = (direction.Multiply(size).magnitude * 0.5f) + extraDistance;

		//makes BoxCast sort of flat in whatever direction it is
		size.x = direction.x != 0 ? 0 : size.x * bias;
		size.y = direction.y != 0 ? 0 : size.y * bias;
		size.z = direction.z != 0 ? 0 : size.z * bias;

		Physics.BoxCast(transform.position, size / 2, direction, out hit, boxCollider.transform.rotation, distance * 4, layerMask, triggerActive);
		if (debugging)
		{
			//set up an extention method later for multiplying two vectors..
			//the center is a bit complicated for debugging.. not sure how to explain yet also needs one more thing...
			_gizmoBoxCenter = transform.position + direction * distance / 2; //new Vector3(direction.x * _boxCollider.size.x/2, direction.y * _boxCollider.size.y/2, direction.z * _boxCollider.size.z/2);
			
			//sees which direction is "flat" in Boxcast then set the size of that direction to the distance * 2
			size.x = size.x == 0 ? distance : size.x;
			size.y = size.y == 0 ? distance : size.y;
			size.z = size.z == 0 ? distance : size.z;
			
			_gizmoBoxSize = size;
		}
		if (hit.collider != null)
		{
			return hit.distance <= distance;
		}
		else
		{
			return false;
		}
		
	}

	public bool CollidingFromDirection(Vector3 centerOffset, Vector3 direction, LayerMask layerMask, float bias = 1, float extraDistance = 0.01f)
	{

		RaycastHit hit;
		direction.Normalize();
		Vector3 size = boxCollider.size;

		//get size in direction then sets to distance.
		//an example.
		// direction is (1,0,0) size is (3,2,2).
		//after multiplying each xyz to the other xyz the new vector will be  (3,0,0)
		//you get the magnitude * 0.5 of that  and should get 1.5f
		float distance = (direction.Multiply(size).magnitude * 0.5f) + extraDistance;

		//makes BoxCast sort of flat in whatever direction it is
		size.x = direction.x != 0 ? 0 : size.x * bias;
		size.y = direction.y != 0 ? 0 : size.y * bias;
		size.z = direction.z != 0 ? 0 : size.z * bias;

		Physics.BoxCast(transform.position + centerOffset, size / 2, direction, out hit, boxCollider.transform.rotation, distance * 4, layerMask);
		if (debugging)
		{
			
			//the center is a bit complicated for debugging.. not sure how to explain yet also needs one more thing...
			_gizmoBoxCenter = transform.position + centerOffset + direction * distance / 2; //new Vector3(direction.x * _boxCollider.size.x/2, direction.y * _boxCollider.size.y/2, direction.z * _boxCollider.size.z/2);

			//sees which direction is "flat" in Boxcast then set the size of that direction to the distance * 2
			size.x = size.x == 0 ? distance : size.x;
			size.y = size.y == 0 ? distance : size.y;
			size.z = size.z == 0 ? distance : size.z;

			_gizmoBoxSize = size;
		}
		if (hit.collider != null)
		{
			return hit.distance <= distance;
		}
		else
		{
			return false;
		}

	}

	void OnDrawGizmosSelected()
	{
		if (transform.hasChanged)
		{
			Gizmos.color = gizmoBoxColor;
			Gizmos.DrawCube(_gizmoBoxCenter, _gizmoBoxSize);
		}
	}
}
