using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class Hammer : MonoBehaviour
{
	Rigidbody _rb;
	[SerializeField]
	float[] speed = new float[2] { 2, 2 };

	[Tooltip("Sets the angle where it will change directions. If set to 180 will continue to rotate")]
	[Range(0, 180)]
	public float angleLimit = 50;

	[Tooltip("if true then directionChangeSpeed will be determined by dividing speed by directionChangeSpeed. In this case lower values make it faster.")]
	[SerializeField]
	bool directionChangeSpeedIsBasedOnSpeed = true;

	[Tooltip("how fast it changes direction")]
	[SerializeField]
	float[] directionChangeSpeed = new float[2] { 2, 2 };

	public GameObject front;
	public GameObject back;

	public AudioSource whooshSource;

	//target direction
	float _targetDirection = 1;
	//current direction
	float _currentDirection;
	
	bool _flipReset;
	const float RESET_ANGLE = 5;

	void Awake ()
	{
		if (whooshSource == null)
		{
			whooshSource = GetComponent<AudioSource>();
		}
		_rb = GetComponent<Rigidbody>();
	}
	
	void FixedUpdate ()
	{
		Vector3 baseDirection, currentDirection;
		float angle;
		float newDirectionChangeSpeed = directionChangeSpeed[SaveGameManager.difficulty];

		baseDirection = -transform.root.up;
		currentDirection = -transform.up;
		angle = Vector3.Angle(currentDirection, baseDirection);

		if (angle > angleLimit && !_flipReset)
		{
			_targetDirection *= -1;
			_flipReset = true;
		}
		else if (angle < RESET_ANGLE)
		{
			if (!whooshSource.isPlaying)
			{
                whooshSource.PitchShift();
                whooshSource.Play();
			}
			_flipReset = false;
		}


		if (directionChangeSpeedIsBasedOnSpeed)
		{
			newDirectionChangeSpeed = Mathf.Abs(speed[SaveGameManager.difficulty] / directionChangeSpeed[SaveGameManager.difficulty]);
		}

		_currentDirection += _targetDirection * newDirectionChangeSpeed * Time.deltaTime;
		_currentDirection = Mathf.Clamp(_currentDirection,-1,1);
		_rb.angularVelocity = transform.root.right * _currentDirection * speed[SaveGameManager.difficulty];
		
		
		front.SetActive((_rb.angularVelocity.x + _rb.angularVelocity.y + _rb.angularVelocity.z) > 0);
		back.SetActive((_rb.angularVelocity.x + _rb.angularVelocity.y + _rb.angularVelocity.z) < 0);

	}
}
