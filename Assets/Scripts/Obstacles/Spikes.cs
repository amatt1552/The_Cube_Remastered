using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class Spikes : MonoBehaviour
{
	public GameObject spike;
	public GameObject spikeBase;
	GameObject _spikeHolder;
	public string tagName = "Enemy";
	public Vector3 size = Vector3.one;
	public float separation = 0.5f;
	public bool setSizeToObjectScale;

	float yPosition;
	float _spikesX, _spikesZ;
	[Tooltip("Set to -1 to not hide spikes")]
	public int hideSpikeDistance = 20;
	CubeMotor playerMotor;

	void Start()
	{
		//I do this in case there is already one there so it still gets destroyed

		Transform spikeHolderTrans = transform.Find("spikeHolder");
		if (spikeHolderTrans != null)
		{
			_spikeHolder = spikeHolderTrans.gameObject;
		}
		if (spikeBase == null)
		{
			Debug.Log("spikeBase not set. Creating empty.");
			spikeBase = new GameObject("spikeBase");
			spikeBase.transform.parent = transform;
			spikeBase.transform.localPosition = new Vector3(0, 0, 0);
		}

		
	}

	void Update()
	{
		if (Application.isEditor && spike != null)
		{
			if (setSizeToObjectScale)
			{
				size = spikeBase.transform.localScale;
			}
		}
	}

	void CalculateSpikeCount()
	{
		if (separation != 0)
		{
			//ex
			//size.x = 1, separation = 1
			//spikesX = 1
			_spikesX = Mathf.Floor(size.x / separation);
			//size.z = 2.6, separation = 1
			//spikesZ = 2
			_spikesZ = Mathf.Floor(size.z / separation);
			
		}
		else
		{
			_spikesX = 0;
			_spikesZ = 0;
		}
	}

	public void RegenerateSpikes()
	{
		//changes size of the base for spikes
		if (!setSizeToObjectScale)
		{
			spikeBase.transform.localScale = size;
		}
		
		//sets spikes to be on top of base
		yPosition = size.y / 2;

		//gets the amount of spikes i'll need in the loop
		CalculateSpikeCount();

		//resets spikes
		if (_spikeHolder != null)
		{
			DestroyImmediate(_spikeHolder);
		}
		if (_spikeHolder == null)
		{
			_spikeHolder = new GameObject("spikeHolder");
			_spikeHolder.transform.parent = transform;
			//set the y here so it will sit on top of base
			_spikeHolder.transform.localPosition = new Vector3(0, yPosition, 0);
			
		}

		//gets the offset from pivot
		Vector3 offset = transform.position - spikeBase.transform.position;

		Vector3 currentPosition;

		//starting position y and z
		currentPosition.y = -offset.y;
		currentPosition.z = -((size.z - separation) / 2) - offset.z;

		for (float i = 0; i < _spikesZ; i++)
		{
			//starting position x
			currentPosition.x = -((size.x - separation) / 2) - offset.x;

			for (float ii = 0; ii < _spikesX; ii++)
			{

				GameObject newSpike = Instantiate(spike);
				newSpike.transform.parent = _spikeHolder.transform;
				newSpike.transform.localPosition = currentPosition;
				//move to next x position
				currentPosition.x += separation;
			}

			//move to next z position
			currentPosition.z += separation;
		}
		
		//rotates spikes in right direction
		_spikeHolder.transform.localRotation = Quaternion.identity;
	}

	//only need a minor update so decided to use a delegate in cameraFollow. This was ruined with the zoom so now its in CubeMotor.

	private void OnEnable()
	{
		GameObject player = TheCubeGameManager.player;
		if (player != null)
		{
			playerMotor = player.GetComponent<CubeMotor>();
		}
		if (playerMotor != null)
		{
			playerMotor.TransformChanged += HideSpikes;
		}
		
	}
	

	private void OnDisable()
	{
		if (playerMotor != null)
		{
			playerMotor.TransformChanged -= HideSpikes;
		}
	}

	void HideSpikes()
	{
		if (_spikeHolder != null &&  hideSpikeDistance != -1 && spikeBase != null)
		{
			float distance = Vector3.Distance(spikeBase.transform.position, playerMotor.transform.position);

            //Debug.Log("Hiding Spikes");
            _spikeHolder.SetActive(distance < hideSpikeDistance);
		
		}
	}

	
}
