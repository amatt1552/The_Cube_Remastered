using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;

public class Slideshow : MonoBehaviour
{
	public RectTransform movedUI;
	[Tooltip("Guesses move distance based on scale on the x")]
	public bool guessMoveDistance = true;
	public float moveDistance;
	public Vector3 moveDirection = Vector3.right;
	public float speed = 100;
	[Tooltip("Determines when to reset based on moveDistance")]
	public bool setByMoveDistance = true;
	public Vector3 maxLimits;
	public Vector3 minLimits;
	public bool auto = true;
	public float autoTime = 8;
	public bool startOnAwake;

	public int CurrentIndex { get; private set; } = 1;
	int count;
	public bool IsEnabled { get; private set; }
	Vector3 startPos;
	Vector3 targetPos;
	List<RectTransform> contentsBackground;
	List<TMP_Text> contentsText;
	const float DIST_TO_POINT = 0.1f;
	bool enumRunning;

	[SerializeField]
	[Tooltip("Finds a parent with a canvas by default.")]
	GameObject hiddenObject;
	bool resetDown;
	bool autoEnumRunning;

	//events
	public UnityEvent OnStop;

	void Awake()
	{
		contentsBackground = new List<RectTransform>();
		contentsText = new List<TMP_Text>();
		//check if movedUI exists
		if (movedUI != null)
		{
			//get image and text
			FindContent();
			startPos = movedUI.localPosition;
		}
		targetPos = startPos;
		
		//find the proper parent.
		
		if(hiddenObject == null)
		{
			if (gameObject.GetComponent<Canvas>())
			{
				hiddenObject = gameObject;
			}
			else
			{
				GameObject tempObj = transform.FindParentOfType(typeof(Canvas));
				//Debug.Log(tempObj.name);
				hiddenObject = tempObj == null ? null : tempObj;
				if (hiddenObject == null)
				{
					Debug.LogWarning("could not find canvas!");
				}
			}
		}
		if (startOnAwake)
		{
			StartCoroutine("AutoSlideshow");
		}

		//event init
		if (OnStop == null)
		{
			OnStop = new UnityEvent();
		}

	}

	void FindContent()
	{
		contentsBackground = new List<RectTransform>();
		count = 0;
		for (int i = 0; i < movedUI.transform.childCount; i++)
		{
			//get backgrounds
			RectTransform tempRect = movedUI.GetChild(i).GetComponent<RectTransform>();
			
			if (tempRect != null)
			{
				contentsBackground.Add(movedUI.GetChild(i).GetComponent<RectTransform>());
			}
			if (tempRect.gameObject.activeInHierarchy)
			{
				count++;
			}

			//set moveDistance and limits
			if (guessMoveDistance)
			{
				moveDistance = contentsBackground[0].sizeDelta.x;
			}
			if (setByMoveDistance)
			{
				maxLimits = Vector3.zero;
				minLimits = Vector3.one * -moveDistance * (count - 3);
			}

			//get texts
			FindText(i);
		}
	}

	/*public void CheckCount()
	{
		for (int i = 0; i < movedUI.transform.childCount; i++)
		{
			RectTransform tempRect = movedUI.GetChild(i).GetComponent<RectTransform>();
			if (tempRect.gameObject.activeInHierarchy)
			{
				count++;
			}
		}
	}*/

	void FindText(int index)
	{
		if (index < contentsText.Count) //check if already in list
		{
			if (contentsBackground[index].Find("Text") != null)
			{
				contentsText[index] = contentsBackground[index].Find("Text").GetComponent<TMP_Text>();
			}
			else
			{
				contentsText[index] = null;
			}
		}
		else //add to list if not in list
		{
			if (contentsBackground[index].Find("Text") != null)
			{
				
				contentsText.Add(contentsBackground[index].GetComponentInChildren<TMP_Text>());

			}
			else
			{
				contentsText.Add(null);
			}
		}
	}

	public void Increment()
	{
		//subtract from targetPos based on moveDistance and given direction
		targetPos -= moveDirection * moveDistance;
		StartItAlready();
	}
	public void Decrement()
	{
		//add to targetPos based on moveDistance and given direction
		targetPos += moveDirection * moveDistance;
		StartItAlready();
	}
	void StartItAlready()//since the code was the same for increment and decrement decided to make another function.
	{
		//start coroutine if not already started.
		if (!enumRunning)
		{
			StartCoroutine("MoveToPoint");
		}
		if (auto)
		{
			if (!autoEnumRunning)
			{
				StartCoroutine("AutoSlideshow");
			}
			else
			{
				//resets autoSlideshow
				StopCoroutine("AutoSlideshow");
				StartCoroutine("AutoSlideshow");
			}
		}
		else if (autoEnumRunning)
		{
			StopCoroutine("AutoSlideshow");
		}
	}	

