using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionActivatorH : MonoBehaviour
{
#if UNITY_EDITOR
	public UnityEvent startAction;

    void Awake()
    {
		if (startAction == null)
		{
			startAction = new UnityEvent();
		}
    }
	
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.L))
		{
			startAction.Invoke();
		}
    }
#endif
}
