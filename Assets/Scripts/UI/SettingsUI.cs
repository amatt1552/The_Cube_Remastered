using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{

	public static SettingsUI GC;

	[Header("Player settings section")]
	public TMP_Text difficultyText;
	public TMP_Text hiddenText;
	bool forceRestart;
	public Button playerSettingsButton;
	public GameObject[] playerSettings;
	public Slider red, green, blue;
	public RectTransform sliderColorRed, sliderColorGreen, sliderColorBlue;
	public MeshRenderer playerUI;
	public TMP_Dropdown playerSkin;

	[Header("Audio settings section")]
	public Button audioButton;
	public GameObject[] audioSettings;
	public Slider master, music, soundEffects;
	public Toggle masterMute, musicMute, soundEffectsMute;
	const int MUTE_LEVEL = -50;

	[Header("Video settings section")]
	public Button videoButton;
	public GameObject[] videoSettings;
	public Dropdown screenResolution;
	Vector2 resolution;
	public Slider videoQuality;
	public TMP_InputField videoQualityInput;
	//public Toggle bloom;
	//public Slider bloomIntensity;
	public Toggle fullScreen;
	public Toggle hdr;
	public Slider fps;
	public Toggle defaultfps;
	public TMP_InputField fpsInput;
	public Toggle displayfps;
	public Toggle displayTime;
	public Toggle displayDeaths;

	[Header("Game Settings section")]
	public Slider difficulty;
	//controls
	public Button controlsButton;
	public GameObject[] controls;
	public GameObject left1, left2;
	public GameObject right1, right2;
	public GameObject jump1, jump2;
	bool usingScroll = true;
	public GameObject zoomIn1, zoomIn2;
	public GameObject zoomOut1, zoomOut2;
	public GameObject selfDestruct1, selfDestruct2;
	public GameObject help1, help2;
	//Camera Settings
	public Toggle activeCamera;
	public TMP_Text hiddenTextActiveCamera;
	public Slider sensitivity;
	public TMP_InputField sensitivityInput;
	public Toggle rotateOnMove;
	public Slider rotateValue;
	public TMP_InputField rotateInput;

	//setting KeyBindings
	public GameObject currentKey;
	Dictionary<string, KeyCode> keyValues = new Dictionary<string, KeyCode>();
	Dictionary<string, HardShellStudios.CompleteControl.MouseAxis> mouseValues = new Dictionary<string, HardShellStudios.CompleteControl.MouseAxis>();

	//Other
	GameObject chosenObject;
	ColorBlock playerBlock, audioBlock, videoBlock, controlsBlock;
	Color temp;
	SaveFile saveFile;
	GameSettings gameSettings;
	bool updating;
	public bool menuUI; 

	private void Awake()
	{
		if (TheCubeGameManager.CurrentLevel == 0 && !menuUI)
		{
			gameObject.SetActive(false);
			GC = null;
		}
		else
		{
			gameObject.SetActive(true);
			if (GC == null)
				GC = this;
			else
				Destroy(this);
		}
		//initialize keybindings
		keyValues.Add(left1.name, KeyCode.A);
		keyValues.Add(left2.name, KeyCode.LeftArrow);
		keyValues.Add(right1.name, KeyCode.D);
		keyValues.Add(right2.name, KeyCode.RightArrow);
		keyValues.Add(jump1.name, KeyCode.Space);
		keyValues.Add(jump2.name, KeyCode.W);
		keyValues.Add(zoomIn2.name, KeyCode.C);
		keyValues.Add(zoomOut2.name, KeyCode.V);
		mouseValues.Add(zoomIn1.name, HardShellStudios.CompleteControl.MouseAxis.ScrollWheel);
		keyValues.Add(selfDestruct1.name, KeyCode.Z);
		keyValues.Add(selfDestruct2.name, KeyCode.X);
		keyValues.Add(help1.name, KeyCode.H);
		keyValues.Add(help2.name, KeyCode.None);

		//initialize button colors
		temp = playerSettingsButton.GetComponent<Button>().colors.normalColor;
		playerBlock = playerSettingsButton.colors;
		playerBlock.normalColor = playerSettingsButton.GetComponent<Button>().colors.highlightedColor;
		audioBlock = audioButton.colors;
		videoBlock = videoButton.colors;
		controlsBlock = controlsButton.colors;

        //initialize music

        SaveGameManager.Load();
        saveFile = SaveGameManager.GetSave();
        gameSettings = saveFile.gameSettings;

        Revert();
		Apply();

	}


    //updating parts of the UI in "realtime"
    public void UpdateUI()
	{
		playerSettingsButton.colors = playerBlock;
		audioButton.colors = audioBlock;
		videoButton.colors = videoBlock;
		controlsButton.colors = controlsBlock;
		if (!updating)
		{
			updating = true;
			
			//Dropdown for skin setting
			List<TMP_Dropdown.OptionData> dropdownList = new List<TMP_Dropdown.OptionData>();
			int currentSkin = playerSkin.value;
			for (int i = 0; i < TheCubeGameManager.cubeInfoSO.skins.Length; i++)
			{
				TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
				if (TheCubeGameManager.cubeInfoSO.skins[i] != null)
				{
					option.text = TheCubeGameManager.cubeInfoSO.skins[i].name;
				}
				else
				{

					option.text = "None";
				}

				dropdownList.Add(option);

				//Debug.Log(dropdownList[i].text);
			}
			playerSkin.options = dropdownList;
			playerSkin.value = currentSkin;

			//slider color change
			Image sliderColorRed;
			Image sliderColorGreen;
			Image sliderColorBlue;

			sliderColorRed = this.sliderColorRed.GetComponent<Image>();
			sliderColorGreen = this.sliderColorGreen.GetComponent<Image>();
			sliderColorBlue = this.sliderColorBlue.GetComponent<Image>();

			sliderColorRed.color = new Color(red.value, 0, 0);
			sliderColorBlue.color = new Color(0, 0, blue.value);
			sliderColorGreen.color = new Color(0, green.value, 0);

			//set player colors and textures
			TheCubeGameManager.cubeInfoSO.currentSkin = playerSkin.value;
			TheCubeGameManager.UpdatePlayerTexture();
			TheCubeGameManager.UpdatePlayerColor(GetPlayerColor());
			if (playerUI != null)
			{
				playerUI.material.mainTexture = TheCubeGameManager.cubeInfoSO.skins[TheCubeGameManager.cubeInfoSO.currentSkin];
				playerUI.material.color = GetPlayerColor();
				
				//sets the slide effect material
				Material mat = Resources.Load("Materials/playerMaterialNoTex") as Material;
				mat.color = GetPlayerColor();
			}

			//making sure difficulty value is correct
			if (difficulty.value != gameSettings.difficulty && !menuUI)
			{
				hiddenText.enabled = true;
			}
			else
			{
				hiddenText.enabled = false;
			}
			updating = false;
		}
	}

	#region switching tabs

	public void PlayerSettingsUI()
	{
		UpdateUI();
		playerBlock.normalColor = playerBlock.highlightedColor;
		audioBlock.normalColor = temp;
		videoBlock.normalColor = temp;
		controlsBlock.normalColor = temp;
		for (int i = 0; i < playerSettings.Length;)
		{
			On(playerSettings[i], playerBlock);
			Off(audioSettings[i], audioBlock);
			Off(videoSettings[i], videoBlock);
			Off(controls[i], controlsBlock);
			i++;
		}
	}

	public void AudioSettings()
	{
		UpdateUI();
		playerBlock.normalColor = temp;
		audioBlock.normalColor = playerBlock.highlightedColor;
		videoBlock.normalColor = temp;
		controlsBlock.normalColor = temp;
		for (int i = 0; i < playerSettings.Length;)
		{

			Off(playerSettings[i], playerBlock);
			On(audioSettings[i], audioBlock);
			Off(videoSettings[i], videoBlock);
			Off(controls[i], controlsBlock);
			i++;
		}
	}

	public void VideoSettings()
	{
		UpdateUI();
		playerBlock.normalColor = temp;
		audioBlock.normalColor = temp;
		videoBlock.normalColor = playerBlock.highlightedColor;
		controlsBlock.normalColor = temp;
		for (int i = 0; i < playerSettings.Length;)
		{

			Off(playerSettings[i], playerBlock);
			Off(audioSettings[i], audioBlock);
			On(videoSettings[i], videoBlock);
			Off(controls[i], controlsBlock);
			i++;
		}
	}

	public void Controls()
	{
		UpdateUI();
		playerBlock.normalColor = temp;
		audioBlock.normalColor = temp;
		videoBlock.normalColor = temp;
		controlsBlock.normalColor = playerBlock.highlightedColor;
		for (int i = 0; i < playerSettings.Length;)
		{
			Off(playerSettings[i], playerBlock);
			Off(audioSettings[i], audioBlock);
			Off(videoSettings[i], videoBlock);
			On(controls[i], controlsBlock);
			i++;
		}
	}
	#endregion

	#region saving reverting etc

	public void Default()
	{
		//music
		master.value = 1;
		music.value = 1;
		soundEffects.value = 1;
		masterMute.isOn = false;
		musicMute.isOn = false;
		soundEffectsMute.isOn = false;

		//player settings
		red.value = 0.5f;
		green.value = 0.7f;
		blue.value = 0.9f;
		playerSkin.value = 0;

		//video settings
		hdr.isOn = true;
		//bloom.isOn = true;
		//bloomIntensity.value = 0.9f;
		videoQuality.value = 4;
		fullScreen.isOn = false;
		hdr.isOn = true;
		screenResolution.value = 1;
		fps.value = 60;
		defaultfps.isOn = true;
		displayfps.isOn = false;
		displayTime.isOn = true;
		displayDeaths.isOn = true;

		//Game Settings
		difficulty.value = 0;

		keyValues[left1.name] = KeyCode.A;//sets in dictionary
		SetControlsText(left1, KeyCode.A);//sets in UI

		keyValues[left2.name] = KeyCode.LeftArrow;
		SetControlsText(left2, KeyCode.LeftArrow);

		keyValues[right1.name] = KeyCode.D;
		SetControlsText(right1, KeyCode.D);

		keyValues[right2.name] = KeyCode.RightArrow;
		SetControlsText(right2, KeyCode.RightArrow);

		keyValues[jump1.name] = KeyCode.Space;
		SetControlsText(jump1, KeyCode.Space);

		keyValues[jump2.name] = KeyCode.W;
		SetControlsText(jump2, KeyCode.W);

		usingScroll = true;
		gameSettings.inverted = false;

		keyValues[zoomIn2.name] = KeyCode.C;
		SetControlsText(zoomIn2, KeyCode.C);
		keyValues[zoomOut2.name] = KeyCode.V;
		SetControlsText(zoomOut2, KeyCode.V);
		mouseValues[zoomIn1.name] = HardShellStudios.CompleteControl.MouseAxis.ScrollWheel;
		SetControlsTextScroll(zoomIn1);

		keyValues[selfDestruct1.name] = KeyCode.Z;
		SetControlsText(selfDestruct1, KeyCode.Z);

		keyValues[selfDestruct2.name] = KeyCode.X;
		SetControlsText(selfDestruct2, KeyCode.X);

		keyValues[help1.name] = KeyCode.H;
		SetControlsText(help1, KeyCode.H);

		keyValues[help2.name] = KeyCode.None;
		SetControlsText(help2, KeyCode.None);
		
		//camera Settings
		activeCamera.isOn = true;
		sensitivity.value = 3;
		rotateOnMove.isOn = true;
		rotateValue.value = 3;
		UpdateInput();
	}

	public void Apply()
	{
        //load old data
        SaveGameManager.Load();
        saveFile = SaveGameManager.GetSave();
        gameSettings = saveFile.gameSettings;

        //music settings
        gameSettings.masterVol = master.value;
		gameSettings.musicVol = music.value;
		gameSettings.soundEffectsVol = soundEffects.value;
		gameSettings.muteMaster = masterMute.isOn;
		gameSettings.muteMusic = musicMute.isOn;
		gameSettings.muteEffects = soundEffectsMute.isOn;
		SetVolume();

		//player settings
		gameSettings.normalPlayerColor = GetPlayerColor();
		gameSettings.currentSkin = playerSkin.value;

		//video settings
		gameSettings.quality = (int)videoQuality.value;
		gameSettings.fullScreen = fullScreen.isOn;
		gameSettings.hdr = hdr.isOn;
		//Settings.bloom = bloom.isOn;
		//Settings.bloomIntensity = bloomIntensity.value;
		gameSettings.fps = (int)fps.value;
		gameSettings.defaultfps = defaultfps.isOn;
		gameSettings.displayfps = displayfps.isOn;
		gameSettings.displayTime = displayTime.isOn;
		gameSettings.displayDeaths = displayDeaths.isOn;

		gameSettings.resolution = resolution;
		gameSettings.resolutionVal = screenResolution.value;
		switch (screenResolution.value)
		{
			case 0:
				resolution = new Vector2(800, 600);
				break;

			case 1:
				resolution = new Vector2(1280, 720);
				break;

			case 2:
				resolution = new Vector2(1920, 1080);
				break;

			default:
				break;
		}
#if UNITY_WEBGL
		QualitySettings.SetQualityLevel(gameSettings.quality);
		Camera.main.allowHDR = gameSettings.hdr;
		Application.targetFrameRate = defaultfps.isOn ? -1 : (int)fps.value;
		//unlimitedfps ? 0 : (int)fps.value;
		Screen.SetResolution(Display.main.systemWidth, Display.main.systemHeight, Screen.fullScreen);
#else
		QualitySettings.SetQualityLevel(gameSettings.quality);
		Camera.main.allowHDR = gameSettings.hdr;
		Application.targetFrameRate = defaultfps.isOn ? -1 : (int)fps.value;
		Screen.SetResolution((int)resolution.x, (int)resolution.y, Settings.fullScreen, defaultfps.isOn ? 0:(int)fps.value);
#endif

		//Game Settings
		if (gameSettings.difficulty != difficulty.value && !menuUI)
		{
			forceRestart = true;
		}
		gameSettings.difficulty = (int)difficulty.value;

		gameSettings.left1 = keyValues[left1.name];
		gameSettings.left2 = keyValues[left2.name];

		gameSettings.right1 = keyValues[right1.name];
		gameSettings.right2 = keyValues[right2.name];

		gameSettings.jump1 = keyValues[jump1.name];
		gameSettings.jump2 = keyValues[jump2.name];

		gameSettings.usingScroll = usingScroll;
		gameSettings.zoom1 = keyValues[zoomIn2.name];
		gameSettings.zoom2 = keyValues[zoomOut2.name];
		gameSettings.mouseZoom = mouseValues[zoomIn1.name];

		gameSettings.selfDestruct1 = keyValues[selfDestruct1.name];
		gameSettings.selfDestruct2 = keyValues[selfDestruct2.name];
		gameSettings.help1 = keyValues[help1.name];
		gameSettings.help2 = keyValues[help2.name];
		//camera Settings
		gameSettings.activeCamera = activeCamera.isOn;
		gameSettings.sensitivity = sensitivity.value;
		//hInput.SetKeySensitivity("move_scroll", sensitivity.value);
		gameSettings.rotateOnMove = rotateOnMove.isOn;
		gameSettings.rotateValue = rotateValue.value;

		//saving
		saveFile.gameSettings = gameSettings;
		SaveGameManager.SetSaveFile(saveFile);
		SaveGameManager.Save();
		SetControls();
		UpdateUI();
		if (forceRestart)
		{

			forceRestart = false;
			TheCubeGameManager.LoadLevelStatic(TheCubeGameManager.CurrentLevel);
		}

	}

	public void Revert()
	{
		//Debug.Log("Reverting Settings");
        //load old data
        SaveGameManager.Load();
        saveFile = SaveGameManager.GetSave();
        gameSettings = saveFile.gameSettings;

        //Game Settings
        difficulty.value = gameSettings.difficulty;

		keyValues[left1.name] = gameSettings.left1;
		SetControlsText(left1, gameSettings.left1);
		keyValues[left2.name] = gameSettings.left2;
		SetControlsText(left2, gameSettings.left2);

		keyValues[right1.name] = gameSettings.right1;
		SetControlsText(right1, gameSettings.right1);
		keyValues[right2.name] = gameSettings.right2;
		SetControlsText(right2, gameSettings.right2);

		keyValues[jump1.name] = gameSettings.jump1;
		SetControlsText(jump1, gameSettings.jump1);
		keyValues[jump2.name] = gameSettings.jump2;
		SetControlsText(jump2, gameSettings.jump2);

		usingScroll = gameSettings.usingScroll;
		
		//scroll here
		keyValues[zoomIn2.name] = gameSettings.zoom1;
		SetControlsText(zoomIn2, gameSettings.zoom1);
		keyValues[zoomOut2.name] = gameSettings.zoom2;
		SetControlsText(zoomOut2, gameSettings.zoom2);
		//gameSettings.inverted = false;
		SetControlsTextScroll(zoomIn1);

		keyValues[selfDestruct1.name] = gameSettings.selfDestruct1;
		SetControlsText(selfDestruct1, gameSettings.selfDestruct1);
		keyValues[selfDestruct2.name] = gameSettings.selfDestruct2;
		SetControlsText(selfDestruct2, gameSettings.selfDestruct2);

		keyValues[help1.name] = gameSettings.help1;
		SetControlsText(help1, gameSettings.help1);
		keyValues[help2.name] = gameSettings.help2;
		SetControlsText(help2, gameSettings.help2);
		
		//Camera Settings
		activeCamera.isOn = gameSettings.activeCamera;
		sensitivity.value = gameSettings.sensitivity;
		rotateOnMove.isOn = gameSettings.rotateOnMove;
		rotateValue.value = gameSettings.rotateValue;

		//video settings
		//bloom.isOn = bloom;
		//bloomIntensity.value = GameSettings.bloomIntensity;
		fullScreen.isOn = gameSettings.fullScreen;
		hdr.isOn = gameSettings.hdr;
		screenResolution.value = gameSettings.resolutionVal;
		resolution = gameSettings.resolution;
		videoQuality.value = gameSettings.quality;
		fps.value = gameSettings.fps;
		defaultfps.isOn = gameSettings.defaultfps;
		displayfps.isOn = gameSettings.displayfps;
		displayTime.isOn = gameSettings.displayTime;
		displayDeaths.isOn = gameSettings.displayDeaths;

		//sound
		master.value = gameSettings.masterVol;
		music.value = gameSettings.musicVol;
		soundEffects.value = gameSettings.soundEffectsVol;
		masterMute.isOn = gameSettings.muteMaster;
		musicMute.isOn = gameSettings.muteMusic;
		soundEffectsMute.isOn = gameSettings.muteEffects;

		//player settings
		red.value = gameSettings.normalPlayerColor.r;
		green.value = gameSettings.normalPlayerColor.g;
		blue.value = gameSettings.normalPlayerColor.b;
		playerSkin.value = gameSettings.currentSkin;
		//Debug.Log(gameSettings.currentSkin);
		UpdateUI();

	}
	#endregion

	//updating on slider change for player color
	public void RedChange()
	{
		//gameSettings.normalPlayerColor.r = red.value;
		UpdateUI();
	}
	public void GreenChange()
	{
		//gameSettings.normalPlayerColor.g = green.value;
		UpdateUI();
	}
	public void BlueChange()
	{
		//gameSettings.normalPlayerColor.b = blue.value;
		UpdateUI();
	}

	//turns off and on GameObjects
	void Off(GameObject chosenObject, ColorBlock block)
	{
		chosenObject.SetActive(false);
	}
	void On(GameObject chosenObject, ColorBlock block)
	{
		this.chosenObject = chosenObject;
		UpdateUI();
		//prevents the gameobject from not being able to load on pause. 
		if (!TheCubeGameManager.paused)
		{
			Invoke("Invoked", 0.1f);
		}
		else
		{
			Invoked();
		}
	}
	void Invoked()
	{
		chosenObject.SetActive(true);
	}

	//quicky get color value from UI
	public Color GetPlayerColor()
	{
		return new Color(red.value, green.value, blue.value);
	}

	//used to check for input for controls
	private void OnGUI()
	{
		
		if (currentKey != null)
		{
			Event e = Event.current;
			if (e.isKey && currentKey != zoomIn1 && currentKey != zoomOut1)
			{
				if (e.keyCode != KeyCode.Delete)
				{
					keyValues[currentKey.name] = e.keyCode;
					SetControlsText(currentKey, e.keyCode);
				}
				else
				{
					keyValues[currentKey.name] = KeyCode.None;
					SetControlsText(currentKey, KeyCode.None);
				}
				if (currentKey == zoomIn1 || currentKey == zoomIn2)
				{
					usingScroll = false;
				}
				currentKey = null;
			}

			if (currentKey == zoomIn1 || currentKey == zoomOut1)
			{
				if (Input.GetAxis("Mouse ScrollWheel") > 0)
				{
					usingScroll = true;
					gameSettings.inverted = true;
					SetControlsTextScroll(currentKey);
				}
				else if (Input.GetAxis("Mouse ScrollWheel") < 0)
				{
					usingScroll = true;
					gameSettings.inverted = false;
					SetControlsTextScroll(currentKey);
				}
			}
		}
	}

	//selects current button
	public void SetCurrentKey(GameObject keyGameObject)
	{
		currentKey = keyGameObject;
	}

	//sets text for selected button
	void SetControlsText(GameObject currentKey, KeyCode key)
	{
		if (currentKey != null)
		{
			currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = key.ToString();
			
			currentKey = null;
			//Debug.Log(currentKey);
		}
	}

	//sets text if using scroll wheel
	void SetControlsTextScroll(GameObject currentKey)
	{
		if (currentKey != null)
		{
			if (currentKey == zoomIn1)
			{
				if (!gameSettings.inverted)
				{
					currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Scroll Down";
					currentKey = zoomOut1;
					currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Scroll Up";
					currentKey = zoomIn1;
					//Debug.Log(currentKey);
				}
				else
				{
					currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Scroll Up";
					currentKey = zoomOut1;
					currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Scroll Down";
					currentKey = zoomIn1;
					//Debug.Log(currentKey);

				}
			}
			else if (currentKey == zoomOut1)
			{
				if (!gameSettings.inverted)
				{
					currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Scroll Down";
					currentKey = zoomIn1;
					currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Scroll Up";
					currentKey = zoomOut1;
					//Debug.Log(currentKey);
				}
				else
				{
					currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Scroll Up";
					currentKey = zoomIn1;
					currentKey.transform.GetChild(0).GetComponent<TMP_Text>().text = "Scroll Down";
					currentKey = zoomOut1;
					//Debug.Log(currentKey);

				}
			}
			currentKey = null;
		}
	}

	//applies new buttons to hInput
	void SetControls()
	{
		hInput.SetKey("move_horizontal", keyValues[right1.name], KeyTarget.PositivePrimary);
		hInput.SetKey("move_horizontal", keyValues[right2.name], KeyTarget.PositiveSecondary);

		hInput.SetKey("move_horizontal", keyValues[left1.name], KeyTarget.NegativePrimary);
		hInput.SetKey("move_horizontal", keyValues[left2.name], KeyTarget.NegativeSecondary);

		hInput.SetKey("move_jump", keyValues[jump1.name], KeyTarget.PositivePrimary);
		hInput.SetKey("move_jump", keyValues[jump2.name], KeyTarget.PositiveSecondary);

		hInput.SetKey("move_scrollKey", keyValues[zoomIn2.name], KeyTarget.PositivePrimary);
		hInput.SetKey("move_scrollKey", keyValues[zoomOut2.name], KeyTarget.NegativeSecondary);
		hInput.SetKey("move_scrollMouse", HardShellStudios.CompleteControl.MouseAxis.ScrollWheel);

		hInput.SetKey("move_self_destruct", keyValues[selfDestruct1.name], KeyTarget.PositivePrimary);
		hInput.SetKey("move_self_destruct", keyValues[selfDestruct2.name], KeyTarget.PositiveSecondary);
		hInput.SetKey("move_help", keyValues[help1.name], KeyTarget.PositivePrimary);
		hInput.SetKey("move_help", keyValues[help2.name], KeyTarget.PositiveSecondary);
	}

	//sets volume for mixers
	void SetVolume()
	{
		AudioMixer mixer = TheCubeGameManager.GetMixer();
		//set volume if not muted
		if(!masterMute.isOn)// && master.value > MUTE_LEVEL)
			mixer.SetFloat("MasterVolume", Mathf.Log10(master.value) * 20);
		else
			mixer.SetFloat("MasterVolume", -80f);

		if (!musicMute.isOn)// && master.value > MUTE_LEVEL)
			mixer.SetFloat("MusicVolume", Mathf.Log10(music.value) * 20);
		else
			mixer.SetFloat("MusicVolume", -80f);

		if (!soundEffectsMute.isOn)// && master.value > MUTE_LEVEL)
			mixer.SetFloat("SoundEffectsVolume", Mathf.Log10(soundEffects.value) * 20);
		else
			mixer.SetFloat("SoundEffectsVolume", -80f);
	}

	public void UpdateSliders()
	{
		//video quality
		//clamps values
		if (int.Parse(videoQualityInput.text) > 4)
		{
			videoQualityInput.text = "4";
		}
		else if (int.Parse(videoQualityInput.text) < 0)
		{
			videoQualityInput.text = "0";
		}
		//setsvalue
		videoQuality.value = int.Parse(videoQualityInput.text);

		//fps
		if (int.Parse(fpsInput.text) > 120)
		{
			fpsInput.text = "120";
		}
		else if (int.Parse(fpsInput.text) < 0)
		{
			fpsInput.text = "0";
		}
		fps.value = int.Parse(fpsInput.text);

		//sensitivity
		if (int.Parse(sensitivityInput.text) > 5)
		{
			sensitivityInput.text = "5";
		}
		else if (int.Parse(sensitivityInput.text) < 1)
		{
			sensitivityInput.text = "1";
		}
		sensitivity.value = int.Parse(sensitivityInput.text);

		//rotate Value
		if (int.Parse(rotateInput.text) > 25)
		{
			rotateInput.text = "25";
		}
		else if (int.Parse(rotateInput.text) < 0)
		{
			rotateInput.text = "0";
		}
		rotateValue.value = int.Parse(rotateInput.text);
	}
	public void UpdateInput()
	{
		videoQualityInput.text = "" + videoQuality.value;
		fpsInput.text = "" + fps.value;
		sensitivityInput.text = "" + sensitivity.value;
		rotateInput.text = "" + rotateValue.value;

		switch (difficulty.value)
		{
			case 0:
				difficultyText.text = "Kid Mode!";
				break;
			case 1:
				difficultyText.text = "Hard Mode";
				break;
			default:
				break;
		}
		if (difficulty.value != gameSettings.difficulty && !menuUI)
		{
			hiddenText.enabled = true;
		}
		else
		{
			hiddenText.enabled = false;
		}
		SetVolume();
	}

	public void HideObj(GameObject obj)
	{
		obj.SetActive(false);
	}

	public void ShowObj(GameObject obj)
	{
		obj.SetActive(true);
	}
}
