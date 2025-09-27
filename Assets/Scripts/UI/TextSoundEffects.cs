using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextSoundEffects : MonoBehaviour
{
	public AudioSource hoverSound;
	public AudioSource clickSound;
	
	/// <summary>
	/// 0 = hover, 1 = click
	/// </summary>
	/// <param name="i"></param>
	public void PlaySound(int i)
	{
		switch (i)
		{
			case 0:
				if (hoverSound != null)
				{
					if (hoverSound.isPlaying)
					{
						hoverSound.Stop();
					}
					hoverSound.PitchShift(0.8f,0.9f);
					hoverSound.Play();
				}
				break;
			case 1:
				if (clickSound != null)
				{
					if (clickSound.isPlaying)
					{
						clickSound.Stop();
					}
					clickSound.PitchShift(0.7f,0.8f);
					clickSound.Play();
				}
				break;
			default:
				break;
		}


	}



}
