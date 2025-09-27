using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class Bouncer : MonoBehaviour
{
	public float targetHeight;
	public Color bounceColor;
	Color startColor = Color.green;
	bool waiting;
	const float COLOR_REVERT_TIME = 1f;
	public MeshRenderer bounceMaterial;

	private void Awake()
	{
		if (bounceMaterial != null && Application.isPlaying)
		{
			startColor = bounceMaterial.material.color;
		}
	}

	public void Bounce(Rigidbody movedRb)
	{
		//v = final velocity
		//u = initial velocity
		//a = acceleration
		//s = distance
		//(v^2 - u^2) / 2a = s 
		//I want u
		//2as - v^2 = u^2
		//sqrt(2as - v^2) = u
		float velocity = Mathf.Sqrt(2 * Physics.gravity.magnitude * targetHeight);
		//print(velocity);
		movedRb.velocity = transform.up * velocity;
		if (bounceMaterial != null)
		{
			bounceMaterial.material.color = bounceColor;
			
			if (waiting)
			{
				StopCoroutine("Wait");
			}
			StartCoroutine("Wait", COLOR_REVERT_TIME);
		}

	}

	IEnumerator Wait(float time)
	{
		waiting = true;
		yield return new WaitForSecondsRealtime(time);
		bounceMaterial.material.color = startColor;
		waiting = false;
	}

	void OnGUI()
	{
		Debug.DrawLine(transform.position, transform.position + Vector3.up * targetHeight);
	}
}
