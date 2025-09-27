//#define DEBUGGING

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.InteropServices;

static public class SaveGameManager
{
    static private SaveFile saveFile;
    static private string filePath;
	static public int difficulty;
	const int LEVEL_COUNT = 12;
	const int DIFF_CAP = 1;
	public delegate void Saved();
	public static event Saved SaveComplete;

	// For saving in WebGL
	[DllImport("__Internal")]
	private static extern void SyncFiles();

	[DllImport("__Internal")]
	private static extern void WindowAlert(string message);

    // Loading and saving limits
    static public bool LOCK
    {
        get;
        private set;
    }
    static bool alreadyLoaded;
	static float saveTime = -10f;
	const float COOLDOWN = 0.5f;

    static SaveGameManager()
    {
        LOCK = false;
        filePath = Application.persistentDataPath + "/Data.dat";

#if DEBUGGING
        Debug.Log("SaveGameManager:Awake() – Path: " + filePath);
#endif

        saveFile = new SaveFile();
    }


    static public void Save()
    {
        // prevents saving while loading
        if (LOCK) return;

		// adds cooldown to saving to prevent multiple saves
		if (Time.time < saveTime + COOLDOWN) return;
		//might add achievements later so kept these
		//saveFile.progress = Achievements.GetProgress();
		//saveFile.achievements = Achievements.GetAchievements();
		
		//initialize gameSettings
		if (saveFile.gameSettings == null)
		{
			saveFile.gameSettings = new GameSettings();
		}

		//makes sure difficulty isn't too high
		if (saveFile.gameSettings.difficulty > DIFF_CAP)
		{
			saveFile.gameSettings.difficulty = DIFF_CAP;
		}

		//initialize levelInfo
		if (saveFile.levelInfos == null)
		{
			saveFile.levelInfos = new LevelInfo[LEVEL_COUNT];
			LevelComplete(0, true, 0, 0);
		}
		else if(!saveFile.levelInfos[0].levelComplete)//double checks that the menu level is complete.
		{
			
			LevelComplete(0, true, 0, 0);
		}

		//initialize newItems
		if (saveFile.newItems == null)
		{
			saveFile.newItems = new NewItem[2];
			Debug.Log(saveFile.newItems.Length);
			saveFile.newItems[0] = new NewItem("Launcher");
			saveFile.newItems[1] = new NewItem("Bouncer");
		}
		string jsonSaveFile = JsonUtility.ToJson(saveFile, true);

        File.WriteAllText(filePath, jsonSaveFile);

		if (Application.platform == RuntimePlatform.WebGLPlayer)
		{
			SyncFiles();
		}

#if DEBUGGING
        Debug.Log("SaveGameManager:Save() – Path: " + filePath);
        Debug.Log("SaveGameManager:Save() – JSON: " + jsonSaveFile);

#endif
		SaveComplete?.Invoke();
		saveTime = Time.time;
		alreadyLoaded = false;
    }


    static public void Load()
    {
		// This prevents loading when data has not been modified yet.
		// Set to false on successful save (when it is modified).
		// Set to true on successful load.
		if(alreadyLoaded) return;

		if (File.Exists(filePath))
        {
			LOCK = true;
            string dataAsJson = File.ReadAllText(filePath);
#if DEBUGGING
            Debug.Log("SaveGameManager:Load() – File text is:\n" + dataAsJson);
#endif

            try
            {
                saveFile = JsonUtility.FromJson<SaveFile>(dataAsJson);
            }
            catch
            {
                Debug.LogWarning("SaveGameManager:Load() – SaveFile was malformed.\n" + dataAsJson);
                LOCK = false;
                Save();
                return;
            }

#if DEBUGGING
            Debug.Log("SaveGameManager:Load() – Successfully loaded save file.");
#endif
            
			//Load 
			//Achievements.LoadDataFromSaveFile(saveFile);
			difficulty = saveFile.gameSettings.difficulty;

            alreadyLoaded = true;
            LOCK = false;
        }
        else
        {

#if DEBUGGING
            Debug.LogWarning("SaveGameManager:Load() – Unable to find save file. "
                + "This is totally fine if you've never gotten a game over "
                + "or completed an Achievement, which is when the game is saved.");
#endif
			Save();
        }
    }


    static public void DeleteSave()
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            saveFile = new SaveFile();
            Debug.Log("SaveGameManager:DeleteSave() – Successfully deleted save file.");
        }
        else
        {
            Debug.LogWarning("SaveGameManager:DeleteSave() – Unable to find and delete save file!"
                + " This is fine if you've never saved or have just deleted the file.");
        }

        //Achievements.ClearAchievements();
    }
	/// <summary>
	/// this will also save.
	/// </summary>
	/// <param name="score"></param>
	/// <returns></returns>
	/* static internal bool CheckHighScore(int score)
	 {
		 if (score > saveFile.highScore)
		 {
			 saveFile.highScore = score;

			 Save();
			 return true;
		 }
		 return false;
	 }
	 */

	static private int CurrentLevelCheck()
	{
        //saveFile.gameSettings.firstStart = true;
        for (int i = 0; i < saveFile.levelInfos.Length; i++)
		{
			if (!saveFile.levelInfos[i].levelComplete)
			{
				saveFile.currentLevel = i;
				//checking if you beat level 1
				if (saveFile.gameSettings.firstStart && TheCubeGameManager.CurrentLevel > 0)
				{
					saveFile.gameSettings.firstStart = false;
                    Debug.Log("Disabled first start help menu.");
                }
				Save();
				return i;
			}
		}
		return 0;
	}

	static internal void LevelComplete(int level, bool complete, int deaths, float time)
	{
		if (saveFile != null)
		{
			if (saveFile.levelInfos[level] != null)
			{
				saveFile.levelInfos[level].levelComplete = complete;
				switch (difficulty)
				{
					case 0:
						if (complete)
						{
							if (deaths < saveFile.levelInfos[level].kidDeaths)
								saveFile.levelInfos[level].kidDeaths = deaths;
							saveFile.levelInfos[level].kidTime = time;
						}
						saveFile.levelInfos[level].kidTotalDeaths += deaths;
						break;
					case 1:
						if (complete)
						{
							if (deaths < saveFile.levelInfos[level].hardDeaths)
								saveFile.levelInfos[level].hardDeaths = deaths;
							saveFile.levelInfos[level].hardTime = time;
						}
						saveFile.levelInfos[level].hardTotalDeaths += deaths;
						break;
					default:
						break;
				}
#if DEBUGGING
				Debug.Log(saveFile.levelInfos[level].ToString());
				Debug.Log(saveFile.currentLevel);
#endif
				CurrentLevelCheck();
			}
		}
	}

	static internal void NewGame()
	{
		LOCK = true;
		saveFile.levelInfos = null;
		saveFile.newItems = null;
		saveFile.gameSettings.firstStart = true;
		LOCK = false;
		Save();
	}

	static internal SaveFile GetSave()
	{
		//Load();
		return saveFile;
	}
	static internal void SetSaveFile(SaveFile saveFile)
	{
		SaveGameManager.saveFile = saveFile;
		Save();
	}
}

// A class must be serializable to be converted to and from JSON by JsonUtility.
[System.Serializable]
public class SaveFile
{
	//public Progress progress;
	//public AchievementSettings[] achievements;
	//public int highScore = 5000;
	public GameSettings gameSettings;
	public LevelInfo[] levelInfos;
	public NewItem[] newItems;
	public int currentLevel;
}