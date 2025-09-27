using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerAction : ActionQueueBase
{
	const float RESET_ANGLE = 5;
	public AudioSource whooshSource;
	protected override void Awake()
    {
		base.Awake();
    }
	
	protected override void FixedUpdate()
    {
		base.FixedUpdate();
    }

	protected override void RotateTo(Vector3 targetAngle, float speed)
	{
		base.RotateTo(targetAngle, speed);
		float angle;
		

		Vector3 baseDirection = -transform.root.up;
		currentDirection = -transform.up;
		angle = Vector3.Angle(currentDirection, baseDirection);

		
		if (angle < RESET_ANGLE && whooshSource != null)
		{
			if (!whooshSource.isPlaying)
			{
				whooshSource.Play();
			}
			
		}
	}
}
