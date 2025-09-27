using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction : ActionQueueBase
{
	protected override void Awake()
	{ 
		base.Awake();
	}
	protected override void FixedUpdate()
	{
		base.FixedUpdate();
	}
	protected override void Move(Vector3 direction, float speed)
	{
		base.Move(direction, speed);
		transform.Rotate(Vector3.forward * direction.magnitude, speed);
		
	
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Hammer")
		{
			CancelQueue();
			rb.constraints = RigidbodyConstraints.None;
			TheCubeGameManager.PlayMenu();
		}
	}

}
