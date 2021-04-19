using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    public HeroController hero;

    private float yaw = 0;
    private float pitch = 0;

    private float cameraSensitivityX = 4; // How quickly the player can move the camera horizontally.
    private float cameraSensitivityY = 1; // How quickly the player can move the camera vertically.
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = hero.transform.position;
        float mx = Input.GetAxisRaw("Mouse X");
        float my = Input.GetAxisRaw("Mouse Y");

        yaw += mx * cameraSensitivityX;
        pitch -= my * cameraSensitivityY;

        pitch = Mathf.Clamp(pitch, -5, 90);
        print(pitch);
        transform.rotation = AnimMath.Slide(transform.rotation, Quaternion.Euler(pitch, yaw, 0), .001f);
    }
}
