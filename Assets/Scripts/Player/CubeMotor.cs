using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(CubeController))]
public class CubeMotor : MonoBehaviour
{
	//state of object. not sure if I want to use this yet.
	enum CurrentState
	{
		alive,
		burned,
		fell,
		popped,
		pushed,
		squished,
		whacked
	}
	CurrentState currentState;

	//used to self destruct
	public float selfDestructTargetTime = 3;
	float destructTime;
	bool selfDestruct;
	bool shaking;

	//launcher
	bool inLauncher;
	Launcher currentLauncher;
	bool _launching;

	//bouncer
	bool _inBouncer;
	bool _groundJumpActive = true;
	Bouncer _currentBouncer;

	//speedboost
	bool _inBooster;
	float _currentBoostingSpeed;
	Vector3 _boostingDirection;
	Vector3 boostRaw;
	float boostingPitch;

	//movingObject
	bool onMovingObject;
	WaypointNav movingObject;
	Vector3 oldMovingObjectDirection;
	Vector3 movingObjectDirection;

	//camera
	bool _inCameraEdit;
	public CameraFollow cameraFollow;
	CameraEditSettings _currentCameraEdit;

	//misc movement and player stuff
	public bool movementEnabled { get; private set; } = true;
	[Header("Set before starting")]
	public CubeInfoScriptableObject cubeInfoSO;
	CubeController controller;
	Rigidbody _rb;
	Vector3 moveDirection;
	float zoomDirection;
	float _currentSpeed;
	GameObject _playerMesh;
	MeshRenderer _playerMeshRenderer;
	Color startingEmmission;
	Color currentEmmission;
	bool respawning;
	public delegate void OnTransformChanged();
	public event OnTransformChanged TransformChanged;

	public CanvasGroup fallingCanvasGroup;

	#region jump variables.

	int _extraJumps;
	float _realJumpForce;
	float targetHeight;
	//makes the jumpForce increase to the cubeinfo's faster.
	const float _jumpForceIncreaseMultiplier = 4;

	//will run like a getKey
	bool _holdingJump;
	//will run like a getKeyDown
	bool jumpDown;
	//helps reset the the _jumpdown
	bool _jumpDownReset = true;
	bool _jumpOneShot;
	//sets the moment you jump
	Vector3 _positionBeforeJump;
	bool _distanceReached;

	#endregion

	//helps prevent missing the ground check.
	bool ground;
	bool _groundOneShot;

	//for falling
	bool fallImminent;
	bool falling;

	//for getting back up
	bool recoverOneShot;
	bool _recoverTriggered;

	//particle effects
	ParticleSystem movingEffect;
	ParticleSystem poppedEffect;

	//sound effects
	AudioSource movingSource;
	AudioSource hittingSource;
	bool hittingOneShot;
	bool runningTurnDown;
	AudioSource poppedSource;
	AudioSource burnedSource;
	
	//saved settings
	GameSettings gameSettings;
	NewItem[] newItems;

	private void Awake()
	{
		
		if (cubeInfoSO == null)
		{
			Debug.LogError("Need to add a CubeInfoScriptableObject to this controller!");
			enabled = false;
		}

		controller = GetComponent<CubeController>();
		_rb = controller.GetRigidbody();
		targetHeight = cubeInfoSO.jumpHeight;

		//particle effects

		GameObject tempObj;

		tempObj = Instantiate(cubeInfoSO.slidingEffects[0]);
		tempObj.transform.parent = transform;
		tempObj.transform.localPosition = Vector3.down * 0.49f;
		movingEffect = tempObj.GetComponent<ParticleSystem>();

		tempObj = Instantiate(cubeInfoSO.poppedEffects[0]);
		tempObj.transform.parent = transform;
		tempObj.transform.localPosition = Vector3.zero;
		tempObj.transform.rotation = transform.rotation;
		poppedEffect = tempObj.GetComponent<ParticleSystem>();

		controller.onRespawn.AddListener(OnRespawn);

		//sound effects

		movingSource = CreateAudioSource(cubeInfoSO.slidingSoundEffects[0], "SoundEffects", 0.3f);
		hittingSource = CreateAudioSource(cubeInfoSO.hittingSoundEffects[0], "SoundEffects", 0.5f);
		poppedSource = CreateAudioSource(cubeInfoSO.poppedSoundEffects[0], "SoundEffects", 1, 1.5f);
		burnedSource = CreateAudioSource(cubeInfoSO.burnedSoundEffects[0], "SoundEffects", 0.1f, 1.5f);

		
	}

