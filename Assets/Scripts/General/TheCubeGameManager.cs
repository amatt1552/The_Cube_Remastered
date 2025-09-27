using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(Fader))]
public class TheCubeGameManager : MonoBehaviour
{
	private static TheCubeGameManager GC;
	public static int CurrentLevel { get; private set; }
	public static int FinalLevel { get; private set; }
	AsyncOperation asyncManager;
	bool _startOverOneShot;
	public static GameObject player;
	public static GameObject playerMesh;
	static MeshRenderer _playerMeshRenderer;
	public static CubeInfoScriptableObject cubeInfoSO;
	public static Vector3 startScale;

	//music 
	public static bool turnDownTheRunning;
	public AudioSource idleMusic;
	public AudioSource runningMusic;
	public AudioSource menuMusic;
	public AudioMixer mixer;

	//constants
	const float RUNNING_MUSIC_MAX_VOLUME = 0.6f;
	const float IDLE_MUSIC_MAX_VOLUME = 1;
	const float ON_SPEED = 0.5f;//music
	const float OFF_SPEED = 0.5f;//music

	//level change fader
	public Fader levelFader { get; private set; }

	//Music fader
	public float musicFadeTime = 1;
	public Fader musicFader { get; private set; }

	//timer
	static float startTime;
	public static float timePassed { get; private set; }

	public static bool paused { get; private set; }
	static bool loadLevelOneshot;
	bool inited; //this insures that I dont initialize some varibles twice. I have to do it at awake as well as scene load.

	//training
	public Slideshow trainingMenu;
	public Hint hint;

	private void Awake()
	{
		levelFader = GetComponent<Fader>();
		musicFader = gameObject.AddComponent<Fader>();
		if (runningMusic == null)
		{
			Debug.LogWarning("runningMusic not set for " + name + "!");
			runningMusic = GetComponent<AudioSource>();
		}
		if (idleMusic == null)
		{
			Debug.LogWarning("idleMusic not set for " + name + "!");
			idleMusic = GetComponent<AudioSource>();
		}
		if (menuMusic == null)
		{
			Debug.LogWarning("menuMusic not set for " + name + "!");
			menuMusic = GetComponent<AudioSource>();
		}

		if (GC == null)
		{
			GC = this;
		}
		else
		{
			Destroy(this);
		}
		CurrentLevel = SceneManager.GetActiveScene().buildIndex;
		FinalLevel = SceneManager.sceneCountInBuildSettings - 2;
		//Debug.Log("final level " + finalLevel);
		GC.StartCoroutine("MenuEnum", 0);

		Init();

	}

