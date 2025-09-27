using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionActivator : MonoBehaviour
{
	public ActionQueueBase[] actions;
	public float loadTime = 25f;
	bool startOverOneShot;
	CubeMotor motor;

	private void Start()
	{
		motor = TheCubeGameManager.player.GetComponent<CubeMotor>();
	}

	private void OnTriggerEnter(Collider other)
	{
		foreach (ActionQueueBase action in actions)
		{
			action.StartQueue();
		}
		if (!startOverOneShot)
		{
			StartCoroutine("StartOver");
			startOverOneShot = true;
		}
		motor.DeactivateMovement();
	}
	IEnumerator StartOver()
	{
		yield return new WaitForSeconds(loadTime);
		TheCubeGameManager.LoadLevelStatic(0);
	}
}
