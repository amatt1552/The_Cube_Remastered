using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeEndText : MonoBehaviour
{
	public TMP_Text changedText;
	bool oneShot;

	private void OnEnable()
	{
		SaveGameManager.SaveComplete += RunAfterSave;
	}

	private void OnDisable()
	{
		SaveGameManager.SaveComplete -= RunAfterSave;
	}

	void RunAfterSave()
	{
		bool hardModeComplete = false;
		LevelInfo[] info = SaveGameManager.GetSave().levelInfos;
		for (int i = 0; i < info.Length - 1; i++)
		{
			if (info[i].hardTime < 1)
			{
				hardModeComplete = false;
				break;
			}
			hardModeComplete = true;
		}
		
		if (!oneShot && !hardModeComplete)
		{ 
			changedText.text = "Great work kid. Now go finish hard mode! It's in the Game Settings.";
			oneShot = true;
		}
	}

}
