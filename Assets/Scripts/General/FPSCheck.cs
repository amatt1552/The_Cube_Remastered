using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCheck : MonoBehaviour
{
	int FPS;

	void Update()
	{
		QualitySettings.vSyncCount = 0;
		FPS = (int)(1f / Time.unscaledDeltaTime);
		print(FPS + " fps");
	}
}
