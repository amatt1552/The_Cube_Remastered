using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]

public class LightingSetter : MonoBehaviour 
{
	public enum AmbientMode
	{
		Skybox, Color, Gradient
	}
	public enum ReflectionMode
	{
		Skybox, Custom
	}
	public enum ReflectionResolution
	{
		_16 = 16,
		_32 = 32,
		_64 = 64,
		_128 = 128,
		_256 = 256,
		_512 = 512,
		_1024 = 1024,
		_2048 = 2048
	}
	
	public static LightingSetter GC;

	[Tooltip("tag a light with Sun to set automaticaly.")]
	public Light sun;
	[Tooltip("tag a camera with MainCamera to set automaticaly.")]
	public Camera mainCamera;
	public Color cameraBackground = Color.blue;
	
	//skybox

	public List<Material> skyboxes = new List<Material>();
	public int currentSkybox;
	public Color skyBoxColor;
	public bool rotating;
	public float startRotation;
	public float rotateSpeed;
	float rotateVal;

	public AmbientMode ambientMode;

	//color settings

	public Color ambientColor;
	
	//gradient settings
	
	public Color skyColor = Color.white;
	public Color equatorColor = Color.gray;
	public Color groundColor = Color.black;

	//skybox settings
	[Range(0,8)]
	public float skyboxIntensityMultiplier = 1;

	[Tooltip("sets if ambient color is baked or realtime")]

	//reflections stuff

	public ReflectionMode reflectionMode;
	public Cubemap reflectionCubeMap;
	public ReflectionResolution reflectionResolution;
	[Range(0,1)]
	public float reflectionIntensity;
	[Range(1,5)]
	public int reflectionBounces;

	//baking settings
	
	//addFog

	void Start()
	{
		if(mainCamera == null)
		{
			mainCamera = Camera.main;
		}
		if (GC == null)
		{
			GC = this;
		}
		StartCoroutine("Rotate");
	}

	IEnumerator Rotate()
	{
		while (1 == 1)
		{
			if (rotating)
			{
				rotateVal += rotateSpeed * Time.deltaTime;
				RenderSettings.skybox.SetFloat("_Rotation", rotateVal);
			}
			if (rotateVal >= 360)
			{
				rotateVal = 0;
			}
			yield return null;
		}
	}

	public void UpdateSettings () 
	{
		
		RenderSettings.sun = sun;
		switch (ambientMode)
		{
			case AmbientMode.Skybox:
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
				RenderSettings.skybox = skyboxes[currentSkybox];
				RenderSettings.skybox.SetColor("_Tint", skyBoxColor);
				RenderSettings.ambientIntensity = skyboxIntensityMultiplier;
				RenderSettings.skybox.SetFloat("_Rotation", startRotation);
				break;
			case AmbientMode.Color:
				RenderSettings.skybox = null;
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
				RenderSettings.ambientSkyColor = ambientColor;
				break;
			case AmbientMode.Gradient:
				RenderSettings.skybox = null;
				RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;				
				RenderSettings.ambientSkyColor = skyColor;
				RenderSettings.ambientEquatorColor = equatorColor;
				RenderSettings.ambientGroundColor = groundColor;
				break;
		}

		switch (reflectionMode)
		{
			case ReflectionMode.Skybox:
				RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
				RenderSettings.defaultReflectionResolution = (int)reflectionResolution;
				RenderSettings.reflectionIntensity = reflectionIntensity;
				RenderSettings.reflectionBounces = reflectionBounces;
				break;
			case ReflectionMode.Custom:
				RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
				RenderSettings.customReflectionTexture = reflectionCubeMap;
				RenderSettings.reflectionIntensity = reflectionIntensity;
				RenderSettings.reflectionBounces = reflectionBounces;
				break;
		}
		
		mainCamera.backgroundColor = cameraBackground;

		if(currentSkybox >= skyboxes.Count)
		{
			currentSkybox = skyboxes.Count - 1;
		}
	}

	public void FindSun()
	{
		GameObject sunObj = GameObject.FindGameObjectWithTag("Sun");
		if (sunObj != null)
		{
			sun = sunObj.GetComponent<Light>();
		}
	}

	public void CopyCurrentSettings ()
	{
		sun = RenderSettings.sun;
		switch (RenderSettings.ambientMode)
		{
			case UnityEngine.Rendering.AmbientMode.Skybox:
				ambientMode = AmbientMode.Skybox;

				//makes sure the skybox gets set correctly

				if (skyboxes.Contains(RenderSettings.skybox))
				{
					for (int i = 0; i < skyboxes.Count; i++)
					{
						if (skyboxes[i].Equals(RenderSettings.skybox))
						{
							currentSkybox = i;
							break;
						}
					}
					RenderSettings.skybox = skyboxes[currentSkybox];
				}
				else
				{
					skyboxes.Add(RenderSettings.skybox);
					currentSkybox = skyboxes.Count - 1;
				}

				skyBoxColor = RenderSettings.skybox.GetColor("_Tint");
				skyboxIntensityMultiplier = RenderSettings.ambientIntensity;
				break;
			case UnityEngine.Rendering.AmbientMode.Flat:
				RenderSettings.skybox = null;
				ambientMode = AmbientMode.Color;
				ambientColor = RenderSettings.ambientSkyColor;
				break;
			case UnityEngine.Rendering.AmbientMode.Trilight:
				RenderSettings.skybox = null;
				ambientMode = AmbientMode.Gradient;
				skyColor = RenderSettings.ambientSkyColor;
				equatorColor = RenderSettings.ambientEquatorColor;
				groundColor = RenderSettings.ambientGroundColor;
				break;
		}

		switch (RenderSettings.defaultReflectionMode)
		{
			case DefaultReflectionMode.Skybox:
				reflectionMode = ReflectionMode.Skybox;
				reflectionResolution = (ReflectionResolution)RenderSettings.defaultReflectionResolution;
				reflectionIntensity = RenderSettings.reflectionIntensity;
				reflectionBounces = RenderSettings.reflectionBounces;
				break;
			case DefaultReflectionMode.Custom:
				reflectionMode = ReflectionMode.Custom;
                reflectionCubeMap = (Cubemap)RenderSettings.customReflectionTexture;
				reflectionIntensity = RenderSettings.reflectionIntensity;
				reflectionBounces = RenderSettings.reflectionBounces;
				break;
		}

		cameraBackground = mainCamera.backgroundColor;
		
	}
	
}
