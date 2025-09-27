using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnDistance : MonoBehaviour
{
    GameObject player;
    Light spotlight;
    float maxDistance = 20f;

    private void Start()
    {
        player = TheCubeGameManager.player;
        spotlight = GetComponent<Light>();
        if (spotlight == null ) enabled = false;
    }
    private void FixedUpdate()
    {
        if (Vector3.Distance(player.transform.position, spotlight.transform.position) > maxDistance) 
        {
            spotlight.enabled = false;
        }
        else 
        {
            spotlight.enabled = true;
        }
    }
}
