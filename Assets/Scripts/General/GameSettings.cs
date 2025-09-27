using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings 
{
	//Video settings
	//public bool bloom;
	//public float bloomIntensity;
	public bool fullScreen;
	public bool hdr;
	public Vector2 resolution;
	public int resolutionVal;
	public int quality;
	public int fps;
	public bool defaultfps;
	public bool displayfps;
	public bool	displayTime;
	public bool	displayDeaths;

	//Sounds
	public float masterVol;
	public bool muteMaster;
	public float musicVol;
	public bool muteMusic;
	public float soundEffectsVol;
	public bool muteEffects;

	//Game Settings (the old control settings)
	public int difficulty;
	public KeyCode left1, left2;
	public KeyCode right1, right2;
	public KeyCode jump1, jump2;
	public bool usingScroll; //used to check if using a mouse or key value
	public bool inverted;
	public KeyCode zoom1, zoom2;
	public HardShellStudios.CompleteControl.MouseAxis mouseZoom;
	public KeyCode selfDestruct1, selfDestruct2;
	public KeyCode help1, help2;
	//Camera Settings
	public bool activeCamera;
	public float sensitivity;
	public bool rotateOnMove;
	public float rotateValue;

	//PlayerSettings
	public Color normalPlayerColor;
	public int currentSkin;

	//Training
	public bool firstStart;

	//Default values
	public GameSettings()
	{
		//Video Settings
		fullScreen = false;
		hdr = true;
		resolution = new Vector2(1280, 720);
		resolutionVal = 1;
		quality = 4;
		fps = 60;
		defaultfps = true;
		displayfps = false;
		displayTime = true;
		displayDeaths = true;

		//Audio Settings
		masterVol = 1;
		muteMaster = false;
		musicVol = 1;
		muteMusic = false;
		soundEffectsVol = 1;
		muteEffects = false;

		//Player Settings
		normalPlayerColor = new Color(0.5f, 0.7f, 0.9f);
		currentSkin = 0;

		//Game Settings (the old control settings)
		difficulty = 0;
		left1 = KeyCode.A;
		left2 = KeyCode.LeftArrow;
		right1 = KeyCode.D;
		right2 = KeyCode.RightArrow;
		jump1 = KeyCode.Space;
		jump2 = KeyCode.W;
		usingScroll = true;
		inverted = false;
		zoom1 = KeyCode.C;
		zoom2 = KeyCode.V;
		mouseZoom = HardShellStudios.CompleteControl.MouseAxis.ScrollWheel;
		selfDestruct1 = KeyCode.Z;
		selfDestruct2 = KeyCode.X;
		help1 = KeyCode.H;
		help2 = KeyCode.None;
		//Camera Settings
		activeCamera = true;
		sensitivity = 3;
		rotateOnMove = true;
		rotateValue = 3;

		//Training
		firstStart = true;
	}
	public override string ToString()
	{
		return "GameSettings!";
	}
}
