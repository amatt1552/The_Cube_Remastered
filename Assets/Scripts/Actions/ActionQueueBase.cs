using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Rigidbody))]

//The objective of this script is to be overridden to simplify making realtime cinematics. This may also be able to be used for ai however.

public class ActionQueueBase : MonoBehaviour
{
	protected int state; //picks what function is running the stuff
	protected float timer = 0.0f; //timer for types of movement
	public bool loop; //queue repeats
	public bool playOnAwake;
	[Tooltip("Plays all of these scripts in this GameObject from top to bottom")]
	public bool playAll = true;
	[Tooltip("Snaps back to initial point at end of queue")]
	public bool initialPointEnd;
	[Tooltip("If true the order of execution will be based on the index in your created action. Must manually set all.")]
	public bool setOrderFromAction;
	public float groundOffset = 0.1f;

	protected Queue<Action> actionQueue; //da queue
	public Action[] actions;
	Action[] sortedActions; //sorted out actions
	protected Action currentAction;
	bool startedQueue; //makes sure only called once
	protected Vector3 currentDirection; //direction for movement
	protected Vector3 targetRotation; //targeted rotation for RotateTo
	protected Transform targetPosition; //targeted position for MoveTo
	protected float currentSpeed; //speed for movement
	
	protected Animator currentAnimation;
	protected bool animDone = true; //animation complete
	protected float currentAnimSpeed; //animation speed

	//public UnityEvent OnActionStart;//called everytime an action is executed
	//public UnityEvent OnActionComplete;//called everytime an action is executed
	public UnityEvent OnAllActionsComplete;//called when all actions are done

	
	ActionQueueBase[] actionBases;//used to find all ActionQueueBase scripts on this gameobject
	int budIndex;//pickes current ActionQueue

	protected Rigidbody rb;

	//falling stuff
	protected bool falling;//currently falling
	protected bool fallImminent;//about to fall
	public CanvasGroup fallingCanvasGroup;//sweating

	//color stuff
	protected GameObject fadeTarget;//object faded
	protected Color targetColor;//color faded to
	protected float fadeTime;//time in seconds

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		actionQueue = new Queue<Action>();
		if (OnAllActionsComplete == null)
		{
			OnAllActionsComplete = new UnityEvent();
		}
		actionBases = GetComponents<ActionQueueBase>();
		OnAllActionsComplete.AddListener(ExtraBudCheck);

