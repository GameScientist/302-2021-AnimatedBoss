using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Controls where each hand is pointing to.
/// </summary>
public class HandAnimator : MonoBehaviour
{
    /// <summary>
    /// The local-space starting position of this object.
    /// </summary>
    private Vector3 startingPos;

    /// <summary>
    /// The local-space starting rotation
    /// </summary>
    private Quaternion startingRot;

    /// <summary>
    /// Determines if this hand has an offset to it.
    /// </summary>
    public bool offset;

    /// <summary>
    /// The state controller.
    /// </summary>
    HeroController hero;

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
                Swing(0, 2f, 0.0625f);
                break;
            case HeroController.States.Walk:
                if (offset) Swing(1.57f, 1f, 0.125f);
                else Swing(0f, 1f, 0.125f);
                break;
            case HeroController.States.Jog:
                if (offset) Swing(3.14f, 1f, 0.25f);
                else Swing(0f, 1f, 0.25f);
                break;
            case HeroController.States.Run:
                if (offset) Swing(2.355f, 1f, 0.25f);
                else Swing(0f, 1f, 0.25f);
                break;
            case HeroController.States.Jump:
                Stretch(2.5f, 2, 0.03125f);
                break;
            case HeroController.States.Fall:
                Stretch(2.5f, -2, 0.015625f);
                break;
            case HeroController.States.Attack:
                Stretch(0.5f, 0, 0.5f);
                break;
        }
    }
    /// <summary>
    /// Swings the player's arms to and fro.
    /// </summary>
    /// <param name="stepOffset"></param>
    /// <param name="restraint"></param>
    /// <param name="easing"></param>
    private void Swing(float stepOffset, float restraint, float easing)
    {
        Vector3 finalPos = startingPos;

        float time = (Time.time + stepOffset) * hero.stepSpeed / restraint;

        // lateral movement: (z + x)
        float frontToBack = Mathf.Sin(time);
        //finalPos += hero.moveDir * frontToBack * hero.walkScale.z;
        finalPos.z = frontToBack * hero.walkScale.z / restraint;

        // vertical movement: (y)
        if(restraint != 2f)finalPos.y += Mathf.Cos(time) * hero.walkScale.y / restraint;

        finalPos.x *= hero.walkScale.x;

        bool isOnGround = (finalPos.y < startingPos.y);

        if (isOnGround) finalPos.y = startingPos.y;

        // convert from z(-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(frontToBack);

        float anklePitch = isOnGround ? 0 : -p * 20;

        transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, easing);

        transform.localRotation = startingRot * Quaternion.Euler(0, 0, anklePitch);

    }
    /// <summary>
    /// Stretches the player's limbs out to simulate jumping.
    /// </summary>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <param name="easing"></param>
    private void Stretch(float y, float z, float easing)
    {
        Vector3 finalPos = startingPos;
        finalPos.y = y;
        finalPos.z = z;
        transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, easing);
    }
}
