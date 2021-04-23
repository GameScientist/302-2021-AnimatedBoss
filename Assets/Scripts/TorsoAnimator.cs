using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class TorsoAnimator : MonoBehaviour
{
    /// <summary>
    /// The player character.
    /// </summary>
    HeroController hero;

    /// <summary>
    /// The local-space starting rotation
    /// </summary>
    private Quaternion startingAngle;

    /// <summary>
    /// The bone controlling the torso.
    /// </summary>
    public RotationConstraint bone;

    // Start is called before the first frame update
    void Start()
    {
        hero = GetComponentInParent<HeroController>();
        startingAngle = transform.localRotation;
    }

    private void Update()
    {
        if (hero.State == HeroController.States.Attack) SetRotation(1, Mathf.Lerp(startingAngle.y, 360, hero.spinTime / 0.2f)); // The character is turned around quickly when attacking.
        else SetRotation(0, startingAngle.x); // The character is kept in place.
    }

    /// <summary>
    /// Sets the rotation of the player character.
    /// </summary>
    /// <param name="boneWeight"></param>
    /// <param name="angle"></param>
    private void SetRotation(float boneWeight, float angle)
    {
        bone.weight = boneWeight;
        transform.rotation = Quaternion.AngleAxis(angle, transform.up);
    }
}
