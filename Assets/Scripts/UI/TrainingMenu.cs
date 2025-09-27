using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Fader))]

public class TrainingMenu : MonoBehaviour
{
	
	bool getHelpReset;
	public Slideshow slideshow;
	public CanvasGroup canvasGroup;
	[Header("Fill with {0} to replace the first key values and {1} to replace the second ones")]
	bool inited;
	Fader fader;
	bool helpEnabled;
	public float fadeTime;
	public GameObject[] unlockedHelp;
	List<string> oldText = new List<string>();
	GameSettings _gameSettings;


    void Awake()
    {
        SaveGameManager.Load();
        _gameSettings = SaveGameManager.GetSave().gameSettings;
        //training
        fader = GetComponent<Fader>();
		if(slideshow == null)
			slideshow = TheCubeGameManager.GetSlideshow();
		Init();
	}
	
	//sets up on sceneLoaded
	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
		SaveGameManager.SaveComplete += SaveComplete;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
		SaveGameManager.SaveComplete -= SaveComplete;
	}
	
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
        Init(); 
		if (TheCubeGameManager.CurrentLevel == 0 || !_gameSettings.firstStart)
        {

            slideshow.Enabled(false);
            helpEnabled = false;
        }
        else if (_gameSettings.firstStart)
        {
            Debug.Log($"Help menu loaded. Current level is: {TheCubeGameManager.CurrentLevel}");
            helpEnabled = true;
            Enable();
            //Invoke("Disable", 20);
        }
    }
	private void SaveComplete()
    {
        SaveGameManager.Load();
        _gameSettings = SaveGameManager.GetSave().gameSettings;
	}

	private void Start()
	{
		inited = false;
	}
	//where my awake variables go
	private void Init()
	{
		
		//keeps it from initializing multiple times.
		if (!inited)
		{
			
			//setting up initial values so I can change more than once.
			List<TMP_Text> text = slideshow.GetText();

			for (int i = 0; i < text.Count; i++)
			{
				oldText.Add(text[i].text);
			}
			ResetText();
			
			inited = true;
		}

	}

	// Update is called once per frame
	void Update()
    {
		if (TheCubeGameManager.CurrentLevel > 0 && TheCubeGameManager.CurrentLevel < TheCubeGameManager.FinalLevel)
		{
			HelpPressed();
			
		}
    }
	void HelpPressed()
	{
		if (hInput.GetAxis("Help") > 0)
		{
			if (!getHelpReset)
			{
				getHelpReset = true;
				GetHelp();
			}
		}
		else
		{
			getHelpReset = false;
		}
	}

	public void GetHelp()
	{
		helpEnabled = !helpEnabled;
		if (helpEnabled)
		{
			ResetText();
			Enable();
		}
		else
		{
			Disable();
		}
	}

	public void Enable()
	{
		StopCoroutine("Disable");
		slideshow.Enabled(true);
		if (canvasGroup != null)
		{
			fader.FadeTo(canvasGroup, 1, fadeTime, true);
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}
		
	}
	public void Disable()
	{
		StartCoroutine("EnumDisable");
	}
	IEnumerator EnumDisable()
	{
		if (canvasGroup != null)
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
			fader.FadeTo(canvasGroup, 0, fadeTime, true);
		}
		while (fader.fading)
		{
			yield return null;
		}
		slideshow.Enabled(false);
	}
	
	//setting up the text to match controls
	public void ResetText()
	{
		List<RectTransform> temp = slideshow.GetBackgrounds();
		List<TMP_Text> text = slideshow.GetText();
		

		for (int i = 0; i< unlockedHelp.Length; i++)
		{
			if (SaveGameManager.GetSave().newItems[i].found)
			{
				unlockedHelp[i].SetActive(true);
			}
		}

		for (int i = 0; i < temp.Count; i++)
		{
			switch (temp[i].name)
			{
				case "Right":
					slideshow.SetText(i, oldText[i]);
					if (_gameSettings.right1 != KeyCode.None && _gameSettings.right2 != KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.right1, " or " + _gameSettings.right2));
					}
					else if (_gameSettings.right1 == KeyCode.None && _gameSettings.right2 == KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, "nothing since its not set", ""));
					}
					else
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.right1 == KeyCode.None ? "" : _gameSettings.right1.ToString(), _gameSettings.right2 == KeyCode.None ? "" : _gameSettings.right2.ToString()));
					}
					
					break;
				case "Left":
					slideshow.SetText(i, oldText[i]);
					if (_gameSettings.left1 != KeyCode.None && _gameSettings.left2 != KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.left1, " or " + _gameSettings.left2));
					}
					else if (_gameSettings.left1 == KeyCode.None && _gameSettings.left2 == KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, "nothing since its not set", ""));
					}
					else
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.left1 == KeyCode.None ? "" : _gameSettings.left1.ToString(), _gameSettings.left2 == KeyCode.None ? "" : _gameSettings.left2.ToString()));
					}

					break;
				case "Jump":
					slideshow.SetText(i, oldText[i]);
					if (_gameSettings.jump1 != KeyCode.None && _gameSettings.jump2 != KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.jump1, " or " + _gameSettings.jump2));
					}
					else if (_gameSettings.jump1 == KeyCode.None && _gameSettings.jump2 == KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, "nothing since its not set", ""));
					}
					else
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.jump1 == KeyCode.None ? "" : _gameSettings.jump1.ToString(), _gameSettings.jump2 == KeyCode.None ? "" : _gameSettings.jump2.ToString()));
					}

					break;
				case "Self Destruct":
					slideshow.SetText(i, oldText[i]);
					if (_gameSettings.selfDestruct1 != KeyCode.None && _gameSettings.selfDestruct2 != KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.selfDestruct1, " or " + _gameSettings.selfDestruct2));
					}
					else if (_gameSettings.selfDestruct1 == KeyCode.None && _gameSettings.selfDestruct2 == KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, "nothing since its not set", ""));
					}
					else
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.selfDestruct1 == KeyCode.None ? "" : _gameSettings.selfDestruct1.ToString(), _gameSettings.selfDestruct2 == KeyCode.None ? "" : _gameSettings.selfDestruct2.ToString()));
					}

					break;
				case "Zoom":
					slideshow.SetText(i, oldText[i]);
					if (_gameSettings.usingScroll)
					{
						string zoom1;
						string zoom2;
						if (!_gameSettings.inverted)
						{
							zoom1 = "scroll up";
							zoom2 = "scroll down";
						}
						else
						{
							zoom1 = "scroll down";
							zoom2 = "scroll up";
						}
						
						slideshow.SetText(i, string.Format(text[i].text, zoom1, " to zoom in and " + zoom2));
						
					}
					else
					{
						if (_gameSettings.zoom1 != KeyCode.None && _gameSettings.zoom2 != KeyCode.None)
						{
							slideshow.SetText(i, string.Format(text[i].text, _gameSettings.zoom1, " to zoom in and " + _gameSettings.zoom2));
						}
						else if (_gameSettings.zoom1 == KeyCode.None && _gameSettings.zoom2 == KeyCode.None)
						{
							slideshow.SetText(i, string.Format(text[i].text, "nothing since its not set", "nothing since its not set"));
						}
						else
						{
							slideshow.SetText(i, _gameSettings.zoom1 == KeyCode.None ? "You only have one Zoom set. It's " + _gameSettings.zoom2 + ". I would suggest changing that setting." :
								"You only have one Zoom set. It's " + _gameSettings.zoom1 + ". I would suggest changing that setting.");
						}
					}
					break;
				case "Help":
					slideshow.SetText(i, oldText[i]);
					if (_gameSettings.help1 != KeyCode.None && _gameSettings.help2 != KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.help1, " or " + _gameSettings.help2));
					}
					else if (_gameSettings.help1 == KeyCode.None && _gameSettings.help2 == KeyCode.None)
					{
						slideshow.SetText(i, string.Format(text[i].text, "nothing since its not set", ""));
					}
					else
					{
						slideshow.SetText(i, string.Format(text[i].text, _gameSettings.help1 == KeyCode.None ? "" : _gameSettings.help1.ToString(), _gameSettings.help2 == KeyCode.None ? "" : _gameSettings.help2.ToString()));
					}

					break;

			}
		}
	}

}
