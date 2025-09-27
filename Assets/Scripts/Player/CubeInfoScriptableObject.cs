using UnityEngine;

[CreateAssetMenu(fileName = "CubeInfo", menuName = "CubeInfo")]

public class CubeInfoScriptableObject: ScriptableObject
{
	public float speed = 2;
	public float speedBoostSpeed = 4;
	public float jumpForce = 10;
	public float gravityScale = 1;
	public float jumpHeight = 2f;
	public int maxExtraJumps = 1;
	public float deathTime = 3;

	[Space]

	public Texture[] skins;
	public int currentSkin;

	[Space]

	public GameObject[] poppedEffects;
	public GameObject[] smashedEffects;
	public GameObject[] burnedEffectsA;
	public GameObject[] burnedEffectsB;
	public GameObject[] fallenEffects;
	public GameObject[] slidingEffects;

	[Space]

	public AudioClip[] poppedSoundEffects;
	public AudioClip[] smashedSoundEffects;
	public AudioClip[] burnedSoundEffects;
	public AudioClip[] fallenSoundEffects;
	public AudioClip[] slidingSoundEffects;
	public AudioClip[] hittingSoundEffects;
}
