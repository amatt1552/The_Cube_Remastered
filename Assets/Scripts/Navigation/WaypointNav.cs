using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointNav : MonoBehaviour
{
	public Waypoints waypoints;

	public float[] speed = new float[2] { 1, 1 };
	public float[] waitTime = new float[2] { 1, 1 };
	//public float[] slowDownScale = new float[2] { 1, 1 };
	public float[] slowDownDistance = new float[2] {1.5f, 1.5f};
	public int indexOfTarget = 0;

	//float speedChangeScale = 0.5f;
	float currentSpeed;
	Rigidbody rb;
	bool waiting = false;
	int difficulty;
	Vector3 targetDirection;

	//used to find acceleration
	float speedBeforeSlowingDown;
	float distanceBeforeSlowingDown;

	const float CLOSE_TO = 0.05f;
	const float extraAcceleration = 0.2f;
	
	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		difficulty = SaveGameManager.difficulty;
		if (waypoints != null)
		{
			targetDirection = GetCurrentDirection();
		}
	}

	void FixedUpdate()
	{
		if (rb != null && waypoints != null)
		{
			
			MoveRb();
		}
	}

	//rigidbody movement
	void MoveRb()
	{ 
		float distance = Vector3.Distance(transform.position, waypoints.connectedPoints[indexOfTarget].position); //calculates the distance from the target point and the gameObject
		
		if ((distance < CLOSE_TO) && !waiting) //if its close to the target point it waits then changes the target to the next point.
		{
			currentSpeed = 0;

			StartCoroutine("Wait");
			MoveToNext();

		}
		else if (!waiting) //if its not waiting then move again.
		{
			if (distance > slowDownDistance[difficulty])
			{
				//accelerate 
				if (currentSpeed < speed[difficulty])
				{
					currentSpeed += speed[difficulty] * Time.deltaTime;
				}
				else
				{
					currentSpeed = speed[difficulty];
				}
				speedBeforeSlowingDown = currentSpeed;
				distanceBeforeSlowingDown = distance;
			}
			else
			{
				//calculate the acceleration required to slow down the object based on its speed and target distance.
				
				//first I need time. speed = distance/time, so time = distance/speed
				float time = 0;
				float acceleration = 0;
				if (currentSpeed != 0)
				{
					time = distanceBeforeSlowingDown / currentSpeed;
				}
				else
				{
					time = 0;
				}
				
				//then I get the acceleration. acceleration = the change in velocity over the change in time
				if (time != 0)
				{
					acceleration = -speedBeforeSlowingDown / time;
				}
				else
				{
					acceleration = 0;
				}

				//set current speed
				if (currentSpeed > 0)
				{
					currentSpeed += (acceleration + extraAcceleration) * Time.deltaTime;
				}
				else
				{
					currentSpeed = 0;
					if (!waiting)
					{
						StartCoroutine("Wait");
						MoveToNext();
					}
				}
			}

		}

		//move the object
		rb.velocity = targetDirection * currentSpeed;
	}

	//waiting.
	IEnumerator Wait()
	{
		waiting = true;
		yield return new WaitForSecondsRealtime(waitTime[difficulty]);
		waiting = false;
	}

	//changes the target to the next point.
	void MoveToNext()
	{
		indexOfTarget++;
		if (indexOfTarget >= waypoints.connectedPoints.Length)
		{
			indexOfTarget = 0;
		}
		//figure out which direction its going so i can calculate my movement correctly.
		targetDirection = GetCurrentDirection();
	}

	//get functions

	public Vector3 GetCurrentDirection()
	{
		Vector3 direction = waypoints.connectedPoints[indexOfTarget].position - transform.position;
		return direction.normalized;
	}

	public Vector3 GetCurrentDirectionWithSpeed()
	{
		Vector3 direction = waypoints.connectedPoints[indexOfTarget].position - transform.position;
		if (direction.magnitude > 0)
		{
			return direction.normalized * currentSpeed * Time.deltaTime;
		}
		else if (waiting)
		{
			return Vector3.zero;
		}
		return Vector3.zero;

	}

	public float GetCurrentSpeed()
	{
		return currentSpeed;
	}

	public Vector3 GetVelocity()
	{
		if (rb != null)
		{
			return rb.velocity;
		}
		return Vector3.zero;
	}

	public bool Waiting()
	{

		return waiting;
	}
	
}
