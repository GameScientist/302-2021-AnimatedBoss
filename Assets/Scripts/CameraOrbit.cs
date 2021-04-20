using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public HeroController hero;
    private Camera cam; // The camera that is attached to the rig.

    private float yaw = 0;
    private float pitch = 0;

    private float cameraSensitivityX = 4; // How quickly the player can move the camera horizontally.
    private float cameraSensitivityY = 1; // How quickly the player can move the camera vertically.

    public bool zoomedOut;

    public float shakeIntensity = 0;
    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = hero.transform.position;
        RotateCamera();
        ZoomCamera();
        //ShakeCamera();
    }

    private void RotateCamera()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0) MouseLook();
        else if (Input.GetAxisRaw("AimHorizontal") != 0 || Input.GetAxisRaw("AimVertical") != 0) StickLook();
    }

    private void MouseLook()
    {
        float mx = Input.GetAxisRaw("Mouse X");
        float my = Input.GetAxisRaw("Mouse Y");

        yaw += mx * cameraSensitivityX;
        pitch -= my * cameraSensitivityY;

        if (zoomedOut) pitch = Mathf.Clamp(pitch, -15, 90);
        else pitch = Mathf.Clamp(pitch, -5, 90);

        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }

    private void StickLook()
    {
        float mx = Input.GetAxisRaw("AimHorizontal");
        float my = Input.GetAxisRaw("AimVertical");

        yaw += mx * cameraSensitivityX;
        pitch -= my * cameraSensitivityY;

        if (zoomedOut) pitch = Mathf.Clamp(pitch, -15, 90);
        else pitch = Mathf.Clamp(pitch, -5, 90);

        print(pitch);
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }

    private void ZoomCamera()
    {
        if (zoomedOut) cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 25, -75), .001f);
        else cam.transform.localPosition = AnimMath.Slide(cam.transform.localPosition, new Vector3(0, 2, -12), .001f);
    }

    /*private void ShakeCamera()
    {
        if (shakeIntensity < 0) shakeIntensity = 0;

        if (shakeIntensity > 0) shakeIntensity -= Time.deltaTime;
        else return; // shake intentisty is 0, so do nothing...

        // pick a SMALL random rotation:
        Quaternion targetRot = AnimMath.Lerp(Random.rotation, Quaternion.identity, .99f);

        // cam.transform.localRotation *= targetRot
        cam.transform.localRotation = AnimMath.Lerp(cam.transform.localRotation, cam.transform.localRotation * targetRot, shakeIntensity * shakeIntensity);
    }*/
}
