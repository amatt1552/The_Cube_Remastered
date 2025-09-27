using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Syncer : MonoBehaviour
{
	bool _waiting;
	public delegate void OnWaitComplete();
	public event OnWaitComplete WaitComplete;

	public void StartWait(float time)
	{
		if(!_waiting)
		{
			StartCoroutine("Waiting", time);
		}
	}

	public void StopWait()
	{
		if (_waiting)
		{
			StopCoroutine("Waiting");
		}
	}

	IEnumerator Waiting(float time)
	{
		_waiting = true;
		yield return new WaitForSeconds(time);
		_waiting = false;
		WaitComplete?.Invoke();
	}
}
