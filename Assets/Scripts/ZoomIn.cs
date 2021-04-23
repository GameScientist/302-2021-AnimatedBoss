using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls how close the camera is to the player.
/// </summary>
public class ZoomIn : MonoBehaviour
{
    /// <summary>
    /// The camera that is being zoomed in and out.
    /// </summary>
    public CameraOrbit cam;
    private void OnTriggerEnter(Collider other) // Zooms out the camera once the trigger is entered.
    {
        if (other.CompareTag("Player")) cam.zoomedOut = true;
    }

    private void OnTriggerExit(Collider other) // Zooms in the camera once the trigger is left.
    {
        if (other.CompareTag("Player")) cam.zoomedOut = false;
    }
}
