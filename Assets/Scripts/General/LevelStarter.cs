using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody))]
public class LevelStarter : MonoBehaviour
{
	public Transform pointToMovePlayerTo;
	public Transform elevatorTargetPosition;
	const float closeToPoint = 0.01f;
	public float elevatorSpeed = 2;
	CubeMotor _cubeMotor;
	GameObject _player;
	Rigidbody _rb;
	CubeController _controller;

	void Start ()
	{
		_player = TheCubeGameManager.player;
		_rb = GetComponent<Rigidbody>();
		if (_player != null && elevatorTargetPosition != null)
		{
			_cubeMotor = _player.GetComponent<CubeMotor>();

			if (_cubeMotor != null)
			{
				_cubeMotor.DeactivateMovement();
			}
			if (pointToMovePlayerTo != null)
			{
				_controller = _player.GetComponent<CubeController>();
				_controller.IsKinematic(true);
				_controller.UseGravity(false);
				_player.transform.SetParent(transform);
				_player.transform.position = pointToMovePlayerTo.position;
			}
			StartCoroutine("MoveToPoint");
			
		}
	}

	IEnumerator MoveToPoint()
	{
		while (Vector3.Distance(elevatorTargetPosition.position, transform.position) > closeToPoint)
		{
			transform.position = Vector3.MoveTowards(transform.position, elevatorTargetPosition.position, elevatorSpeed * Time.deltaTime);
			//this will be on the object if it has a CubeMotor so no null check.
			
			yield return null;
		}
		_player.transform.SetParent(null, true);
		_controller.IsKinematic(false);
		_controller.UseGravity(true);
		_cubeMotor.ActivateMovement();
	}
}
