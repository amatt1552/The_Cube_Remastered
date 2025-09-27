using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelInfo 
{
	public bool levelComplete;
	public int kidTotalDeaths;
	public int kidDeaths;
	public float kidTime;
	public int kidStars;
	public int hardTotalDeaths;
	public int hardDeaths;
	public float hardTime;
	public int hardStars;

	//default values
	public LevelInfo()
	{
		levelComplete = false;
		kidTotalDeaths = 0;
		kidDeaths = 100000;
		kidTime = 0;
		kidStars = 0;
		hardTotalDeaths = 0;
		hardDeaths = 100000;
		hardTime = 0;
		hardStars = 0;
	}

	public override string ToString()
	{
		return "LevelInfo\nLevel Complete: " + levelComplete + "   Deaths: " + kidDeaths + "   Time: " + kidTime + "   Starpower: " + kidStars;
	}
}
