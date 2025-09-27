using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pusher : MonoBehaviour
{
	enum PushState
	{
		retract, pushWarning, push
	}

	//its currentState.
	PushState pushState;
	
	public Syncer syncer;

	//makes sure they run one at a time.
	bool _coroutineRunning;

	//pushing out
	const float PUSH_SPEED = 10;
	[SerializeField]
	float[] pushTime = new float[2] { 4, 4 };
	const float WARNING_TIME = 1;

	//pulling back in
	const float RETRACT_SPEED = 2;
	const float RETRACT_TIME = 2;

	//transforms and such for pusher. might make a scriptable object for them later.
	public Transform pusher, pushPoint, retractPoint;
	Rigidbody _pusherRb;
	Vector3 targetPoint;
	public MeshRenderer pusherMat;

	//sets the color to be pusher material's color at start.
	Color defaultColor;
	//the color it changes to right before pushing
	public Color pushWarningColor = Color.yellow;
	public Color pushCompleteColor = Color.black;

	//how close it gets to a point
	const float CLOSE_TO_POINT = 0.1f;

	//trigger used to kill player
	public Collider killingTrigger;

	//difficulty settings
	int difficulty;
	[SerializeField]
	GameObject spikes;

	private void Awake()
	{
		VariableChecks();
		
		//makes sure nothing is null since there are a lot of parts involved 
		if (pusherMat != null)
		{
			defaultColor = pusherMat.material.color;
		}
		else
		{
			Debug.LogWarning("pusherMat not set for " + name + "!");
		}

		if (retractPoint == null)
		{
			Debug.LogWarning("retractPoint not set for " + name + "!");
			retractPoint = transform;
		}
		if (pushPoint == null)
		{
			Debug.LogWarning("pusherPoint not set for " + name + "!");
			pushPoint = transform;
		}
		if (pusher == null)
		{
			Debug.LogWarning("pusher not set for " + name + "!");
			pusher = transform.Find("pusher");
		}
		if (pusher != null)
		{
			_pusherRb = pusher.GetComponent<Rigidbody>();
			
		}
		VariableChecks();
	}
	

	private void FixedUpdate()
	{
		//if I can't find the pusher I just dont let this run.
		if (_pusherRb != null)
		{
			//gets difficulty
			difficulty = SaveGameManager.difficulty;
			
			//sets spike visibility based on difficulty
			if (spikes != null)
			{
				switch (difficulty)
				{
					case 0:
						if (spikes.activeInHierarchy)
						{
							spikes.SetActive(false);
						}
						break;
					case 1:
						if (!spikes.activeInHierarchy)
						{
							spikes.SetActive(true);
						}
						break;
					default:
						break;
				}
			}

			if (!_coroutineRunning && syncer == null)
			{
				
				switch (pushState)
				{
					case PushState.pushWarning:
						StartCoroutine("PushWarningEnum");
						break;
					case PushState.retract:
						StartCoroutine("RetractEnum");
						break;
					case PushState.push:
						StartCoroutine("PushEnum");
						break;
					default:
						break;
				}
			}
			else if(syncer != null)
			{
				switch (pushState)
				{
					case PushState.pushWarning:
						PushWarning();
						break;
					case PushState.retract:
						Retract();
						break;
					case PushState.push:
						Push();
						break;
					default:
						break;
				}
			}
		}
		else
		{
			Debug.LogError("could not set pusher.. Set in inspector before start or name the child pusher.");
		}
	}


	private void OnEnable()
	{
		if (syncer != null)
		{
			syncer.WaitComplete += WaitForSyncer;
		}
	}

	private void OnDisable()
	{
		if (syncer != null)
		{
			syncer.WaitComplete -= WaitForSyncer;
		}
	}

	void WaitForSyncer()
	{
		switch (pushState)
		{
			case PushState.pushWarning:
				pushState = PushState.push;
				break;
			case PushState.retract:
				pushState = PushState.pushWarning;
				break;
			case PushState.push:
				pushState = PushState.retract;
				break;
			default:
				break;
		}
	}


	//pushing out method
	void Push()
	{
		killingTrigger.enabled = true;
		//go to push point
		if (Vector3.Distance(pusher.position, pushPoint.position) > CLOSE_TO_POINT)
		{
			_pusherRb.MovePosition(Vector3.MoveTowards(pusher.position, pushPoint.position, PUSH_SPEED * Time.fixedDeltaTime));


		}
		else
		{
			killingTrigger.enabled = false;
			syncer.StartWait(RETRACT_TIME);
		}
		if (pusherMat != null)
		{
			pusherMat.material.color = pushCompleteColor;
		}
		//wait to retract back
		
		
	}
	//pushing out Enum
	IEnumerator PushEnum()
	{
		_coroutineRunning = true;
		killingTrigger.enabled = true;
		//go to push point
		while (Vector3.Distance(pusher.position, pushPoint.position) > CLOSE_TO_POINT)
		{
			_pusherRb.MovePosition(Vector3.MoveTowards(pusher.position, pushPoint.position, PUSH_SPEED * Time.fixedDeltaTime));
			yield return new WaitForFixedUpdate();

		}
		if (pusherMat != null)
		{
			pusherMat.material.color = pushCompleteColor;
		}
		//wait to retract back
		killingTrigger.enabled = false;
		yield return new WaitForSeconds(RETRACT_TIME);
		pushState = PushState.retract;
		_coroutineRunning = false;
	}

	//pulling in method
	void Retract()
	{
		if (Vector3.Distance(pusher.position, retractPoint.position) > CLOSE_TO_POINT)
		{
			_pusherRb.MovePosition(Vector3.MoveTowards(pusher.position, retractPoint.position, RETRACT_SPEED * Time.fixedDeltaTime));
		}
		else
		{
			syncer.StartWait(pushTime[difficulty] - WARNING_TIME);
		}
		if (pusherMat != null)
		{
			pusherMat.material.color = defaultColor;
		}
	}
	//pulling in Enum
	IEnumerator RetractEnum()
	{
		_coroutineRunning = true;

		//go to retract point
		while (Vector3.Distance(pusher.position, retractPoint.position) > CLOSE_TO_POINT)
		{
			_pusherRb.MovePosition(Vector3.MoveTowards(pusher.position, retractPoint.position, RETRACT_SPEED * Time.fixedDeltaTime));
			yield return new WaitForFixedUpdate();

		}
		if (pusherMat != null)
		{
			pusherMat.material.color = defaultColor;
		}
		//wait until its time to warn about the incoming push
		yield return new WaitForSeconds(pushTime[difficulty] - WARNING_TIME);
		pushState = PushState.pushWarning;
		_coroutineRunning = false;
	}

	void PushWarning()
	{
		if (pusherMat != null)
		{
			//changes material so player can tell its about to push
			pusherMat.material.color = pushWarningColor;
		}
		syncer.StartWait(WARNING_TIME);
	}
	//Enum warning before push method 
	IEnumerator PushWarningEnum()
	{
		_coroutineRunning = true;
		if (pusherMat != null)
		{
			//changes material so player can tell its about to push
			pusherMat.material.color = pushWarningColor;
		}

		//wait to push
		yield return new WaitForSeconds(WARNING_TIME);
		
		pushState = PushState.push;
		_coroutineRunning = false;
	}


	//makes sure certain values aren't wrong at start
	void VariableChecks()
	{
		//if (pushTime[difficulty] < WARNING_TIME)
		//{
		//	pushTime[difficulty] = WARNING_TIME;
		//}

		if (pushTime[difficulty] < 0)
		{
			pushTime[difficulty] = 0;
		}
	}

}
