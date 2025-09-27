using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadingAction : ActionQueueBase
{
	public Fader fader;
	
	protected override void Awake()
	{
		base.Awake();
		if (fader == null)
		{
			enabled = false;
		}
	}
	protected override void FixedUpdate()
	{
		base.FixedUpdate();

	}

	protected override void Fade(GameObject fadedObject, Color targetColor, float targetTime)
	{
		TMP_Text text = fadedObject.GetComponent<TMP_Text>();
		if (text != null)
		{
			fader.FadeTo(text, targetColor, targetTime);
		}
	}
}