	public void Enabled(bool enabled)
	{
		if (hiddenObject != null)
		{
			//enable disable
			
			hiddenObject.SetActive(enabled);
			
			if (enabled)
			{
				FindContent();
				if (!autoEnumRunning)
				{
					StartCoroutine("AutoSlideshow");
				}
			}
			else
			{
				StopCoroutine("AutoSlideshow");
			}
			IsEnabled = enabled;
			
		}
	}

	public void SetText(int index, string newText)
	{
		if (index < contentsText.Count && index >= 0)
		{
			//change text of content
			contentsText[index].text = newText;
		}
	}

	public List<TMP_Text> GetText()
	{
		return contentsText;
	}

	/// <summary>
	/// I wouldn't suggest doing this but it should work..
	/// </summary>
	/// <param name="index"></param>
	/// <param name="newBackground"></param>
	public void SetBackground(int index, RectTransform newBackground)
	{
		if (index < contentsBackground.Count && index >= 0)
		{
			//Destroys old Background
			Destroy(movedUI.GetChild(index));
			//adds newBackground 
			newBackground.parent = movedUI;
			//sets index
			newBackground.SetSiblingIndex(index);
			//corrects background list
			contentsBackground[index] = newBackground;
			//corrects text list
			FindText(index);
		}
	}

	public List<RectTransform> GetBackgrounds()
	{
		return contentsBackground;
	}

	public RectTransform GetCurrentBackground()
	{
		return contentsBackground[CurrentIndex];
	}

	//needed for other scripts
	void IndexClamp()
	{
		//makes sure it's calculating it correctly
		
		if (CurrentIndex > count - 2)
		{
			CurrentIndex = 1;
		}
		else if (CurrentIndex < 0)
		{
			CurrentIndex = count - 2;
		}
		
	}

	IEnumerator AutoSlideshow()
	{
		
		autoEnumRunning = true;
		yield return new WaitForSeconds(autoTime);
		autoEnumRunning = false;
		//Debug.Log("done");
		Increment();
	}

	IEnumerator MoveToPoint()
	{
		//checks if close
		
		while (Vector3.Distance(movedUI.localPosition, targetPos) > DIST_TO_POINT)
		{
			enumRunning = true;
			//gets direction
			Vector3 tempTargetPos = targetPos - movedUI.localPosition;
			Vector3 direction = tempTargetPos.normalized;

			//checks limits x
			if (movedUI.localPosition.x > maxLimits.x - 1 && direction.x > 0)
			{
				movedUI.localPosition = new Vector3(minLimits.x - moveDistance + startPos.x, movedUI.localPosition.y, movedUI.localPosition.z);
				targetPos.x = minLimits.x + startPos.x;
			}
			if (movedUI.localPosition.x < minLimits.x + 1 && direction.x < 0)
			{
				movedUI.localPosition = new Vector3(maxLimits.x + moveDistance + startPos.x, movedUI.localPosition.y, movedUI.localPosition.z);
				targetPos.x = maxLimits.x + startPos.x;
			}
			//checks limits y
			if (movedUI.localPosition.y > maxLimits.y - 1 && direction.y > 0)
			{
				movedUI.localPosition = new Vector3(movedUI.localPosition.x, minLimits.y - moveDistance + startPos.y, movedUI.localPosition.z);
				targetPos.y = minLimits.y + startPos.y;
			}
			if (movedUI.localPosition.y < minLimits.y + 1 && direction.y < 0)
			{
				movedUI.localPosition = new Vector3(movedUI.localPosition.x, maxLimits.y + moveDistance + startPos.y, movedUI.localPosition.z);
				targetPos.y = maxLimits.y + startPos.y;
			}
			//checks limits z
			if (movedUI.localPosition.z > maxLimits.z - 1 && direction.z > 0)
			{
				movedUI.localPosition = new Vector3(movedUI.localPosition.x, movedUI.localPosition.y, minLimits.z - moveDistance + startPos.z);
				targetPos.z = minLimits.z + startPos.z;
			}
			if (movedUI.localPosition.z < minLimits.z + 1 && direction.z < 0)
			{
				movedUI.localPosition = new Vector3(movedUI.localPosition.x, movedUI.localPosition.y, maxLimits.z + moveDistance + startPos.z);
				targetPos.z = maxLimits.z + startPos.z;
			}

			//moves to point if not in range
			if (Vector3.Distance(movedUI.localPosition + direction * speed * Time.unscaledDeltaTime, targetPos) > DIST_TO_POINT * speed / 4)
				movedUI.localPosition += direction * speed * Time.unscaledDeltaTime;
			else
				movedUI.localPosition = targetPos;
			yield return null;
		}

		//ends

		enumRunning = false;
		CurrentIndex++;
		IndexClamp();
		OnStop.Invoke();
	}

}