	private void Update()
	{
		TurnDownTheRunning();
	}

	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	//since it doesn't destroy on load I needed a new "start".
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		Init();
		inited = false;
	}

	void Init()
	{

		if (!inited)
		{
			//sets start time
			//startTime = Time.time;

			//Debug.Log("SceneLoaded");
			levelFader.FadeToEndActivate();

			//finds player
			player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				playerMesh = player.transform.Find("playerMesh").gameObject;
				cubeInfoSO = player.GetComponent<CubeMotor>().cubeInfoSO;
				if (playerMesh != null && cubeInfoSO != null)
				{
					_playerMeshRenderer = playerMesh.GetComponent<MeshRenderer>();
					startScale = playerMesh.transform.localScale;
				}


			}

			//disables music at menu
			if (CurrentLevel == 0 && !menuMusic.enabled)
			{
				PlayMenu();
				levelFader.canvasGroup.alpha = 0;
			}
			else if (CurrentLevel == 0 && menuMusic.enabled)
			{

			}
			else if (!idleMusic.enabled)
			{
				PlayPlaying();
			}
			else
			{
				menuMusic.enabled = false;
			}
			inited = true;
		}
	}

	//Music
	public static void PlayMenu()
	{
		GC.StopCoroutine("PlayEnum");
		GC.StartCoroutine("MenuEnum",GC.musicFadeTime);
		
	}
	IEnumerator MenuEnum(float time)
	{
		
		musicFader.FadeTo(GC.idleMusic, 0, time, true);
		while (musicFader.fading)
		{
			//Debug.Log("music is fading");
			yield return null;
		}
		GC.idleMusic.enabled = false;
		GC.runningMusic.enabled = false;
		GC.menuMusic.volume = 0;
		GC.menuMusic.enabled = true;
		musicFader.FadeTo(GC.menuMusic, 1, time, true);
		
		
	}

	public static void PlayPlaying()
	{
		GC.StopCoroutine("MenuEnum");
		GC.StartCoroutine("PlayEnum", GC.musicFadeTime);
	}
	IEnumerator PlayEnum(float time)
	{
		musicFader.FadeTo(GC.menuMusic, 0, time, true);
		while (musicFader.fading)
		{
			//Debug.Log("music is fading");
			yield return null;
		}
		GC.menuMusic.enabled = false;
		GC.idleMusic.volume = 0;
		
		musicFader.FadeTo(GC.idleMusic, 1, time, true);
		GC.idleMusic.enabled = true;
		GC.runningMusic.enabled = true;

	}
	void TurnDownTheRunning()
	{
		if (CurrentLevel != 0)
		{
			if (turnDownTheRunning)
			{
				if (runningMusic.volume > 0)
				{
					runningMusic.volume -= OFF_SPEED * Time.deltaTime;
				}
				else
				{
					runningMusic.volume = 0;
				}
			}
			else
			{
				if (runningMusic.volume < RUNNING_MUSIC_MAX_VOLUME)
				{
					runningMusic.volume += ON_SPEED * Time.deltaTime;
				}
				else
				{
					runningMusic.volume = RUNNING_MUSIC_MAX_VOLUME;
				}
			}
		}
	}

	public static AudioMixerGroup GetMixerGroup(string mixerGroupName)
	{
		if (GC.mixer != null)
		{
			AudioMixerGroup[] mixerGroup;
			mixerGroup = GC.mixer.FindMatchingGroups(mixerGroupName);
			if (mixerGroup.Length > 0)
			{
				return mixerGroup[0];
			}
			else
			{
				Debug.LogWarning("No AudioMixerGroup found named " + mixerGroupName);
				return null;
			}
		}

		Debug.LogWarning("No AudioMixer set in Inspector.");

		return null;
	}

	//levels
	public static void LoadNextLevel()
	{
		if (!loadLevelOneshot)
		{
			GC.levelFader.FadeToStart();
			GC.StartCoroutine("WaitToLoad", CurrentLevel + 1);
			loadLevelOneshot = true;
		}
	}

	public void LoadLevel(int level)
	{
		if (!loadLevelOneshot)
		{
			levelFader.FadeToStart();
			GC.StartCoroutine("WaitToLoad", level);
			loadLevelOneshot = true;
		}
	}
	/// <summary>
	/// Static version of load level. Easier for scripts to access this way.
	/// </summary>
	/// <param name="level"></param>
	public static void LoadLevelStatic(int level)
	{
        GC.LoadLevel(level);
	}

	IEnumerator WaitToLoad(int level)
	{
		while (levelFader.fading)
		{
			yield return null;
		}
		if (LoadComplete())
		{
			GC.asyncManager = SceneManager.LoadSceneAsync(level);
			loadLevelOneshot = false;
			CheckpointManager.RemoveAllCheckpoints();
		}
	}

	public void NewGame()
	{
		SaveGameManager.NewGame();
		LoadNextLevel();
	}

	public void RestartLevel()
	{
		if (!loadLevelOneshot)
		{
			LoadLevel(CurrentLevel);
			loadLevelOneshot = true;
		}
	}
	
	public static void Continue()
	{
        int currentLevel = SaveGameManager.GetSave().currentLevel;
		//Debug.Log($"current level {currentLevel}");
		//Debug.Log($"final level {FinalLevel}");
        GC.LoadLevel(currentLevel < FinalLevel ? currentLevel : currentLevel - 1);
	}

	public static bool LoadComplete()
	{
		CurrentLevel = SceneManager.GetActiveScene().buildIndex;
		if (GC.asyncManager != null)
			return GC.asyncManager.isDone;
		return true;
	}

	public static void UpdatePlayerTexture()
	{
		if (_playerMeshRenderer != null)
		{
			_playerMeshRenderer.material.mainTexture = cubeInfoSO.skins[cubeInfoSO.currentSkin];

		}
	}

	public static void UpdatePlayerColor(Color color)
	{
		if (_playerMeshRenderer != null)
		{
			_playerMeshRenderer.material.color = color;

		}
	}

	public static AudioMixer GetMixer()
	{
		return GC.mixer;
	}
	
	public static void SetPause(bool pause)
	{
		if (pause)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
		paused = pause;
	}

	public static bool Fading()
	{
		return GC.levelFader.fading;
	}

	public static Slideshow GetSlideshow()
	{
		return GC.trainingMenu;
	}

	public static Hint GetHint()
	{
		return GC.hint;
	}
}
