using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script animates the foot / legs.
/// Chaing the local position of this object (ik target).
/// </summary>
public class GoonFootAnimator : MonoBehaviour
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

    GoonControler goon;

    private Vector3 targetPos;
    private Quaternion targetRot;

    // Start is called before the first frame update
    void Start()
    {
        goon = GetComponentInParent<GoonControler>();
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        switch (goon.state)
        {
            case GoonControler.States.Idle:
                AnimateIdle();
                break;
            case GoonControler.States.Walk:
                AnimateWalk();
                break;
        }

        //transform.position = AnimMath.Slide(transform.position, targetPos, .01f);
        //transform.rotation = AnimMath.Slide(transform.rotation, targetRot, .01f);
    }

    void AnimateWalk()
    {
        Vector3 finalPos = startingPos;

        float time = (Time.time + stepOffset) * goon.stepSpeed;

        // lateral movement: (z + x)
        float frontToBack = Mathf.Sin(time);
        finalPos += goon.moveDir * frontToBack * goon.walkScale.z;

        // vertical movement: (y)
        finalPos.y += Mathf.Cos(time) * goon.walkScale.y;

        finalPos.x *= goon.walkScale.x;

        bool isOnGround = (finalPos.y < startingPos.y);

        if (isOnGround) finalPos.y = startingPos.y;

        // convert from z(-1 to 1) to p (0 to 1 to 0)
        float p = 1-Mathf.Abs(frontToBack);

        float anklePitch = isOnGround ? 0 : -p * 20;

        transform.localPosition = finalPos;

        //targetPos = transform.TransformPoint(finalPos);

        //targetRot = transform.parent.rotation * startingRot * Quaternion.Euler(0, 0, anklePitch);
        transform.localRotation = startingRot * Quaternion.Euler(0, 0, anklePitch);
    }

    void AnimateIdle()
    {
        //transform.localPosition = startingPos;
        //transform.localRotation = startingRot;

        //targetPos = transform.TransformPoint(startingPos);
        //targetRot = transform.parent.rotation * startingRot;
        
        FindGround();
    }
    void FindGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0,.5f, 0), Vector3.down*2);

        Debug.DrawRay(ray.origin, ray.direction);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;

            transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
        else
        {

        }
    }
}