		if (playOnAwake)
		{
			StartQueue();
		}
	}

	protected virtual void ExtraBudCheck()
	{
		if (playAll)
		{
			budIndex++;
			if (budIndex < actionBases.Length)
			{
				actionBases[budIndex].StartQueue();
			}
			else
			{
				budIndex = 0;
			}
		}
	}
	
	protected virtual void FixedUpdate()
	{
		//if (movementQueue.Count > 0)
		//{
		//	timer += Time.deltaTime;
		//}
		if (currentAction != null)
		{
			
			rb.useGravity = currentAction.usingGravity;
			rb.isKinematic = currentAction.isKinematic;
			switch (state)
			{
				case 0:
					Idle();
					break;
				case 1:
					Jump(currentDirection, currentSpeed);
					break;
				case 2:
					Move(currentDirection, currentSpeed);
					break;
				case 3:
					Flip(currentDirection, currentSpeed, currentAnimSpeed);
					break;
				case 4:
					Fall();
					break;
				case 5:
					Recover(currentDirection, currentSpeed);
					break;
				case 6:
					RotateOnAxis(currentDirection, currentSpeed);
					break;
				case 7:
					RotateTo(targetRotation, currentSpeed);
					break;
				case 8:
					MoveTo(targetPosition, currentSpeed);
					break;
				case 9:
					Fade(fadeTarget, targetColor, fadeTime);
					break;
				default:
					Idle();
					break;
			}
		}
	}

	public virtual void StartQueue()
	{
		
		if (!startedQueue)
		{
			SortAction();
			
			for (int i = 0; i < sortedActions.Length; i++)
			{
				//Debug.Log(actions[1].actionState);
				actionQueue.Enqueue(sortedActions[i]);
			}
			
			//timer = 0;
			StartCoroutine("Wait");
			startedQueue = true;
		}
	}
	public virtual void CancelQueue()
	{
		SavePhysics();
		StopCoroutine("Wait");
		actionQueue = null;
		currentAction = new Action();
		UseSavedPhysics();
		state = 0;
		OnAllActionsComplete.Invoke();
		rb.velocity = Vector3.zero;
		
	}

	protected bool Ground()
	{
		//setting up the ray
		Ray ray = new Ray();
		ray.origin = transform.position;
		ray.direction = Vector3.down;
		//getting the distance
		float distance = transform.lossyScale.y * (0.50f + groundOffset);
		//draw the line
		Debug.DrawLine(ray.origin, ray.origin + (ray.direction * distance), Color.red);
		return Physics.Raycast(ray, distance);

	}

	public void SortAction()
	{
		if (setOrderFromAction)
		{
			sortedActions = new Action[actions.Length];
			for (int i = 0; i < actions.Length; i++)
			{
				sortedActions[actions[i].index] = actions[i];
			}
		}
		else
		{
			sortedActions = actions;
		}
	}

	IEnumerator Wait()
	{
		while(actionQueue.Count > 0)
		{
			//get action
			currentAction = actionQueue.Peek();
			state = (int)currentAction.actionState;
			//set current values
			currentDirection = currentAction.direction;
			targetRotation = currentAction.targetRotation;
			targetPosition = currentAction.targetPosition;
			currentSpeed = currentAction.speed;

		
			currentAnimation = currentAction.animation;
			currentAnimSpeed = currentAction.animationSpeed;

			fadeTarget = currentAction.fadeTarget;
			targetColor = currentAction.targetColor;
			fadeTime = currentAction.fadeTime;

			//wait
			yield return new WaitForSeconds(currentAction.holdtime);
			//wait for anim to be done
			if (currentAction.waitForAnim && !animDone)
			{
				yield return new WaitUntil(GetAnimState);
			}
			if (currentAnimation != null)
			{
				currentAnimation.enabled = false;
			}
			//remove from queue
			actionQueue.Dequeue();
		}
		startedQueue = false;
		if (loop)
		{
			//restart
			StartQueue();
		}
		else
		{
			//end
			SavePhysics();
			currentAction = new Action();
			UseSavedPhysics();
			OnAllActionsComplete.Invoke();
		}
	}

	//animation events
	protected void AnimStart()
	{
		animDone = false;
	}
	protected void AnimDone()
	{
		
		animDone = true;
	}

	//checks if animation is done
	protected bool GetAnimState()
	{
		return animDone;
	}

	//preserves physics at end
	bool usingGrav, isKin; //helps set the settings for rb to be the last variables you set in the queue
	protected virtual void SavePhysics()
	{
		usingGrav = currentAction.usingGravity;
		isKin = currentAction.isKinematic;
	}
	protected virtual void UseSavedPhysics()
	{
		currentAction.usingGravity = usingGrav;
		currentAction.isKinematic = isKin;
	}

	protected virtual void Fall()
	{
		if (!fallImminent)
		{
			StartCoroutine("FallImminent", 0.5f);
		}
	}
	IEnumerator FallImminent(float waitTime)
	{
		//falling image
		if (fallingCanvasGroup != null)
		{
			fallingCanvasGroup.alpha = 1;
		}
		fallImminent = true;

		yield return new WaitForSeconds(waitTime);
	
		rb.constraints = RigidbodyConstraints.None;
		falling = true;
		if (fallingCanvasGroup != null)
		{
			fallingCanvasGroup.alpha = 0;
		}
	}
	protected virtual void Recover(Vector3 direction, float speed)
	{
		if (falling)
		{
			rb.constraints = RigidbodyConstraints.FreezeRotation;
			rb.AddForce(direction * speed, ForceMode.Impulse);
			rb.angularVelocity = Vector3.zero;
			rb.MoveRotation(Quaternion.identity);
			falling = false;
			fallImminent = false;
		}
	}

	protected virtual void Jump(Vector3 direction, float speed)
	{
		if (Ground())
		{
			rb.velocity = direction * speed;
		}
	}

	protected virtual void Idle()
	{
		Debug.Log("Idle");
	}

	protected virtual void Flip(Vector3 direction, float speed, float animSpeed)
	{
		Debug.Log("Flip");
	}
	
	protected virtual void Move(Vector3 direction, float speed)
	{
		rb.velocity = direction * speed;
	}

	protected virtual void RotateOnAxis(Vector3 direction, float speed)
	{
		rb.angularVelocity = direction * speed;
	}

	protected virtual void RotateTo(Vector3 targetAngle, float speed)
	{
		
		rb.MoveRotation(Quaternion.RotateTowards(rb.rotation,Quaternion.Euler(targetAngle),speed));
	}

	protected virtual void MoveTo(Transform targetPosition, float speed)
	{
		if (targetPosition != null)
		{
			Vector3 direction = targetPosition.position - transform.position;
			direction.Normalize();
			if (Vector3.Distance(targetPosition.position, transform.position) > 0.1f)
			{
				rb.velocity = direction * speed;
			}
			else
			{
				rb.MovePosition(targetPosition.position);
				rb.velocity = Vector3.zero;
			}
		}
	}

	protected virtual void Fade(GameObject fadedObject, Color targetColor, float targetTime)
	{
		Debug.Log("Faded");
	}
	

	[System.Serializable]
	public class Action
	{
		public enum ActionState
		{
			Idle,
			Jump,
			Move,
			Flip,
			Fall,
			Recover,
			RotateOnAxis,
			RotateTo,
			MoveTo,
			Fade
		}
		//basic Settings
		public ActionState actionState;
		public Vector3 direction;
		public Vector3 targetRotation;
		public Transform targetPosition;
		public int index;
		public float holdtime = 1;
		public float speed = 1;
		//physics
		public bool usingGravity = true;
		public bool isKinematic;
		//animations
		public bool waitForAnim;
		public Animator animation;
		public float animationSpeed = 1;
		//color fade
		public Color targetColor;
		public GameObject fadeTarget;
		public float fadeTime;

		public Action()
		{
			speed = 1;
			holdtime = 1;
			animationSpeed = 1;
			usingGravity = true;
		}
	}
}
