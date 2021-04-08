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
        else if (hero.state == HeroController.States.Dead)
        {
            bone.weight = 1;
            if (deathTime <= 0.5f) transform.position = Vector3.Lerp(transform.up * 1.5f, transform.up * 3, deathTime/2);
            else transform.position = Vector3.Lerp(transform.up * 3, transform.up * 0, deathTime);
            transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(startingAngle.x, deathAngle, hero.spinTime), transform.right);
            deathTime += Time.deltaTime;
            deathTime = Mathf.Clamp(deathTime, 0f, 1f);
            print(transform.rotation);
        }
        else
        {
            bone.weight = 0;
            transform.rotation = Quaternion.AngleAxis(startingAngle.x, transform.up);
        }
    }
}
