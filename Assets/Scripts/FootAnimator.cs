using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls where each foot is pointing towards.
/// </summary>
public class FootAnimator : MonoBehaviour
{
    /// <summary>
    /// The state controller.
    /// </summary>
    HeroController hero;

    /// <summary>
    /// The local-space starting position of this object.
    /// </summary>
    private Vector3 startingPos;

    /// <summary>
    /// The local-space starting rotation
    /// </summary>
    private Quaternion startingRot;

    /// <summary>
    /// Determines if this foot has an offset to it.
    /// </summary>
    public bool offset;

    // Start is called before the first frame update
    void Start()
    {
        hero = GetComponentInParent<HeroController>();
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        switch (hero.State)
        {
            case HeroController.States.Idle:
                AnimateIdle();
                break;
            case HeroController.States.Walk:
                if (offset) AnimateWalk(1.57f, 0.125f);
                else AnimateWalk(0f, 0.125f);
                break;
            case HeroController.States.Jog:
                if (offset) AnimateWalk(3.14f, 0.25f);
                else AnimateWalk(0f, 0.25f);
                break;
            case HeroController.States.Run:
                if (offset) AnimateWalk(2.355f, 0.25f);
                else AnimateWalk(0f, 0.25f);
                break;
            case HeroController.States.Jump:
                Stretch(-2, 0.03125f);
                break;
            case HeroController.States.Fall:
                Stretch(2, 0.015625f);
                break;
        }
    }

    void AnimateWalk(float stepOffset, float easing)
    {
        Vector3 finalPos = startingPos;

        float time = (Time.time + stepOffset) * hero.stepSpeed;

        // lateral movement: (z + x)
        float frontToBack = Mathf.Sin(time);
        finalPos.z = frontToBack*hero.walkScale.z;

        // vertical movement: (y)
        finalPos.y += Mathf.Cos(time) * hero.walkScale.y;

        finalPos.x *= hero.walkScale.x;

        bool isOnGround = (finalPos.y < startingPos.y);

        if (isOnGround) finalPos.y = startingPos.y;

        // convert from z(-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(frontToBack);

        float anklePitch = isOnGround ? 0 : -p * 20;

        transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, easing);

        transform.localRotation = startingRot * Quaternion.Euler(0, 0, anklePitch);
    }

    void AnimateIdle()
    {
        transform.localPosition = AnimMath.Lerp(transform.localPosition, startingPos, 0.0625f);
        Ray ray = new Ray(transform.position + new Vector3(0, .5f, 0), Vector3.down * 2);

        Debug.DrawRay(ray.origin, ray.direction);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }
    private void Stretch(float z, float easing)
    {
        Vector3 finalPos = startingPos;
        finalPos.z = z;
        transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, easing);
    }
}
