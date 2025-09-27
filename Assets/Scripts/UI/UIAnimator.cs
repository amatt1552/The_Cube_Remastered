using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//updates an image using invoke instead of the animator.
public class UIAnimator : MonoBehaviour
{
	public Image image;
	public bool startOnAwake = true;
	public bool loop = true;
	public float framesPerSecond = 60;//a fake fps
	public Transform parentTransform;
	public string path = "Animations";
	public Sprite[] sprites;
	bool running;
	int index;

	void Start()
    {
		if (image == null)
		{
			image = GetComponent<Image>();
		}
		if (sprites != null)
		{
			TryPath();
		}
		StopCheckDistance();

	}

	public void TryPath()
	{
		try
		{
			sprites = Resources.LoadAll<Sprite>(path);
		}
		catch { }
	}

	private void OnEnable()
	{
		StopCheckDistance();
	}

	private void OnDisable()
	{
		Stop();
	}
	
	void RunAnim()
    {
		//display image
		image.sprite = sprites[index];
		//increment
		index++;
		//resets if loop, stops if not
		if (loop)
		{
			index = index >= sprites.Length ? 0 : index;
		}
		else if(index >= sprites.Length)
		{
			Stop();
		}
	}

	public void StopCheckDistance()
	{
		if (parentTransform != null)
		{
			if (Vector3.Distance(parentTransform.position, transform.position) > 50)
			{
				Stop();
			}
			else
			{
				Play();
			}
		}
	}

	public void Play()
	{
		if (!running && framesPerSecond > 0  && image != null && sprites.Length > 0)
		{
			InvokeRepeating("RunAnim", 0, 1/framesPerSecond);
			running = true;
		}
	}

	public void Stop()
	{
		CancelInvoke("RunAnim");
		if (image != null && sprites != null)
		{
			index = 0;
			image.sprite = sprites[index];
		}
		running = false;
	}
}
