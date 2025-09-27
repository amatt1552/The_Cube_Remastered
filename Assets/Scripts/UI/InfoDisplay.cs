using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class InfoDisplay : MonoBehaviour
{
	public CanvasGroup canvasGroup;
	public TMP_Text fps;
	public int fpsValue { get; private set; }
	public TMP_Text time;
	public TMP_Text deaths;
	GameSettings gameSettings;
	int deathsValue;
	float timeValue;

	private void OnEnable()
	{
		SaveGameManager.SaveComplete += UpdateSettings;
		CubeController.YouDied += UpdateValues;
		SceneManager.sceneLoaded += OnSceneLoaded;
		SceneManager.sceneUnloaded += OnSceneUnloaded;

	}
	private void OnDisable()
	{
		SaveGameManager.SaveComplete -= UpdateSettings;
		CubeController.YouDied -= UpdateValues;
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SceneManager.sceneUnloaded -= OnSceneUnloaded;
	}

	void UpdateSettings()
	{
		gameSettings = SaveGameManager.GetSave().gameSettings;
		fps.gameObject.SetActive(gameSettings.displayfps);
		time.gameObject.SetActive(gameSettings.displayTime);
		deaths.gameObject.SetActive(gameSettings.displayDeaths);
		
	}
	void UpdateValues()
	{
		deathsValue++;
	}
	void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		//timeValue = 0;
		//deathsValue = 0;
		LevelEnder.levelComplete = false;
		if (scene.buildIndex == 0)
		{
			HideCanvas();
		}
		else
		{
			ShowCanvas();
		}
	}
	void OnSceneUnloaded(Scene scene)
	{
		if (scene.buildIndex != 12)
		{
			if ((LevelEnder.levelComplete || SaveGameManager.GetSave().levelInfos[scene.buildIndex].levelComplete))
			{
				SaveGameManager.LevelComplete(scene.buildIndex, true, deathsValue, timeValue);
			}
			else
			{
				SaveGameManager.LevelComplete(scene.buildIndex, false, deathsValue, timeValue);
			}
		}
		timeValue = 0;
		deathsValue = 0;
		
	}

	private void Start()
	{
		InvokeRepeating("UpdateFPS",0,1);
	}
	void UpdateFPS()
	{
		fps.text = "FPS: " + fpsValue;
	}

	void HideCanvas()
	{
		if (canvasGroup != null)
		{
			canvasGroup.alpha = 0;
		}
	}
	void ShowCanvas()
	{
		if (canvasGroup != null)
		{
			canvasGroup.alpha = 1;
		}
	}

	void Update()
    {
		if (!LevelEnder.levelComplete)
		{
			timeValue += Time.deltaTime;
		}
		float hours, minutes, seconds;
		string displayedTime;
		seconds = timeValue % 60;
		minutes = (int)timeValue / 60;
		hours = (int)timeValue / 360;
		displayedTime = string.Format("{0:0}:{1:00}:{2:00}", hours, minutes, seconds);

		fpsValue = (int)(1f / Time.unscaledDeltaTime);
		time.text = "" + displayedTime;
		deaths.text = "Deaths: " + deathsValue;

	}

	
}
