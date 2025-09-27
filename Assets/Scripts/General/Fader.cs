using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fader : MonoBehaviour
{
	//I consider this to be obsolete. Look at the new code section.

	public CanvasGroup canvasGroup;
	public bool fadingAlpha = true;
	[Tooltip("if set to true object will be set to be interactable after alpha = 0")]
	public bool interactibleAfterComplete;

	//I want to use these later. for now I'm focusing on the fading to alpha.

	public Color startingColor = Color.white;
	public Color endingColor = Color.black;

	public float fadeSpeed = 1;

	//[HideInInspector] //hide this variable when done testing
	public bool fading;

	//lets me start both.. 
	bool fadingStart, fadingEnd;
	
	public void FadeToStart()
	{
		if (!fadingStart)
		{
			if (fadingEnd)
			{
				fadingEnd = false;
				StopCoroutine("FadeToEnd");
			}
			StartCoroutine("FadeToStartEnum");
		}
	}

	IEnumerator FadeToStartEnum()
	{
		if (canvasGroup != null)
		{
			if (fadingAlpha)
			{
				while (canvasGroup.alpha < 1)
				{
					fading = true;
					fadingStart = true;
					canvasGroup.alpha += fadeSpeed * Time.deltaTime;
					yield return null;
				}
				canvasGroup.alpha = 1;
				canvasGroup.interactable = interactibleAfterComplete;
				fading = false;
				fadingStart = false;
			}
			else
			{
				//i'd do the fading between start and end Colors
			}
		}
	}

	public void FadeToEndActivate()
	{
		if (!fadingEnd)
		{
			if (fadingStart)
			{
				fadingStart = false;
				StopCoroutine("FadeToStartEnum");
			}
			StartCoroutine("FadeToEnd");
		}
	}
	
	IEnumerator FadeToEnd()
	{
		if (canvasGroup != null)
		{
			if (fadingAlpha)
			{
				while (canvasGroup.alpha > 0)
				{
					fading = true;
					fadingEnd = true;
					canvasGroup.alpha -= fadeSpeed * Time.deltaTime;
					yield return null;
				}
				canvasGroup.alpha = 0;
				canvasGroup.interactable = interactibleAfterComplete;
				fading = false;
				fadingEnd = false;
			}
			else
			{
				//i'd do the fading between start and end Colors
			}
		}
	}

	//------------------------------New Code------------------------------
	//I will eventually switch over to this new code for everything but for now I will keep the old functions.
	//The purpose of this script is to fade pretty much anything fadable.
	//usually will simply be fading the colors to a target color.
	//might make these extensions instead..
	
	bool fadingStarted;

	#region Fading functions
	//text
	public void FadeTo(TMP_Text text, Color targetColor, float targetTime, bool stopCoroutinesAtStart = false)
	{
		if (stopCoroutinesAtStart)
		{
			StopAllCoroutines();
			fadingStarted = false;
			fading = false;
		}
		if (!fadingStarted)
		{
			fading = true;
			StartCoroutine(FadingEnum(text,targetColor,targetTime)); 
			fadingStarted = true;
		}
	}

	//sound 
	public void FadeTo(AudioSource source, float targetVolume, float targetTime, bool stopCoroutinesAtStart = false)
	{
		if (stopCoroutinesAtStart)
		{
			StopAllCoroutines();
			fadingStarted = false;
			fading = false;
		}
		if (!fadingStarted)
		{
			fading = true;
			StartCoroutine(FadingEnum(source, targetVolume, targetTime));
			fadingStarted = true;
		}
	}

	//CanvasGroup
	public void FadeTo(CanvasGroup canvasGroup, float targetAlpha, float targetTime, bool stopCoroutinesAtStart = false)
	{
		if (stopCoroutinesAtStart)
		{
			StopAllCoroutines();
			fadingStarted = false;
			fading = false;
		}
		if (!fadingStarted)
		{
			fading = true;
			StartCoroutine(FadingEnum(canvasGroup, targetAlpha, targetTime));
			fadingStarted = true;
		}
	}
	#endregion

	#region FadeInCoroutines
	//text
	IEnumerator FadingEnum(TMP_Text text, Color targetColor, float targetTime)
	{
		Color oldcolor;
		float currentTime = 0;
		float currentAlpha = 0;
		if (text != null)
		{
			oldcolor = text.color;
			while (currentTime <= targetTime)
			{
				currentTime += 1 * Time.deltaTime;
				if(targetTime != 0)
					currentAlpha += 1/targetTime * Time.deltaTime; //needed to change fadespeed to meet targetTime.
				text.color = Color.Lerp(oldcolor, targetColor, currentAlpha);
				yield return null;
			}
			text.color = targetColor;			
		}
		currentTime = 0;
		fading = false;
		fadingStarted = false;
	}

	//sound
	IEnumerator FadingEnum(AudioSource source, float targetVolume, float targetTime)
	{
		float oldVolume;
		float currentTime = 0;
		float currentAlpha = 0;
		if (source != null)
		{
			oldVolume = source.volume;
			while (currentTime <= targetTime)
			{
				currentTime += 1 * Time.deltaTime;
				if (targetTime != 0)
					currentAlpha += 1/targetTime * Time.deltaTime; //needed to change fadespeed to meet targetTime.
				source.volume = Mathf.Lerp(oldVolume, targetVolume, currentAlpha);
				yield return null;
			}
			source.volume = targetVolume;
		}
		currentTime = 0;
		fading = false;
		fadingStarted = false;
	}

	//CanvasGroup
	IEnumerator FadingEnum(CanvasGroup canvasGroup, float targetAlpha, float targetTime)
	{
		float oldAlpha;
		float currentTime = 0;
		float currentAlpha = 0;
		if (canvasGroup != null)
		{
			oldAlpha = canvasGroup.alpha;
			while (currentTime <= targetTime)
			{
				currentTime += 1 * Time.deltaTime;
				if (targetTime != 0)
					currentAlpha += 1/targetTime * Time.deltaTime; //needed to change fadespeed to meet targetTime.
				canvasGroup.alpha = Mathf.Lerp(oldAlpha, targetAlpha, currentAlpha);
				yield return null;
			}
			canvasGroup.alpha = targetAlpha;
		}
		currentTime = 0;
		fading = false;
		fadingStarted = false;
	}
	#endregion


}
