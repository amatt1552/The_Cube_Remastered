using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrainingTextFade : MonoBehaviour 
{

	public Color offAlphaLimit;
	public Color onAlphaLimit;
	public float distanceActivated;
	Color mainColor;
	GameObject player;
	public Text text;

	void Start () 
	{
		text.color = offAlphaLimit;
	}
	
	void Update()
	{
		player = GameObject.Find("player");
		float distance = Mathf.Abs (transform.position.x - player.transform.position.x);
		if(distance == 0)
		{
			distance = 0.1f;
		}

		if(distance > 0 && distance < distanceActivated)
		{
			mainColor = new Color(text.color.r, text.color.g, text.color.b, 1/distance);
		}
		else if(distance > distanceActivated)
		{
			mainColor = offAlphaLimit;
		}

		text.color = mainColor;

	}
}
