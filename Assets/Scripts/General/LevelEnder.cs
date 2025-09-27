using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class LevelEnder : MonoBehaviour
{
	enum LiftState
	{
		idle,
		preLift,
		lift,
		levelChange
	}

	LiftState _liftState;

	const float LIFT_START = 1;
	const float LEVEL_CHANGE = 2;
	CubeMotor _cubeMotor;
	CubeController _controller;
	public float elevatorSpeed = 1;
	public Transform playerLiftPosition;
	public Transform liftingElevator;
	public Transform targetPosition;

	bool startSuccess;
	public static bool levelComplete;

	private void Awake()
	{
		GetComponent<Collider>().isTrigger = true;
	}

	private void Start()
	{
		if (TheCubeGameManager.player != null)
		{
			_cubeMotor = TheCubeGameManager.player.GetComponent<CubeMotor>();
			_controller = TheCubeGameManager.player.GetComponent<CubeController>();
		}
		if (_cubeMotor != null && playerLiftPosition != null && liftingElevator != null && targetPosition != null)
		{
			startSuccess = true;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && startSuccess)
		{
			//SaveGameManager.LevelComplete(TheCubeGameManager.CurrentLevel, true, _controller.deathCount, Time.timeSinceLevelLoad);
			levelComplete = true;
			StartCoroutine("LevelComplete");
			
		}
	}

	private void Update()
	{
		//this is for the movement of the elevator
		if (startSuccess)
		{
			switch ((int)_liftState)
			{
				case 0:
					break;
				case 1:
					_cubeMotor.transform.position = Vector3.MoveTowards(_cubeMotor.transform.position, playerLiftPosition.position, (LIFT_START * 2) * Time.deltaTime);
					break;
				case 2:
				case 3:
					//print("lifting");
					_cubeMotor.transform.position = playerLiftPosition.position;
					liftingElevator.position = Vector3.MoveTowards(liftingElevator.position, targetPosition.position, elevatorSpeed * Time.deltaTime);
					break;
				default:
					break;
			}
		}
	}

	IEnumerator LevelComplete()
	{
		//this mostly sets the states
		
			_cubeMotor.DeactivateMovement();
			_cubeMotor.SetToKinematic(true);
			_cubeMotor.transform.parent = playerLiftPosition;
			_liftState = LiftState.preLift;

			yield return new WaitForSeconds(LIFT_START);
			_liftState = LiftState.lift;

			yield return new WaitForSeconds(LEVEL_CHANGE);
			_liftState = LiftState.levelChange;

			TheCubeGameManager.LoadNextLevel();
		
	}
}
