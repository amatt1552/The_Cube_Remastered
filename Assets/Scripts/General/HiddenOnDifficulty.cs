using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenOnDifficulty : MonoBehaviour
{
	int difficulty;
	public GameObject[] hiddenHardModeObjects;
	public GameObject[] hiddenKidModeObjects;

	void Awake()
    {
		difficulty = SaveGameManager.difficulty;
		if (difficulty == 1)
		{
			Debug.Log("Hiding kidmode objects!");
			for (int i = 0; i < hiddenHardModeObjects.Length; i++)
			{
				
				hiddenHardModeObjects[i].SetActive(false);
			}
		}
		else
		{
			Debug.Log("Hiding hardmode objects!");
			for (int i = 0; i < hiddenKidModeObjects.Length; i++)
			{

				hiddenKidModeObjects[i].SetActive(false);
			}
		}
    }

	

    
}
