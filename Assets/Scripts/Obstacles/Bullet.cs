using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
	public enum Direction
	{
		forward,back,right,left,up,down
	}

	public Direction direction;
	Vector3 _direction;
	public bool useGlobalSpace = true;
	public float lifetime = 4;
	public float speed = 5;
	public ParticleSystem blasterDeath;
	Rigidbody rb;
	
	void Awake ()
	{
		GetComponent<AudioSource>().PitchShift(0.4f, 0.8f);
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
		Destroy(gameObject,lifetime);
		if (useGlobalSpace)
		{
			switch ((int)direction)
			{
				case 0:
					_direction = transform.forward; 
					break;
				case 1:
					_direction = -transform.forward;
					break;
				case 2:
					_direction = transform.right;
					break;
				case 3:
					_direction = -transform.right;
					break;
				case 4:
					_direction = transform.up;
					break;
				case 5:
					_direction = -transform.up;
					break;
				default:
					break;
			}
		}
		else
		{
			switch ((int)direction)
			{
				case 0:
					_direction = Vector3.forward;
					break;
				case 1:
					_direction = -Vector3.forward;
					break;
				case 2:
					_direction = Vector3.right;
					break;
				case 3:
					_direction = -Vector3.right;
					break;
				case 4:
					_direction = Vector3.up;
					break;
				case 5:
					_direction = -Vector3.up;
					break;
				default:
					break;
			}
		}
		rb.velocity = _direction * speed;
	}
	
	private void OnTriggerEnter(Collider collision)
	{
		if (blasterDeath != null)
		{
			blasterDeath.transform.parent = null;
			blasterDeath.Play();
		} 
		Destroy(gameObject);
	}

}
