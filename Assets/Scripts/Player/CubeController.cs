
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollisionHelp))]
public class CubeController : MonoBehaviour
{
	GameObject _player;
	Rigidbody _rigidbody;
	public Transform playerMesh;
	MeshRenderer _playerMeshRenderer;

	//collision help
	BoxCollisionHelp _bCH;
	const float _COLLISION_BIAS = 0.999f;
	//both needed for more realistic movement
	public PhysicMaterial noFrictionMat;
	public PhysicMaterial frictionMat;

	//player positions
	public Transform respawnPoint;
	public float positionZ;
	public float stepHeight = 0.1f;
	public bool moving;

	//layerMasks
	public LayerMask moveLayerMask;
	public LayerMask groundLayerMask;
	
	//death
	bool _deadReset;
	public bool _inCollider;
	public bool dead;
	public delegate void Died();
	public static event Died YouDied;
	Collider _savedCollider;
	public Vector3 VelocityOnDeath { get; private set; }

	public UnityEvent onRespawn;
	public bool RespawnTriggered { get; private set; }

	//player shaking
	Vector3 playerShakeTargetPos;
	Vector3 playerCurrentPosition;

	private void Awake()
	{
		SetPlayerGameObjectAndRigidbody();
		_bCH = GetComponent<BoxCollisionHelp>();
		if (playerMesh == null)
		{
			playerMesh = transform.Find("playerMesh");
		}
		if (playerMesh != null)
		{
			_playerMeshRenderer = playerMesh.GetComponent<MeshRenderer>();
		}
		if (onRespawn == null)
		{
			onRespawn = new UnityEvent();
		}
	}
	
	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.tag == "IgnoreOnDeath" && dead)
		{
			_savedCollider = collision.collider;
			Physics.IgnoreCollision(_bCH.boxCollider, collision.collider);
			
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		_inCollider = true;
		if (!dead && _savedCollider != null)
		{
			Physics.IgnoreCollision(_bCH.boxCollider, _savedCollider, false);
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		_inCollider = false;
	}

	/// <summary>
	/// sets the player and rigidbody. gets attached gameObject if empty
	/// </summary>
	void SetPlayerGameObjectAndRigidbody(GameObject player = null)
	{
		_player = player ?? gameObject;
		_rigidbody = _player.GetComponent<Rigidbody>();
	}

	/// <summary>
	/// basic movement based on the mass that excludes changing the y.
	/// </summary>
	public void Move(float speed, Vector3 direction, Vector3 extraVelocity)
	{
		if (!_bCH.CollidingFromDirection(direction, moveLayerMask, _COLLISION_BIAS))
		{
			//I dont want this to change the Y at all in this case so setting to be the current velocity.
			moving = direction.magnitude <= 0.1f ? false : true;
			Vector3 newDirection = speed * direction;

			newDirection.y = _rigidbody.velocity.y;

			_rigidbody.velocity = newDirection / _rigidbody.mass + extraVelocity;
		}
		else
		{
			moving = false;
		}
	}


	public void MoveToPoint(Vector3 position)
	{
		_rigidbody.position = position;
	}


	public bool Ground()
	{
		float offset = 0.1f;
		/*was trying increasing its reach
		if (_rigidbody.velocity.y < -10)
		{
			offset = 0.1f;
		}
		*/
		if (_bCH.CollidingFromDirection(-transform.up, groundLayerMask, _COLLISION_BIAS, offset))
		{
			/*if (_rigidbody.velocity.y < -3)
			{
				//trying to stop it a little bit before contact
				_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, 0, _rigidbody.velocity.z);
			}
			*/
			return _inCollider;
		}
		return false;
	}


	/// <summary>
	/// makes player move up if jumping and target distance isn't reached. 
	/// </summary>
	/// <param name="speed"></param>
	/// <param name="jumping"></param>
	public void Jump(float speed, bool jumping, ref bool distanceReached)
	{
		
		float offset = 0.1f;
		if (_bCH.CollidingFromDirection(transform.up, moveLayerMask, _COLLISION_BIAS, offset))
		{
			distanceReached = true;
		}
		if (jumping && !distanceReached)
		{
			_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, speed, _rigidbody.velocity.z);
		}
	}
	
	/// <summary>
	/// makes player move up if jumping.
	/// </summary>
	/// <param name="speed"></param>
	/// <param name="jumping"></param>
	public void Jump(float speed, bool jumping)
	{
		
		if (jumping)
		{
			_rigidbody.velocity = new Vector3(_rigidbody.velocity.x, speed, _rigidbody.velocity.z);
		}
	}


	public void Dead(float deathTime, bool hideMesh = true, bool turnOffPhysics = true, bool removeConstraints = true)
	{
		dead = true;
		_bCH.boxCollider.material = frictionMat;
		VelocityOnDeath = _rigidbody.velocity;
		if (hideMesh)
		{
			_playerMeshRenderer.enabled = false;
		}
		if (turnOffPhysics)
		{
			_bCH.boxCollider.enabled = false;
			_rigidbody.isKinematic = true;
		}
		if (removeConstraints)
		{
			RemoveConstraints();
		}

		if (!_deadReset)
		{
			moving = false;
			_deadReset = true;
			YouDied();

			StartCoroutine("WaitForRespawn", deathTime);
		}
	}

	IEnumerator WaitForRespawn(float deathTime)
	{
		yield return new WaitForSeconds(deathTime);
		RespawnTriggered = true;
	}


	public bool OnEdge()
	{
		float offset = 0.2f;
		float bias = 0.30f;
		
		if (_bCH.CollidingFromDirection(-transform.up, groundLayerMask, bias, offset))
		{
			
			return !_inCollider;
		}
		return true;
	}


	public void Respawn()
    {
        RespawnTriggered = false;
        _bCH.boxCollider.enabled = true;
        _rigidbody.isKinematic = true;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        transform.position = respawnPoint.position;
        _player.transform.rotation = Quaternion.identity;
        _rigidbody.isKinematic = false;
        _bCH.boxCollider.material = noFrictionMat;
        _playerMeshRenderer.enabled = true;
        _deadReset = false;
        dead = false;
        onRespawn.Invoke();
	}


	public void UseGravity(bool enabled)
	{
		_rigidbody.useGravity = enabled;
	}
	

	public void IsKinematic(bool kinematic)
	{
		_rigidbody.isKinematic = kinematic;
		_rigidbody.interpolation = kinematic ? RigidbodyInterpolation.None : RigidbodyInterpolation.Interpolate; 
	}


	public Rigidbody GetRigidbody()
	{
		return _rigidbody;
	}

	public void RemoveConstraints()
	{
		_bCH.boxCollider.material = frictionMat;
		_rigidbody.constraints = RigidbodyConstraints.None;
		
	}


	public void Recover()
    {
        _rigidbody.isKinematic = false;
        _rigidbody.velocity = Vector3.up * 2;
		MoveToZ();
        _player.transform.rotation = Quaternion.identity;
		_rigidbody.angularVelocity = Vector3.zero;
		_rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		_bCH.boxCollider.material = noFrictionMat;
	}


	public void SnapToZ()
	{
		transform.position = new Vector3(transform.position.x, transform.position.y, positionZ);
	}


	public void MoveToZ(float speed = 1)
	{
		Vector3 newPosition = new Vector3(transform.position.x, transform.position.y, positionZ);
		transform.position = Vector3.MoveTowards(transform.position, newPosition, speed * Time.deltaTime);
	}


	public void Step(Vector3 direction, float stepSpeed)
	{
		
		float offset = 0.1f;
		Vector3 centerOffset = transform.position + (Vector3.down * (0.49f));
		bool stepOver, somethingInWay;
		//checks if there's something to step over
		//stepOver = _bCH.CollidingFromDirection(centerOffset, direction, groundLayerMask, step * 2f, offset);
		Ray ray = new Ray(centerOffset, direction);
		stepOver = Physics.Raycast(ray, 0.5f + offset, groundLayerMask, QueryTriggerInteraction.Ignore);
		
		Debug.DrawRay(centerOffset,direction, Color.green);

		//makes sure nothing is in the way
		somethingInWay = _bCH.CollidingFromDirection(direction, groundLayerMask, 1 - stepHeight * 2, offset);

		if (stepOver && !somethingInWay && direction.magnitude > 0)
		{
			_rigidbody.velocity = (direction + Vector3.up) * stepSpeed;// + (direction * stepSpeed/2) ;
			
		}
	}

	public void PlayerShake(Vector3 limits, float shakeStrength, bool shaking)
	{
		if (shaking)
		{

			if (Vector3.Distance(playerShakeTargetPos, playerMesh.position) > 0.01f)
			{
				playerMesh.position = Vector3.MoveTowards(playerMesh.position, playerShakeTargetPos, shakeStrength * Time.deltaTime);
			}
			else
			{
				playerShakeTargetPos = Random.insideUnitSphere.Multiply(limits) + _player.transform.position;
			}
		}
		else if (dead)
		{

		}
		else
		{
			playerShakeTargetPos = _player.transform.position;
			if (Vector3.Distance(playerShakeTargetPos, playerMesh.position) > 0.01)
			{
				playerMesh.position = Vector3.MoveTowards(playerMesh.position, playerShakeTargetPos, shakeStrength * Time.deltaTime);
			}
		}
	}

	/// <summary>
	/// checks if you're moving at all
	/// </summary>
	/// <returns></returns>
	public bool VelocityCheck()
	{
		return _rigidbody.velocity.magnitude > 0;
	}
}
