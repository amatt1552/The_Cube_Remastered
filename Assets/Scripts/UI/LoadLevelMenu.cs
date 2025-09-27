using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Fader))]

public class LoadLevelMenu : MonoBehaviour
{
	//private static TheCubeGameManager _S;
	public int currentLevel;
	public int finalLevel;
	public Button continueButton;
	AsyncOperation asyncManager;
	bool _startOverOneShot;

	//constants
	const int WAIT_TIME = 8;
	const float ON_SPEED = 0.5f;
	const float OFF_SPEED = 0.5f;

	//level change fader
	Fader fader;

	//timer
	static float startTime;
	public static float timePassed;

	public static bool paused;
	static bool loadLevelOneshot;

	private void Awake()
	{
		
		finalLevel = SceneManager.sceneCountInBuildSettings - 1;
		//print(finalLevel);
	}
	
	private void Start()
	{
		//sets start time
		//startTime = Time.time;
		fader = GameObject.Find("GameManager").GetComponent<Fader>();
		fader.FadeToEndActivate();
		//Debug.Log("SceneLoading");

		//checks if final level
		currentLevel = SceneManager.GetActiveScene().buildIndex;
		if (currentLevel == finalLevel)
		{
			if (!_startOverOneShot)
			{
				StartCoroutine("StartOver");
				_startOverOneShot = true;
			}
		}
		else
		{
			_startOverOneShot = false;
		}

		//disables music at menu
		if (currentLevel == 0)
		{
			fader.canvasGroup.alpha = 0;
		}
		

	}

	IEnumerator StartOver()
	{
		yield return new WaitForSeconds(5);
		LoadLevel(0);
	}

	public void LoadNextLevel()
	{
		if (!loadLevelOneshot)
		{
			fader.FadeToStart();
			StartCoroutine("WaitToLoad", currentLevel + 1);
			loadLevelOneshot = true;
		}
	}

	public void LoadLevel(int level)
	{
		if (!loadLevelOneshot)
		{
			fader.FadeToStart();
			StartCoroutine("WaitToLoad", level);
			loadLevelOneshot = true;
		}
	}

	IEnumerator WaitToLoad(int level)
	{
		while (fader.fading)
		{
			yield return null;
		}
		if (LoadComplete())
		{
			asyncManager = SceneManager.LoadSceneAsync(level);
			loadLevelOneshot = false;
			CheckpointManager.RemoveAllCheckpoints();
		}
	}

	public void NewGame()
	{
		SaveGameManager.NewGame();
		LoadNextLevel();
	}

	public void NewGameCheck(GameObject menu)
	{
		if (continueButton != null && menu != null)
		{
			if (continueButton.interactable)
			{
				menu.SetActive(true);
			}
			else
			{
				NewGame();
			}
		}
		else
		{
			NewGame();
		}
	}

	public void RestartLevel()
	{
		if (!loadLevelOneshot)
		{
			LoadLevel(currentLevel);
			loadLevelOneshot = true;
		}
	}

	public void Continue()
	{
        TheCubeGameManager.Continue();
    }

	public bool LoadComplete()
	{
		currentLevel = SceneManager.GetActiveScene().buildIndex;
		if (asyncManager != null)
			return asyncManager.isDone;
		return true;
	}
	
}
