using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
	static CheckpointManager _S;
	List<Checkpoint> checkpoints = new List<Checkpoint>();
	static Checkpoint _LAST_ACTIVE;

	void Awake ()
	{
		if (_S == null)
		{
			_S = this;
		}
	}

	public static void AddCheckpoint(Checkpoint checkpoint)
	{
		if (!_S.checkpoints.Contains(checkpoint))
		{
			_S.checkpoints.Add(checkpoint);
		}
	}

	public static void RemoveAllCheckpoints()
	{
		_S.checkpoints.Clear();

	}

	/// <summary>
	/// makes sure only one checkpoint is active.
	/// call it in the checkpoint when set active.
	/// </summary>
	public static void DeacitivateAllOtherCheckpoints(Checkpoint activeCheckpoint)
	{
		foreach (Checkpoint checkpoint in _S.checkpoints)
		{
			if(activeCheckpoint != checkpoint)
			{
				checkpoint.Deactivate();
			}
		}
	}

	public static void SetLastActive(Checkpoint checkpoint)
	{
		_LAST_ACTIVE = checkpoint;
	}

	/// <summary>
	/// makes sure only one checkpoint is active.
	/// call it in the checkpoint when set active.
	/// </summary>
	public static void DeacitivateLastActiveCheckpoint()
	{
		if (_LAST_ACTIVE != null)
		{
			_LAST_ACTIVE.Deactivate();
			_LAST_ACTIVE = null;
		}
	}
}
