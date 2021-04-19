using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomIn : MonoBehaviour
{
    public CameraOrbit cam;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) cam.zoomedOut = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) cam.zoomedOut = false;
    }
}
