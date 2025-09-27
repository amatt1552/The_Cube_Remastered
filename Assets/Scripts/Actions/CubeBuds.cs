using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

//overrided ActionQueueBase

public class CubeBuds : ActionQueueBase
{
	bool flipDone;

	protected override void Awake()
	{
		base.Awake();
		
		
	}
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		
	}


	protected override void Flip(Vector3 direction, float speed, float animSpeed)
	{
		if (currentAnimation != null && !flipDone)
		{
			currentAnimation.enabled = true;
			currentAnimation.SetBool("Start", true);
			currentAnimation.speed = animSpeed;
			flipDone = true;
		}
	}

	

}
