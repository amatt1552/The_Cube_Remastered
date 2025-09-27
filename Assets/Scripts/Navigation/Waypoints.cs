using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
	public Transform[] connectedPoints;
	public bool debugging = true;
	private void OnDrawGizmos()
	{
		if (debugging)
		{
			for (int i = 0; i < connectedPoints.Length; i++)
			{
				if (i + 1 < connectedPoints.Length)
				{
					Debug.DrawLine(connectedPoints[i].position, connectedPoints[i + 1].position);
				}
			}
		}
	}
	/// <summary>
	/// -1 means you're stupid (out of range)
	/// </summary>
	/// <param name="indexA"></param>
	/// <param name="indexB"></param>
	/// <returns></returns>
	public float DistanceFromPoints(int indexA, int indexB)
	{
		if (indexA < connectedPoints.Length && indexB < connectedPoints.Length)
		{
			return Vector3.Distance(connectedPoints[indexA].position, connectedPoints[indexB].position);
		}
		return -1;
	}
}
