using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Adjusts the IK of the boss's head depending on the state to allow for some expression.
/// </summary>
public class BossHeadAnimator : MonoBehaviour
{
    /// <summary>
    /// The state controller.
    /// </summary>
    private BossController boss;
    /// <summary>
    /// The amount of time the boss spends in the idle state before swaying its head to the other side.
    /// </summary>
    private float idleTime = 3.75f;
    /// <summary>
    /// The amount of time the boss has spent winding up its stomp.
    /// </summary>
    private float attackTime = 0.5f;
    /// <summary>
    /// The target the boss is meant to be looking at when walking.
    /// </summary>
    public Transform hero;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossController>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (boss.bossState)
        {
            case BossController.BossStates.Idle: // Turns head from side to side trying to find the player again.
                attackTime = 0.5f;
                if (idleTime <= 0) transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(-45, -45, 30.2969398f), 0.75f);
                else
                {
                    transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(45, -45, 30.2969398f), 0.75f);
                    idleTime -= Time.deltaTime;
                }
                break;
            case BossController.BossStates.Walk:// Looks at the player while trying to walk towards it.
                idleTime = 3.75f;
                transform.position = AnimMath.Slide(transform.position, hero.position, 0.5f);
                break;
            case BossController.BossStates.Attack: // Keeps its head at the last position.
                if (attackTime <= 0) transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(0, -45, 58.30734f), 0.125f);
                else
                {
                    transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(0, 77, 30.2969398f), 0.125f);
                    attackTime -= Time.deltaTime;
                }
                break;
            case BossController.BossStates.Dead: // Hangs its head low in defeat.
                transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(0, -50, 30.29694f), 0.999f);
                break;
        }
    }
}
