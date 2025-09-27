using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseMenu : MonoBehaviour
{

    public enum MenuState
	{
		Closed,
		PauseMenu,
		SettingsMenu,
		LevelSelectionMenu
	}
	MenuState menuState;
	public GameObject pauseMenu;
	public GameObject settingsMenu;
	public GameObject levelSelectionMenu;
	public Slideshow slideshow;
	CubeMotor cubeMotor;
	public UnityEvent OnMenuOpen;

    void Awake()
    {
		SetMenuState(0);
        SettingsOn();
    }

	private void Start()
	{
		cubeMotor = TheCubeGameManager.player.GetComponent<CubeMotor>();
	}

	void Update()
    {
		EscCheck();
		SettingsOn();
		ForceOff();
	}

	void EscCheck()
	{
		if (Input.GetKeyDown(KeyCode.Escape) && TheCubeGameManager.CurrentLevel != 0 && cubeMotor.movementEnabled)
		{
			switch (menuState)
			{
				case MenuState.Closed:
					menuState = MenuState.PauseMenu;
					pauseMenu.SetActive(true);
					TheCubeGameManager.SetPause(true);
                    OnMenuOpen?.Invoke();
                    break;
				case MenuState.PauseMenu:
					menuState = MenuState.Closed;
					pauseMenu.SetActive(false);
					TheCubeGameManager.SetPause(false);
					break;
				case MenuState.SettingsMenu:
					menuState = MenuState.PauseMenu;
					settingsMenu.SetActive(false);
					pauseMenu.SetActive(true);
					break;
				case MenuState.LevelSelectionMenu:
					menuState = MenuState.PauseMenu;
					levelSelectionMenu.SetActive(false);
					pauseMenu.SetActive(true);
					slideshow.Enabled(false);
					break;
				default:
					break;
			}
		}
	}

	public void SetMenuState(int newMenuState)
	{
		switch (newMenuState)
		{
			case 0://closed
				menuState = MenuState.Closed;
				pauseMenu.SetActive(false);
				settingsMenu.SetActive(false);
				levelSelectionMenu.SetActive(false);
				TheCubeGameManager.SetPause(false);
				break;
			case 1://paused
				menuState = MenuState.PauseMenu;
				pauseMenu.SetActive(true);
				settingsMenu.SetActive(false);
				levelSelectionMenu.SetActive(false);
				TheCubeGameManager.SetPause(true);
				break;
			case 2://settings
				menuState = MenuState.SettingsMenu;
				pauseMenu.SetActive(false);
				settingsMenu.SetActive(true);
				levelSelectionMenu.SetActive(false);
				TheCubeGameManager.SetPause(true);
				break;
			case 3://levelSelection
				menuState = MenuState.LevelSelectionMenu;
				pauseMenu.SetActive(false);
				settingsMenu.SetActive(false);
				levelSelectionMenu.SetActive(true);
				slideshow.Enabled(true);
				TheCubeGameManager.SetPause(true);
				break;
			default:
				Debug.Log("OI. 0-3 only for menuState.");
				break;
		}
		
	}

	void SettingsOn()
	{
		if (TheCubeGameManager.CurrentLevel == 0)
		{
			settingsMenu.GetComponent<SettingsUI>().enabled = false;
		}
		else
		{
			settingsMenu.GetComponent<SettingsUI>().enabled = true;
		}
	}
	void ForceOff()
	{
		if ((!TheCubeGameManager.LoadComplete() || TheCubeGameManager.Fading()))
		{
			SetMenuState(0);
		}
	}
}
