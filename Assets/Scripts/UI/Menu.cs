using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{
	int currentPoint;
	[Header("0 = Menu, 1 = Selection, 2 = Settings")]
	public Transform[] paths;

	//---------------------------------------------------

	void Awake () 
	{
		currentPoint = 0;
		MoveToPoint();
	}
	
	//---------------------------------------------------

	void FixedUpdate() 
	{
		MoveToPoint();
	}

	//---------------------------------------------------

	public void ToMenu()
	{
		currentPoint = 0;
	}

	//---------------------------------------------------

	public void ToSelection()
	{
		currentPoint = 1;
	}

	//---------------------------------------------------

	public void ToSettings()
	{
		currentPoint = 2;
	}

	public void MoveToPoint()
	{
		transform.position = Vector3.MoveTowards(transform.position, paths[currentPoint].transform.position, 400f * Time.deltaTime);
		transform.rotation = paths[currentPoint].transform.rotation;
	}
}
