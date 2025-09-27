using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(ParticleSystem))]
[ExecuteInEditMode]
public class Launcher : MonoBehaviour
{
	//public stuff for figuring out the force for the launcher

	[Range(1,89)]
	public float angle = 45;
	public Transform targetPosition;
	public Vector3 direction;

	//only want targetZ to be set in inspector

	[Tooltip("if set to -1 will be the distance's z")]
	[SerializeField]
	float targetZ = -1;
	private float _velocity;

	//particle stuff

	public ParticleSystem particlesForVisualiztion;
	ParticleSystem.VelocityOverLifetimeModule particlesVOLM;

	//mostly for rotation
	bool enumOneShot;
	public Collider launchCollider;
	Quaternion targetRotation;
	public GameObject rotatedObject;
	const float WAIT_TIME = 1;

	//launchers activating without jump

	public bool activateOnJump = true;

	private void Awake()
	{
		//normally I wouldn't bother with the null check with a RequireComponent. I've been
		//getting errors sometimes though, so I need to check it anyway to prevent them.
		//might get rid of the RequireComponent
		if (particlesForVisualiztion == null)
		{
			particlesForVisualiztion = GetComponent<ParticleSystem>();
		}
		if (particlesForVisualiztion != null)
		{
			particlesVOLM = particlesForVisualiztion.velocityOverLifetime;
			particlesForVisualiztion.Play();
		}

		GetDistanceZ();
	}
	private void Update()
	{
		//for editor stuff
		if (particlesForVisualiztion != null)
		{
			if (Application.isPlaying)
			{

				particlesForVisualiztion.Clear();
				particlesForVisualiztion.Stop();
			}
			else
			{
				particlesVOLM = particlesForVisualiztion.velocityOverLifetime;
				CalculateForce();
			}
		}
	}

	private void FixedUpdate()
	{
		CalculateForce();
		rotatedObject.transform.localRotation = Quaternion.RotateTowards(rotatedObject.transform.localRotation, targetRotation, 10);
	}

	void CalculateForce()
	{
		float gravity = Physics.gravity.magnitude;
		Vector3 distance = targetPosition.position - transform.position;
		
		float distanceY = -distance.y;
		distance.y = 0;
		float distanceXZ = distance.magnitude;
		
		float angleRadians = angle * Mathf.Deg2Rad;
		
		//https://answers.unity.com/questions/1353777/calculate-initial-velocity-given-distance-gravity.html
		//velocity = (1/ Mathf.Cos(angle * Mathf.Deg2Rad))*Mathf.Sqrt(0.5F*distanceX*distanceX* gravity /(distanceY + Mathf.Tan(angle * Mathf.Deg2Rad) * distanceX));
		_velocity = (1 / Mathf.Cos(angleRadians)) * Mathf.Sqrt((0.5f * distanceXZ * distanceXZ * gravity) / (distanceY + Mathf.Tan(angleRadians) * distanceXZ));
		//cos(θ) = Adjacent / Hypotenuse. I want Adjacent.
		float directionX = Mathf.Cos(angleRadians) * _velocity;
		//sin(θ) = Opposite / Hypotenuse. I want Opposite.
		float directionY = Mathf.Sin(angleRadians) * _velocity;
		//just guessing with the Z...
		float directionZ = Mathf.Cos(angleRadians) * _velocity;
		
		Vector3 normalizedDistance = distance.normalized;
		//lets me get the direction just for the x and z
		normalizedDistance.y = 1;
		direction = new Vector3(directionX, directionY, directionZ).Multiply(normalizedDistance);
		//sets direction for ParticleSystem
		particlesVOLM.x = direction.x;
		particlesVOLM.y = direction.y;
		particlesVOLM.z = direction.z;
	}

	public void Launch(GameObject launchedObject)
	{
		Rigidbody rb = launchedObject.GetComponent<Rigidbody>();
		
		if (launchedObject != null)
		{
			rb.velocity = direction;
		}

		if (!enumOneShot)
		{
			//disables collider just to be sure it doesn't mess with the force on player
			launchCollider.enabled = false;
			//sets new target rotation
			targetRotation = Quaternion.Euler(-angle, 0, 0);
			StartCoroutine("Wait", WAIT_TIME);
			enumOneShot = true;
		}
	}
	IEnumerator Wait(float time)
	{
		yield return new WaitForSeconds(time);
		targetRotation = Quaternion.identity;
		launchCollider.enabled = true;
		enumOneShot = false;
	}

	public float GetDistance()
	{
		Vector3 distance = targetPosition.position - transform.position;
		distance.y = 0;
		return distance.magnitude;
	}

	public float GetDistanceZ()
	{
		Vector3 distance = targetPosition.position - transform.position;
		if (targetZ == -1)
		{
			targetZ = distance.z;
		}
		return targetZ;
	}
	/*
	Vector3[] CalculateArc()
	{
		Vector3[] arcArray = new Vector3[resolution + 1];
		gravity = Physics.gravity.magnitude;

		radianAngle = angle * Mathf.Deg2Rad;
		float maxDistance = (velocity * velocity * Mathf.Sin(2 * radianAngle)) / gravity;
		for (int i = 0; i <= resolution; i++)
		{
			float t = (float)i / resolution;
			arcArray[i] = CalculateArcPoint(t, maxDistance);
		}

		return arcArray;
	}

	Vector3 CalculateArcPoint(float t, float maxDistance)
	{
		float x = t * maxDistance;
		float y = x * Mathf.Tan(radianAngle) - ((gravity * x * x) / (2 * velocity * velocity * Mathf.Cos(radianAngle) * Mathf.Cos(radianAngle)));
		return new Vector3(x, y);
	}
	*/

}
