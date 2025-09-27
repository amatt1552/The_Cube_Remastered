using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Smasher : MonoBehaviour
{
	enum SmasherState
	{
		Smashing, Retracting, Waiting, Offset
	}
	SmasherState smasherState;

	Rigidbody _rb;
	public Collider killingCollider;
	public Transform topTransform, bottomTransform;
	public AudioSource smashSource;

	const float SMASH_SPEED = 20;
	[SerializeField]
	float[] smashRetractPause = new float[] { 4, 4 };
	[SerializeField]
	float[] timeOffset = new float[] { 0, 0 };
	const float SMASH_RETRACT_SPEED = 5;
	const float CLOSE_TO_POINT = 0.1f;

	//time
	float timer;
	float waitTime;
	SmasherState targetSmasherState;

	const float SMASH_PAUSE = 1;
	//makes sure the Wait function runs only once at a time
	bool waiting;

	void Awake ()
	{
		smasherState = SmasherState.Offset;
		if (smashSource == null)
		{
			smashSource = GetComponent<AudioSource>();
		}
		_rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
	{
		if (topTransform == null || bottomTransform == null)
		{
			Debug.LogError("top or bottom Transforms not set.");
			return;
		}
		//Debug.Log(transform.root.name + $" SmasherState: {smasherState}");
		switch ((int)smasherState)
		{
			case 0:
				Smash();
				if (killingCollider != null)
				{
					killingCollider.enabled = true;
				}
				break;
			case 1:
				Retract();
				if (killingCollider != null)
				{
					killingCollider.enabled = false;
				}
				break;
			case 2:
				break;
			case 3:
				if (!waiting)
				{
					waiting = true;
					waitTime = timeOffset[SaveGameManager.difficulty] + 1;
					targetSmasherState = SmasherState.Smashing;
				}
				break;
			default:
				break;
				
		}
		Wait();

	}
	
	void Smash()
	{
		_rb.MovePosition(Vector3.MoveTowards(transform.position, bottomTransform.position, SMASH_SPEED * Time.fixedDeltaTime));
		
		if (Vector3.Distance(transform.position, bottomTransform.position) < CLOSE_TO_POINT && !waiting)
		{
			transform.position = bottomTransform.position;
			waiting = true;
			smashSource.PitchShift();
			smashSource.Play();
			waitTime = SMASH_PAUSE;
			targetSmasherState = SmasherState.Retracting;
		}
	}

	void Retract()
	{
		_rb.MovePosition(Vector3.MoveTowards(transform.position, topTransform.position, SMASH_RETRACT_SPEED * Time.fixedDeltaTime));
		if (Vector3.Distance(transform.position, topTransform.position) < CLOSE_TO_POINT && !waiting)
		{
			transform.position = topTransform.position;
			waiting = true;
			waitTime = smashRetractPause[SaveGameManager.difficulty];
			targetSmasherState = SmasherState.Smashing;
		}
	}
	
	//waits for time then sets state to be targeted state.

	void Wait()
	{
		if (waiting)
		{
            //Debug.Log(name + " Smasher is waiting..");
            smasherState = SmasherState.Waiting;

			timer += 1 * Time.fixedDeltaTime;
			if (timer >= waitTime)
			{
				smasherState = targetSmasherState;
				waiting = false;
				timer = 0;
			}
		}
	}

	//incrementing the timer
	
}
