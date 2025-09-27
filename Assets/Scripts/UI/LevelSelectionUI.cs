using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectionUI : MonoBehaviour
{
	public Button continueButton;
	public Button[] levelButtons;
	public TMP_Text levelName;
	public Image levelImage;
	public TMP_Text mode;
	public Slideshow slideshow;

	TMP_Text kidDeaths;
	TMP_Text kidTime;
	TMP_Text kidTotalDeaths;
	//TMP_Text kidRating;
	TMP_Text hardDeaths;
	TMP_Text hardTime;
	TMP_Text hardTotalDeaths;	
	//TMP_Text hardRating;

	bool gotValues;

	private void Awake()
	{
		GetValues();
		SetlevelInfo(1);
	}

	void GetValues()
	{
		if (mode != null && slideshow != null && levelImage != null && levelName != null)
		{
			try
			{
				//KIDMODE!
				kidDeaths = slideshow.GetBackgrounds()[1].GetChild(0).GetComponent<TMP_Text>();
				kidTime = slideshow.GetBackgrounds()[1].GetChild(1).GetComponent<TMP_Text>();
				kidTotalDeaths = slideshow.GetBackgrounds()[1].GetChild(2).GetComponent<TMP_Text>();
				//kidRating = slideshow.GetBackgrounds()[1].GetChild(4).GetComponent<TMP_Text>();

				//hardmode
				hardDeaths = slideshow.GetBackgrounds()[2].GetChild(0).GetComponent<TMP_Text>();
				hardTime = slideshow.GetBackgrounds()[2].GetChild(1).GetComponent<TMP_Text>();
				hardTotalDeaths = slideshow.GetBackgrounds()[2].GetChild(2).GetComponent<TMP_Text>();
				//hardRating = slideshow.GetBackgrounds()[1].GetChild(4).GetComponent<TMP_Text>();
				gotValues = true;
			}
			catch
			{
				Debug.LogError("initialize level selection failed!");
			}
		}
	}

	void Start()
    {
		SaveFile saveFile = SaveGameManager.GetSave();
		if (continueButton != null)
		{
			ContinueCheck(saveFile);
		}
		//Debug.Log("aye");
		LevelSelection(saveFile);
    }

	void ContinueCheck(SaveFile saveFile)
	{
		
		if (saveFile.currentLevel > 1)
		{
			continueButton.interactable = true;
		}
		else
		{
			continueButton.interactable = false;
		}
	}

	void LevelSelection(SaveFile saveFile)
	{
		for (int i = 0; i < saveFile.levelInfos.Length - 2; i++)
		{
			if (saveFile.levelInfos[i].levelComplete)
			{
				levelButtons[i].interactable = true;
			}
			else
			{
				levelButtons[i].interactable = false;
			}
		}
	}

	public void SetText()
	{
		if (gotValues)
		{
			switch (slideshow.CurrentIndex)
			{
				case 1:
					mode.text = "KID MODE!";
					break;
				case 2:
					mode.text = "Hard Mode";
					break;
				default:
					break;
			}
		}
	}

	public void SetlevelInfo(int i)
	{
		LevelInfo levelInfo = SaveGameManager.GetSave().levelInfos[i];

		//formatting time
		float hours, minutes, seconds;
		string displayedTimeKid, dispayedTimeHard;
		//kid
		seconds = levelInfo.kidTime % 60;
		minutes = levelInfo.kidTime / 60;
		hours = levelInfo.kidTime / 360;
		displayedTimeKid = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);
		//hard
		seconds = levelInfo.hardTime % 60;
		minutes = levelInfo.hardTime / 60;
		hours = levelInfo.hardTime / 360;
		dispayedTimeHard = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);

		if (levelButtons[i - 1].interactable && gotValues)
		{
			levelName.text = "Level " + i;
			levelImage.sprite = levelButtons[i - 1].GetComponent<Image>().sprite;
			
			kidDeaths.text = levelInfo.kidDeaths != 100000? "Min Deaths: " + levelInfo.kidDeaths : "Min Deaths: " + 0;
			kidTime.text = "Time: " + displayedTimeKid;
			kidTotalDeaths.text = "Total Deaths: " + levelInfo.kidTotalDeaths;

			hardDeaths.text = levelInfo.hardDeaths != 100000 ? "Min Deaths: " + levelInfo.hardDeaths : "Min Deaths: " + 0;
			hardTime.text = "Time: " + dispayedTimeHard;
			hardTotalDeaths.text = "Total Deaths: " + levelInfo.hardTotalDeaths;
		}
	}
	
}
