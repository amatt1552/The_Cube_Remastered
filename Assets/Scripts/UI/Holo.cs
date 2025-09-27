using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Holo : MonoBehaviour 
{
	public Color darkColor;
	public Color brightColor;
	public float holoRate;
	public Material currentMat;
	public Text currentText;

	void Start () 
	{
		 
	}
	
	void Update () 
	{
		if(currentMat != null)
		{
			currentMat.color = Color.Lerp(darkColor, brightColor, Mathf.PingPong(Time.time * holoRate, 1));
		}
		if(currentText != null)
		{
			currentText.color = Color.Lerp (darkColor, brightColor, Mathf.PingPong(Time.time * holoRate, 1));
		}
	}
}
