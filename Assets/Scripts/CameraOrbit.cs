using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    private readonly float cameraSensitivityX = 360; // How quickly the player can move the camera horizontally.
    private readonly float cameraSensitivityY = 90; // How quickly the player can move the camera vertically.
    private float yaw = 0;
    private float pitch = 0;

    private Camera cam; // The camera that is attached to the rig.

    public bool zoomedOut;
    public float shakeIntensity = 0;
    public HeroController target;

    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.transform.position;
        ZoomCamera();
        RotateCamera();
    }
    /// <summary>
    /// Rotates the camera around the player character using either mouse or analog stick input.
    /// </summary>
    private void RotateCamera()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            yaw += Input.GetAxisRaw("Mouse X") * cameraSensitivityX * Time.deltaTime;
            pitch -= Input.GetAxisRaw("Mouse Y") * cameraSensitivityY * Time.deltaTime;
        }
        else if (Input.GetAxisRaw("AimHorizontal") != 0 || Input.GetAxisRaw("AimVertical") != 0)
        {
            yaw += Input.GetAxisRaw("AimHorizontal") * cameraSensitivityX * Time.deltaTime;
            pitch -= Input.GetAxisRaw("AimVertical") * cameraSensitivityY * Time.deltaTime;
        }

        if (zoomedOut) pitch = Mathf.Clamp(pitch, -15, 90);
        else pitch = Mathf.Clamp(pitch, -5, 90);

        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }
    /// <summary>
    /// Adjusts how far away the camera is from the player character.
    /// </summary>
    private void ZoomCamera()
    {
        if (zoomedOut) cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 25, -75), .001f);
        else cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 2, -12), .001f);
    }
}