	//helps making the sources less tedious. I'll probably make an extention or maybe a static class later.

	AudioSource CreateAudioSource(AudioClip clip, string mixerGroup = "", float volume = 1, float pitch = 1, bool startOn = false)
	{
		if (clip != null)
		{
			AudioSource source = gameObject.AddComponent<AudioSource>();
			source.clip = clip;
			if (mixerGroup != "")
				source.outputAudioMixerGroup = TheCubeGameManager.GetMixerGroup(mixerGroup);
			source.playOnAwake = startOn;
			source.volume = volume;
			source.pitch = pitch;
			return source;
		}
		else
		{
			Debug.LogError("clip given was null, cannot set AudioSource.");
			return null;
		}
	}


	private void OnEnable()
	{
		SaveGameManager.SaveComplete += UpdateSettings;
	}
	private void OnDisable()
	{
		SaveGameManager.SaveComplete -= UpdateSettings;
	}
	void UpdateSettings()
	{
		gameSettings = SaveGameManager.GetSave().gameSettings;
		if (cameraFollow != null)
		{
			cameraFollow.sensitivity = gameSettings.sensitivity;
		}
	}

	private void Start()
	{
		gameSettings = SaveGameManager.GetSave().gameSettings;
		newItems = SaveGameManager.GetSave().newItems;
		if (cameraFollow == null && Camera.main != null)
		{
			cameraFollow = Camera.main.gameObject.GetComponent<CameraFollow>();
		}
		if (cameraFollow != null)
		{
			cameraFollow.FollowedObject = gameObject;
			cameraFollow.SnapToObject(true);
			cameraFollow.FollowObject();
			cameraFollow.SnapToObject(false);
			cameraFollow.sensitivity = gameSettings.sensitivity;
			//sets zoom on last level
			//Debug.Log("current" +TheCubeGameManager.CurrentLevel);
			if (TheCubeGameManager.CurrentLevel >= TheCubeGameManager.FinalLevel)
			{
				
				cameraFollow.SetZoom(Vector3.zero);
			}
		}
		//gets the starting EmissionColor
		_playerMesh = TheCubeGameManager.playerMesh;
		if (_playerMesh != null)
		{
			_playerMeshRenderer = _playerMesh.GetComponent<MeshRenderer>();
			if (_playerMeshRenderer != null)
			{
				TheCubeGameManager.UpdatePlayerTexture();
				startingEmmission = _playerMeshRenderer.material.GetColor("_EmissionColor");
				currentEmmission = startingEmmission;
			}
		}

		TheCubeGameManager.turnDownTheRunning = true;
	}

	private void Update()
	{
		//saved inputs
		moveDirection = new Vector3(hInput.GetAxis("Horizontal"), 0, 0);
		float scroll = !gameSettings.inverted ? hInput.GetAxis("ScrollMouse") : -hInput.GetAxis("ScrollMouse");
		zoomDirection = scroll + hInput.GetAxis("ScrollKey");
		//Debug.Log(zoomDirection + " zoom");
		if (movementEnabled)
		{
			if (TheCubeGameManager.CurrentLevel < TheCubeGameManager.FinalLevel && cameraFollow != null)
			{
				cameraFollow.Zoom(zoomDirection);
			}
		}
		OnGround();
		if (!controller.dead)
		{
			FallCheck();
		}
		Sounds();

		if (TheCubeGameManager.CurrentLevel > 0)
		{
			SelfDestruct();
			controller.PlayerShake(new Vector3(1, 0, 0.25f) * destructTime / selfDestructTargetTime * 0.1f, destructTime / selfDestructTargetTime * 10, shaking);
		}
		else { }

		//tranform change check. Had to move it from the camera.
		if (transform.hasChanged)
		{
			//print("I... have.. changed.");
			TransformChanged?.Invoke();
		}
	}
	
