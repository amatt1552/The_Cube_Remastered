using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAction : ActionQueueBase
{
	public CameraFollow cameraFollow;
	protected override void Awake()
	{
		base.Awake();


	}
	protected override void FixedUpdate()
	{
		base.FixedUpdate();

	}

	protected override void MoveTo(Transform targetPosition, float speed)
	{

		if (cameraFollow != null)
		{
			if (targetPosition != null)
			{
				cameraFollow.SnapToObject(false);
				cameraFollow.cameraSpeed = speed;
				cameraFollow.SetOffset(currentDirection);
				cameraFollow.offsetRotation = targetRotation;
				cameraFollow.FollowedObject = targetPosition.gameObject;
			}
			else
			{
				Debug.Log("Camera reached target.");
				cameraFollow.FollowedObject = null;
				rb.velocity = Vector3.zero;
			}
		}
		
	}
}
