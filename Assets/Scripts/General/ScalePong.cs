using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalePong : MonoBehaviour
{
	Vector3 startScale;
	float value;
	void Awake()
    {
		startScale = transform.localScale;
		InvokeRepeating("Pong",0.1f,0.1f);
    }

	void Pong()
	{

		value = Mathf.PingPong(Time.time, 0.1f);
		transform.localScale = startScale + (Vector3.one * value);
	}
}
