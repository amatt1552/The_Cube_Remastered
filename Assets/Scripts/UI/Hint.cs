using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hint : MonoBehaviour
{
	//UI
	public TMP_Text title;
	public TMP_Text description;
	public CanvasGroup canvasGroup;

	//timing
	const float FADE_TIME = 0.4f;
	public float timeBeforeDisplay = 6;
	Fader fader;

	//hint text
	[SerializeField]
	HintContent[] hintContent;

	//hint image
	UIAnimator animator;

	//needed for UIAnimator
	int currentIndex;

	private void Awake()
	{
		fader = gameObject.AddComponent<Fader>();
	}

	//function to set current values and start displaying it
	public void DisplayHint(int index)
	{
		try
		{
			StopCoroutine("DisplayHintEnum");
			StopCoroutine("hideHint");
			title.text = hintContent[index].title;
			description.text = hintContent[index].description;
			Transform trans = canvasGroup.transform.Find("Activate");
			if(trans != null)
			{

				animator = trans.gameObject.AddComponent<UIAnimator>();
				animator.framesPerSecond = 15;
				animator.path = hintContent[index].imagePath;
				animator.startOnAwake = false;
				animator.parentTransform = canvasGroup.transform;
				animator.TryPath();
				animator.StopCheckDistance();
			}
			StartCoroutine("DisplayHintEnum", hintContent[index].displayTime);
		}
		catch
		{
			Debug.LogWarning("Hint not set properly! Did you set the title and description? set the hint? Pick the correct index?");
		}
	}
	
	//cancel if they figure it out
	public void Cancel()
	{
		StopAllCoroutines();
		StartCoroutine("HideHint");
	}

	//enum for hidingHint
	IEnumerator HideHint()
	{
		if (canvasGroup != null)
		{
			fader.FadeTo(canvasGroup, 0, FADE_TIME, true);
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			while (fader.fading)
			{
				yield return null;
			}
			canvasGroup.gameObject.SetActive(false);
		}
		else
		{
			gameObject.SetActive(false);
		}
		if (animator != null)
		{
			Destroy(animator);
		}
	}

	//main enum for hint
	IEnumerator DisplayHintEnum(float time)
	{
		//wait before starting
		yield return new WaitForSeconds(timeBeforeDisplay);
		//fade in
		if (canvasGroup != null)
		{
			canvasGroup.gameObject.SetActive(true);
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			fader.FadeTo(canvasGroup, 1, FADE_TIME, true);
		}
		else
		{
			gameObject.SetActive(true);
		}

		//wait
		yield return new WaitForSeconds(time);

		//fadeout
		StartCoroutine("HideHint");
		
	}

	[System.Serializable]
	class HintContent
	{
		public string title = "I'm a Hint!";
		public string description = "Do what I say and you'll win!";
		public string imagePath;
		public float displayTime = 5;
	} 
}
