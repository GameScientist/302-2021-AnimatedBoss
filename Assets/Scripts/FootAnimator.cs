using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootAnimator : MonoBehaviour
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
    /// An offset value to use for timing of the Sin wave that controls the walk animation.
    /// 
    /// A value of Mathf.PI would be half-a-period.
    /// </summary>
    public float stepOffset = 0;

    HeroController hero;

    private Vector3 targetPos;
    private Quaternion targetRot;

    // Start is called before the first frame update
    void Start()
    {
        hero = GetComponentInParent<HeroController>();
        print(hero);
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        switch (hero.state)
        {
            case HeroController.States.Idle:
                AnimateIdle();
                break;
            case HeroController.States.Move:
                AnimateWalk();
                break;
        }

        //transform.position = AnimMath.Slide(transform.position, targetPos, .01f);
        //transform.rotation = AnimMath.Slide(transform.rotation, targetRot, .01f);
    }

    void AnimateWalk()
    {
        Vector3 finalPos = startingPos;

        float time = (Time.time + stepOffset) * hero.stepSpeed;

        // lateral movement: (z + x)
        float frontToBack = Mathf.Sin(time);
        //finalPos += hero.moveDir * frontToBack * hero.walkScale.z;
        finalPos.z = frontToBack*hero.walkScale.z;

        // vertical movement: (y)
        finalPos.y += Mathf.Cos(time) * hero.walkScale.y;

        finalPos.x *= hero.walkScale.x;

        bool isOnGround = (finalPos.y < startingPos.y);

        if (isOnGround) finalPos.y = startingPos.y;

        // convert from z(-1 to 1) to p (0 to 1 to 0)
        float p = 1 - Mathf.Abs(frontToBack);

        float anklePitch = isOnGround ? 0 : -p * 20;

        transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, 0.1f);

        //targetPos = transform.TransformPoint(finalPos);

        //targetRot = transform.parent.rotation * startingRot * Quaternion.Euler(0, 0, anklePitch);
        transform.localRotation = startingRot * Quaternion.Euler(0, 0, anklePitch);
    }

    void AnimateIdle()
    {
        transform.localPosition = AnimMath.Lerp(transform.localPosition, startingPos, 0.1f);
        //transform.localRotation = startingRot;

        //targetPos = transform.TransformPoint(startingPos);
        //targetRot = transform.parent.rotation * startingRot;

        FindGround();
    }
    void FindGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, .5f, 0), Vector3.down * 2);

        Debug.DrawRay(ray.origin, ray.direction);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {

        }
    }
}
