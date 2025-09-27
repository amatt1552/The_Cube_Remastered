using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : MonoBehaviour
{
	[Tooltip("keeps the timing in sync.")]
	public Syncer syncer;
	[Range(10, 179)]
	public float angleLimit = 25;
	public float[] speed = new float[2] { 2, 2 };
	public float currentAngle;
	int flipper = 1;
	//makes sure it doesn't try to flip again until reset
	bool flipReset = true;
	const float PAUSE_TIME = 2;
	GameObject _bullet;
	//angle where it resets
	const float RESET_ANGLE = 5;
	bool waiting;
	public Transform blastPos;
	public float[] fireRate = new float[2] { 2, 2 };
	float startTime;
	int difficulty;
	
	void Start ()
	{
		difficulty = SaveGameManager.difficulty;
		if(speed[difficulty] != 0)
			flipper = (int)(Mathf.Abs(speed[difficulty])/ speed[difficulty]);
		speed[difficulty] = Mathf.Abs(speed[difficulty]);
		
		_bullet = (GameObject)Resources.Load("Prefabs/Blast");
		if (_bullet == null)
			Debug.LogWarning("I need bullets!");
		if (blastPos == null)
		{
			blastPos = transform;
			Debug.LogWarning("Where do you fire this from again?");
		}
		InvokeRepeating("Fire", fireRate[difficulty], fireRate[difficulty]);
	}

	private void FixedUpdate()
	{
		RotateTowards();
		if (syncer == null)
		{
			Wait();
		}
	}

	private void OnEnable()
	{
		if (syncer != null)
		{
			syncer.WaitComplete += WaitForSyncer;
		}
	}

	private void OnDisable()
	{
		if (syncer != null)
		{
			syncer.WaitComplete -= WaitForSyncer;
		}
	}

	void WaitForSyncer()
	{
		Flip();
		waiting = false;
	}

	void Wait()
	{
		if (Time.realtimeSinceStartup - startTime > PAUSE_TIME && waiting == true)
		{
			Flip();
			waiting = false;
		}
		
	}

	void RotateTowards()
	{
		
		Vector3 direction = transform.up;
		float angleCheck = Vector3.Angle(-Vector3.down, direction);
		angleCheck = Mathf.Ceil(angleCheck);
		if (angleCheck <= RESET_ANGLE)
		{
			flipReset = true;
		}

		//time to wait
		
		if (angleCheck >= angleLimit && flipReset)
		{
			if (!waiting)
			{
				if (syncer == null)
				{
					startTime = Time.realtimeSinceStartup;

					waiting = true;
					flipReset = false;
				}
				else
				{
					syncer.StartWait(PAUSE_TIME);
					waiting = true;
					flipReset = false;
				}
			}
		}
		
		
		//clamps values and rotates object

		if (!waiting)
		{
			currentAngle += speed[difficulty] * flipper;
			currentAngle = currentAngle >= 360 ? 0 : currentAngle;
			currentAngle = currentAngle < 0 ? 360 : currentAngle;
			if (currentAngle > angleLimit && currentAngle < 360 - angleLimit)
			{
				//print("oops");
				if (flipper < 0)
				{
					currentAngle = 360 - angleLimit;
				}
				else
				{
					currentAngle = angleLimit;
				}
			}
			
			transform.rotation = Quaternion.AngleAxis(currentAngle, Vector3.forward);
		}
	}

	void Flip()
	{
		flipper *= -1;
	}

	void Fire()
	{
		
		GameObject bulletGO = Instantiate(_bullet, blastPos.position, blastPos.rotation);
			
	}
}
