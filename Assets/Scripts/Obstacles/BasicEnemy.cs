using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]

public class BasicEnemy : MonoBehaviour
{
	public WaypointNav nav;
	//lets me change how much the object rotates. 
	float scaledExtraRotation = 2;
	Rigidbody _rb;
	float currentAngle;
	
	void Awake()
    {
		_rb = GetComponent<Rigidbody>();
		if (nav == null)
		{
			nav = GetComponent<WaypointNav>();
		}
    }
	
    void FixedUpdate()
    {
		if (nav != null)
		{
			if (!nav.Waiting())
			{

				//my way of converting a vector direction into a float direction. I smell extention method >:D
				float guessedDirection = nav.GetCurrentDirection().x + nav.GetCurrentDirection().y + nav.GetCurrentDirection().z;
				if (guessedDirection != 0)
				{
					guessedDirection = guessedDirection / Mathf.Abs(guessedDirection);
				}
				else
				{
					guessedDirection = 0;
				}
				//the rotation
				transform.Rotate(Vector3.back * guessedDirection, nav.GetCurrentSpeed() * scaledExtraRotation);
			}
		}
	}
}
