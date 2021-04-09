using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TorsoAnimator : MonoBehaviour
{
    /// <summary>
    /// The local-space starting rotation
    /// </summary>
    private Quaternion startingAngle;

    HeroController hero;

    private float spinAngle = 360;

    private float deathAngle = -90;

    public RotationConstraint bone;

    private float deathTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        hero = GetComponentInParent<HeroController>();
        startingAngle = transform.localRotation;
    }

    private void Update()
    {
        if (hero.state == HeroController.States.Attack)
        {
            bone.weight = 1;
            transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(startingAngle.y, spinAngle, hero.spinTime / 0.2f), transform.up);
        }
        else
        {
            bone.weight = 0;
            transform.rotation = Quaternion.AngleAxis(startingAngle.x, transform.up);
        }
    }
}
