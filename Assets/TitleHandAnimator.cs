using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleHandAnimator : MonoBehaviour
{
    private Vector3 startingPos;
    private Quaternion startingRot;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.localPosition;
        startingRot = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 finalPos = startingPos;

        float time = Time.time * 5;

        // lateral movement: (z + x)
        float frontToBack = Mathf.Sin(time);
        //finalPos += hero.moveDir * frontToBack * hero.walkScale.z;
        finalPos.z = frontToBack;

        // vertical movement: (y)
        finalPos.y = startingPos.y;

        transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, 0.0625f);

        //targetPos = transform.TransformPoint(finalPos);

        //targetRot = transform.parent.rotation * startingRot * Quaternion.Euler(0, 0, anklePitch);
        transform.localRotation = startingRot * Quaternion.Euler(0, 0, 0);
    }
}
