using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HipsAnimator : MonoBehaviour
{
    float rollAmount;
    GoonControler goon;
    Quaternion startingRot;
    // Start is called before the first frame update
    void Start()
    {
        startingRot = transform.localRotation;
        goon = GetComponentInParent<GoonControler>();
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
    }
    void AnimateIdle()
    {
        transform.localRotation = startingRot;
    }
    void AnimateWalk()
    {
        float time = Time.time * goon.stepSpeed * 2;
        float roll = Mathf.Cos(time) + rollAmount;

        transform.localRotation = Quaternion.Euler(0, 0, roll);
    }
}
