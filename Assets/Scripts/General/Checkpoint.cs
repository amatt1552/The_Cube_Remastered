using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
	[Header("Set in inspector")]
	[SerializeField]
	public Transform spawnPoint;
	public MeshRenderer[] lights;
	public Color deactivatedColor = Color.red;
	public Color activatedColor = Color.green;
	bool _activated;
	public bool activatedAtStart;

	private void Start()
	{
		if (activatedAtStart)
		{
			Activate();
			GameObject player = TheCubeGameManager.player;
			if (player != null)
			{
				ResetRespawn(player);
			}
		}
		else
		{
			Deactivate();
		}
		CheckpointManager.AddCheckpoint(this);
		GetComponent<Collider>().isTrigger = true;
	}

	/// <summary>
	/// checks if checkpoint is active
	/// </summary>
	/// <returns></returns>
	public bool IsActive()
	{
		return _activated;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Activate();
			ResetRespawn(other.gameObject);
		}
	}

	void ResetRespawn(GameObject player)
	{
		CubeController controller = player.GetComponent<CubeController>();
		if (controller != null && spawnPoint != null)
		{
			controller.respawnPoint = spawnPoint;
			controller.positionZ = spawnPoint.position.z;
		}
	}

	public void Activate()
	{
		CheckpointManager.DeacitivateLastActiveCheckpoint();
		CheckpointManager.SetLastActive(this);
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].material.SetColor("_EmissionColor", activatedColor);
		}
		_activated = true;

	}
	public void Deactivate()
	{
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].material.SetColor("_EmissionColor", deactivatedColor);
		}

		_activated = false;
	}
}