	void OnGround()
	{
		//ground stuff
		if (controller.Ground())
		{
			ground = true;
			_extraJumps = 0;
			_jumpDownReset = true;
			_distanceReached = false;
			//only should execute once right when landed.
			if (!_groundOneShot)
			{
				//Debug.Log("Landed.");
				_groundOneShot = true;
			}
			//movingEffects 
			if (controller.moving && !movingEffect.isPlaying)
			{
				movingEffect.Play();
			}
			else if (!controller.moving)
			{
				movingEffect.Stop();
			}
		}
		else
		{
			ground = false;
			_groundOneShot = false;
			movingEffect.Stop();
		}
	}


	void FallCheck()
	{
		//falling stuff
		//fallImminent and falling makes sure it doesn't play more than once

		if (ground && controller.OnEdge() && !fallImminent && !falling && !controller.moving)
		{
			float fallTime;
			fallTime = 0.2f;
			StartCoroutine("FallImminent", fallTime);
		}
		if ((!controller.OnEdge() || _holdingJump) && !falling && fallImminent)
		{
			if (fallingCanvasGroup != null)
			{
				fallingCanvasGroup.alpha = 0;
			}
			StopCoroutine("FallImminent");
			fallImminent = false;
		}
	}


	void Sounds()
	{
		//changes the music when moving.
		//I added the magnitude check to make it start later
		if ((moveDirection.magnitude > 0.8f && controller.moving) || _launching  || onMovingObject)
		{
			StopCoroutine("WaitToTurnDown");
			TheCubeGameManager.turnDownTheRunning = false;
			runningTurnDown = false;
		}
		else
		{
			if (!runningTurnDown)
			{
				StartCoroutine("WaitToTurnDown");
			}
			
		}

		if (controller.moving && !_launching && ground && !TheCubeGameManager.paused)
		{
			if (!movingSource.isPlaying)
			{

				movingSource.PitchShift();
                movingSource.Play();
			}
		}
		else 
		{
			
			movingSource.Stop();
		}
	}
	//waits to turn down the running music
	IEnumerator WaitToTurnDown()
	{
		runningTurnDown = true;
		yield return new WaitForSeconds(4);
		TheCubeGameManager.turnDownTheRunning = true;
		runningTurnDown = false;
	}

	private void FixedUpdate()
	{
		SetJump();
		Launching();
		Bouncing();
		Boosting();
		ColorChange();
		OnMovingObject();

		if (controller.RespawnTriggered) 
		{
			controller.Respawn();
        }

		if (_recoverTriggered && !_launching) 
		{
            controller.Recover();
			_recoverTriggered = false;
        }

		if (!controller.dead && !TheCubeGameManager.paused && TheCubeGameManager.CurrentLevel > 0 && !selfDestruct)
		{
			currentState = CurrentState.alive;
			if (movementEnabled)
			{
				if (TheCubeGameManager.CurrentLevel < TheCubeGameManager.FinalLevel || TheCubeGameManager.CurrentLevel == TheCubeGameManager.FinalLevel + 1)
				{
					cameraFollow.Zoom(zoomDirection);
				}
				controller.Move(cubeInfoSO.speed, moveDirection, movingObjectDirection + _boostingDirection);
				if (gameSettings.rotateOnMove)
				{
					cameraFollow.offsetRotation.y = moveDirection.x * gameSettings.rotateValue;
				}
				else
				{
					cameraFollow.offsetRotation.y = 0;
				}
				if (ground)
				{
					controller.Step(moveDirection, 1f);
				}
				if (_groundJumpActive && !inLauncher && !_inBouncer)
				{
					controller.Jump(_realJumpForce, _holdingJump, ref _distanceReached);
				}
			}
			else
			{
				cameraFollow.offsetRotation.y = 0;
			}
		}

	}


	#region Collisions
	private void OnCollisionEnter(Collision collision)
	{

		//Debug.Log(collision.collider.name + " entered");
		if (!hittingOneShot && _rb.velocity.magnitude > 2f)
		{
			hittingSource.PitchShift();
			hittingSource.Play();
		}
		hittingOneShot = true;

		if (!_holdingJump)
		{
			jumpDown = false;
		}
		//launcher finished
		if (_launching && !inLauncher && currentLauncher != null)
		{
			if (Mathf.Abs(currentLauncher.targetPosition.position.z - transform.position.z) < 1f)
			{
				movementEnabled = true;
				cameraFollow.SnapToObject(false);
				StartCoroutine("Recover", 0);
			}
			_launching = false;
			currentLauncher = null;
		}
		else if (_launching && inLauncher)
		{
			movementEnabled = true;
			_launching = false;
			cameraFollow.SnapToObject(false);
			StartCoroutine("Recover", 0);
		}
		//bouncer check
		if (!_inBouncer && _currentBouncer != null)
		{
			_currentBouncer = null;
			_groundJumpActive = true;
		}
		//respawn complete check
		if (respawning && !controller.dead)
		{
			respawning = false;
			movementEnabled = true;
			cameraFollow.SnapToObject(false);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (falling && !recoverOneShot)
		{
			StartCoroutine("Recover", 1);
			recoverOneShot = true;
		}

		if (!falling && !controller.dead)
		{
			controller.MoveToZ();
		}

		if (falling && controller.dead)
		{
			StopCoroutine("Recover");
		}
		//moving object check. moved here to prevent missing the collision.
		if (!onMovingObject && movingObject != null && collision.gameObject.tag != "Moving")
		{
			//Debug.Log("Left moving object");
			movingObject = null;
			movingObjectDirection = Vector3.zero;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		hittingOneShot = false;
		if (falling)
		{
			StopCoroutine("Recover");
			recoverOneShot = false;
		}
		
	}
	#endregion

	#region Triggers
	private void OnTriggerEnter(Collider other)
	{
		ParticleSystem.VelocityOverLifetimeModule poppedModule = poppedEffect.velocityOverLifetime; //used to change the starting velocity
		#region deaths
		switch (other.tag)
		{
			case "Enemy":
				controller.Dead(cubeInfoSO.deathTime);

                poppedEffect.Play();
				poppedModule.x = controller.VelocityOnDeath.x;
				poppedModule.y = controller.VelocityOnDeath.y;
				poppedModule.z = controller.VelocityOnDeath.z;
				poppedSource.PitchShift(1.3f,1.7f);
				poppedSource.Play();
				
				currentState = CurrentState.popped;
				Debug.Log("Player has died to enemy");
				break;

			case "TheVoid":

				controller.Dead(cubeInfoSO.deathTime, false, false);
				cameraFollow.followObject = false;
				currentState = CurrentState.fell;
				Debug.Log("Player fell..");
				break;

			case "Blaster":

				burnedSource.PitchShift(1.3f,1.7f);
				burnedSource.Play();
				controller.Dead(cubeInfoSO.deathTime);

				//maybe should keep this on player
				Instantiate(cubeInfoSO.burnedEffectsB[0], transform.position, transform.rotation);
				currentState = CurrentState.burned;
				Debug.Log("Player was burned by blaster");
				break;

			case "Pusher":

				currentState = CurrentState.pushed;
				controller.Dead(cubeInfoSO.deathTime, false, false);
				//should simulate the pushing a little better
				controller.GetRigidbody().AddExplosionForce(0.1f, other.transform.parent.position, 0.1f, 0, ForceMode.Impulse);
				//cameraFollow.followObject = false;
				Debug.Log("Player was pushed");
				break;
			case "PushSpike":
				if (currentState != CurrentState.pushed)
				{
					controller.Dead(cubeInfoSO.deathTime);
					poppedEffect.Play();
					poppedModule.x = controller.VelocityOnDeath.x;
					poppedModule.y = controller.VelocityOnDeath.y;
					poppedModule.z = controller.VelocityOnDeath.z;
					poppedSource.PitchShift();
					poppedSource.Play();
					
					currentState = CurrentState.popped;
					Debug.Log("Player has died to pusher spikes");
				}
				break;

			case "Hammer":

				//controller.Dead(cubeInfoSO.deathTime, false, false);
				movementEnabled = false;
				controller.RemoveConstraints();
				falling = true;
				currentState = CurrentState.whacked;
				Debug.Log("Player was hit by hammer");
				break;

			case "Smasher":

				controller.Dead(cubeInfoSO.deathTime, false, false, false);
				if (!ground)
					controller.GetRigidbody().AddForce(Vector3.down * 10, ForceMode.Impulse);

				TheCubeGameManager.playerMesh.transform.localPosition = Vector3.down * 0.5f;
				TheCubeGameManager.playerMesh.transform.localScale = new Vector3(TheCubeGameManager.startScale.x, TheCubeGameManager.startScale.x / 2, TheCubeGameManager.startScale.z / 10);
				currentState = CurrentState.squished;
				Debug.Log("Player was squished by smasher");
				break;

			default:
				break;
		}
		#endregion

		if (other.tag == "Launcher")
		{
			//Debug.Log("inLauncher!");
			Launcher launch = other.GetComponent<Launcher>();
			if (launch != null)
			{
				currentLauncher = launch;
				if (!newItems[0].found)
				{
					TheCubeGameManager.GetHint().DisplayHint(0);
				}
			}
			
			inLauncher = true;
		}

		if (other.tag == "Bouncer")
		{
			//Debug.Log("inBouncer!");
			Bouncer bouncer = other.GetComponent<Bouncer>();
			if (bouncer != null)
			{
				_currentBouncer = bouncer;
				if (!newItems[1].found)
				{
					TheCubeGameManager.GetHint().DisplayHint(1);
				}
			}
			_groundJumpActive = false;
			_inBouncer = true;
		}

		if (other.tag == "Booster")
		{
			//Debug.Log("SPEEDBOOST!");
			_inBooster = true;
		}

		if (other.tag == "Moving")
		{
			//Debug.Log("inMovingObject");
			onMovingObject = true;
			WaypointNav movingObject = other.GetComponent<WaypointNav>(); 
			if (movingObject != null)
			{
				this.movingObject = movingObject;
			}
		}
		if (other.tag == "CameraEdit")
		{
			if (gameSettings.activeCamera)
			{ 
				_currentCameraEdit = other.GetComponent<CameraEditSettings>();
				if (_currentCameraEdit != null)
				{
					//Debug.Log("moving camera..");
					cameraFollow.SetOffset(_currentCameraEdit.newCameraOffset);
				}
			}
		}

	}
	
	private void OnTriggerExit(Collider other)
	{
		if (other.tag == "Launcher")
		{
			//Debug.Log("leftLaucher!");
			inLauncher = false;
		}
		if (other.tag == "Bouncer")
		{
			//Debug.Log("leftBouncer!");
			_inBouncer = false;
		}
		if (other.tag == "Booster")
		{
			//Debug.Log("enoughBoosting!");
			_inBooster = false;
		}
		if (other.tag == "Moving")
		{
			//Debug.Log("leftMovingObject!");
			onMovingObject = false;
		}
		if (other.tag == "CameraEdit")
		{

			//Debug.Log("resetting Camera..");
			cameraFollow.SetToDefault();
			
		}
	}
	#endregion

	void SelfDestruct()
	{
		//suicide countdown..
		if (hInput.GetAxis("Self Destruct") >= 1 && !controller.dead && !controller.VelocityCheck() && controller.Ground())
		{
			selfDestruct = true;
			destructTime += 1 * Time.deltaTime;
			currentEmmission.r = destructTime / selfDestructTargetTime * 0.25f;
			//if (destructTime >= selfDestructTargetTime * 0.5f)
			//{
			shaking = true;
			//}
		}
		else if (destructTime > 0)
		{
			selfDestruct = false;
			shaking = false;
			destructTime -= 1 * Time.deltaTime;
			currentEmmission.r = destructTime / selfDestructTargetTime * 0.25f;
		}
		
		//killing

		if (destructTime >= selfDestructTargetTime)
		{
			poppedEffect.Play();
            poppedSource.PitchShift();
            poppedSource.Play();
			controller.Dead(cubeInfoSO.deathTime);
			currentState = CurrentState.popped;
			selfDestruct = false;
			shaking = false;
			destructTime = 0;
			currentEmmission = startingEmmission;
			Debug.Log("...");
		}

		//clampValue
		if (destructTime < 0)
		{
			destructTime = 0;
		}
	}

	void SetJump()
	{

		//sets up the jumpdown holding jump and other jump variables

		//jump input
		if (hInput.GetAxis("Jump") != 0)
		{
			_holdingJump = true;
			jumpDown = false;
			if (_jumpDownReset)
			{
				jumpDown = true;
			}
			_jumpDownReset = false;
		}
		else
		{
			_distanceReached = true;
			_realJumpForce = 0;
			_holdingJump = false;
			_jumpDownReset = true;
		}

		if (!jumpDown)
		{
			_jumpOneShot = false;
				
		}

		if (jumpDown && ground && !_jumpOneShot)
		{
			//Debug.Log("hop");
			_realJumpForce = 1f;
			_positionBeforeJump = transform.position;
			_distanceReached = false;
			_jumpOneShot = true;
		}
		//extra jumps
		else if (jumpDown && ground && _extraJumps < cubeInfoSO.maxExtraJumps)
		{
			//Debug.Log("extra hop");
			_realJumpForce = 1f;
			_extraJumps++;
			_positionBeforeJump = transform.position;
			_distanceReached = false;
			_jumpOneShot = true;
		}

		//gets the jump to increase to top speed in roughly a second/jumpForceIncreaseMultiplier.
		if (_realJumpForce < cubeInfoSO.jumpForce)
		{

			_realJumpForce += cubeInfoSO.jumpForce * _jumpForceIncreaseMultiplier * Time.deltaTime;
		}
		else
		{
			_realJumpForce = cubeInfoSO.jumpForce;
		}

		//stops jump when the the distance between the last point before jumping and player is greater than the set jumpHieght

		if (transform.position.y - _positionBeforeJump.y > targetHeight)
		{
			_distanceReached = true;
		}
		
	}


	void Launching()
	{
		if (currentLauncher != null)
		{
			if (inLauncher && jumpDown && ground && currentLauncher.activateOnJump)
			{
				if (!newItems[0].found)
				{
					TheCubeGameManager.GetHint().Cancel();
					newItems[0].found = true;
					//SaveGameManager.GetSave().newItems = newItems;
				}
				
				_launching = true;
				movementEnabled = false;
				controller.RemoveConstraints();
				currentLauncher.Launch(gameObject);
				controller.positionZ = currentLauncher.GetDistanceZ();
				_rb.AddTorque(Vector3.right * currentLauncher.direction.z * 2f);
			}
			else if (inLauncher && ground && !currentLauncher.activateOnJump)
			{
				_launching = true;
				movementEnabled = false;
				controller.RemoveConstraints();
				currentLauncher.Launch(gameObject);
				controller.positionZ = currentLauncher.GetDistanceZ();
				_rb.AddTorque(Vector3.right * currentLauncher.direction.z * 2f);
			}			
		}
		if (_launching)
		{
			cameraFollow.SnapToObject(true);
		}
		
	}


	void Bouncing()
	{
		if (_currentBouncer != null)
		{
			if (_inBouncer && jumpDown && ground)
			{
				if (!newItems[1].found)
				{
					TheCubeGameManager.GetHint().Cancel();
					newItems[1].found = true;
					//SaveGameManager.GetSave().newItems = newItems;
				}
				_currentBouncer.Bounce(controller.GetRigidbody());
			}

		}
	}


	void Boosting()
	{
		float fadeSpeed = 0.1f;
		if (destructTime <= 0)//allows me to increase emmision while self destructing
		{
			if (_inBooster && controller.moving)
			{
				//emmision change
				if (currentEmmission.r < 0.25f)
				{
					currentEmmission.r += fadeSpeed * Time.deltaTime;
				}
				//pitch change
				if (movingSource.pitch < 3f)
				{
					movingSource.pitch += cubeInfoSO.speedBoostSpeed * Time.deltaTime;
				}

				//right
				if (moveDirection.x > 0)
				{
					if (_currentBoostingSpeed < cubeInfoSO.speedBoostSpeed)
					{
						//speed it up
						_currentBoostingSpeed += cubeInfoSO.speedBoostSpeed * 2 * Time.deltaTime;
					}
					else
					{
						//clamp it
						_currentBoostingSpeed = cubeInfoSO.speedBoostSpeed;
					}
				}
				//left
				if (moveDirection.x < 0)
				{
					if (_currentBoostingSpeed > -cubeInfoSO.speedBoostSpeed)
					{
						//slow it down
						_currentBoostingSpeed -= cubeInfoSO.speedBoostSpeed * 2 * Time.deltaTime;
					}
					else
					{
						//clamp it
						_currentBoostingSpeed = -cubeInfoSO.speedBoostSpeed;
					}
				}

				//if not moving then wont save move value so saves before stopping to make it slide a bit

				if (moveDirection.magnitude > 0)
				{
					boostRaw = moveDirection.Raw();
				}
				
			}
			else if (Mathf.Abs(_currentBoostingSpeed) > 0.1f)//for when you aren't holding down move
			{
				if (currentEmmission.r > 0)
				{
					currentEmmission.r -= fadeSpeed * Time.deltaTime;
				}
				//slow it down 
				//right
				if (boostRaw.x > 0)
				{
					if (_currentBoostingSpeed > 0)
					{
						//slow down
						_currentBoostingSpeed -= cubeInfoSO.speedBoostSpeed * 4 * Time.deltaTime;
					}
					else
					{
						//clamp it
						_currentBoostingSpeed = 0;
					}
				}
				//left
				else if (boostRaw.x < 0)
				{
					if (_currentBoostingSpeed < 0)
					{
						_currentBoostingSpeed += cubeInfoSO.speedBoostSpeed * 4 * Time.deltaTime;
					}
					else
					{
						//clamp it
						_currentBoostingSpeed = 0;
					}
				}

				if (movingSource.pitch > 1)
				{
					movingSource.pitch -= cubeInfoSO.speedBoostSpeed * Time.deltaTime;
				}
			}
			else
			{
				//stop it
				movingSource.pitch = 1;
				_currentBoostingSpeed = 0;
				if (currentEmmission.r > 0)
				{
					currentEmmission.r -= fadeSpeed * Time.deltaTime;
				}
				else
				{
					currentEmmission = startingEmmission;
				}
			}
		}
		//apply
		_boostingDirection = Vector3.right * _currentBoostingSpeed;
	}


	void OnMovingObject()
	{
		if (movingObject != null && onMovingObject && ground)
		{
			movingObjectDirection = movingObject.GetVelocity();
			oldMovingObjectDirection = movingObject.GetVelocity();
		}
		else if (movingObject != null)
		{
			
			//if (movingObjectDirection != Vector3.zero)
			//{
			//	//Debug.Log("SLOWING DOWN!");
			//	//movingObjectDirection += -oldMovingObjectDirection * 0.1f * Time.deltaTime;
			//}
			//else
			//{
			//	movingObjectDirection = Vector3.zero;
			//}
		}
		
		if (controller.dead)
		{
			onMovingObject = false;
			movingObjectDirection = Vector3.zero;
		}
	}


	IEnumerator Recover(float recoverTime)
	{
		yield return new WaitForSeconds(recoverTime);
		_recoverTriggered = true;
		falling = false;
		movementEnabled = true;
		recoverOneShot = false;
	}

	public void DeactivateMovement()
	{
		movementEnabled = false;
		_rb.velocity = Vector3.zero;
		controller.moving = false;
	}
	public void ActivateMovement()
	{
		movementEnabled = true;
	}

	public void SetToKinematic(bool kinematic)
	{
		controller.GetRigidbody().isKinematic = kinematic;
	}

	IEnumerator FallImminent(float waitTime)
	{
		if (fallingCanvasGroup != null)
		{
			fallingCanvasGroup.alpha = 1;
		}
		fallImminent = true;
		yield return new WaitForSeconds(waitTime);
		Debug.Log("player fell off edge.");
		movementEnabled = false;
		controller.RemoveConstraints();
		fallImminent = false;
		falling = true;

		if (fallingCanvasGroup != null)
		{
			fallingCanvasGroup.alpha = 0;
		}
	}


	void ColorChange()
	{
		if (_playerMeshRenderer != null)
		{

			//if(SaveGameManager.GetSave().gameSettings != null)
				_playerMeshRenderer.material.color = gameSettings.normalPlayerColor;
			_playerMeshRenderer.material.SetColor("_EmissionColor", currentEmmission);
		}
	}

	//eventChecking

	void OnRespawn()
	{
		respawning = true;
		falling = false;
		TheCubeGameManager.playerMesh.transform.localPosition = Vector3.zero;
		TheCubeGameManager.playerMesh.transform.localScale = TheCubeGameManager.startScale;
		cameraFollow.SetToDefault();
		cameraFollow.followObject = true;
		cameraFollow.SnapToObject(true);
		cameraFollow.FollowObject();
		cameraFollow.SnapToObject(false);

	}

	
}
