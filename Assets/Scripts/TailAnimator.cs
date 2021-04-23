using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Wags the tail depending on the boss's current state.
/// </summary>
public class TailAnimator : MonoBehaviour
{
    /// <summary>
    /// The state controller.
    /// </summary>
    private BossController boss;
    /// <summary>
    /// The location of the tail IK when starting the game.
    /// </summary>
    private Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossController>();
        startingPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {

        switch (boss.bossState)
        {
            case BossController.BossStates.Idle: // Stops wagging tail out of confusion. "Where did he go?"
                transform.localPosition = AnimMath.Lerp(transform.localPosition, new Vector3 (0, -7.186531f, -51.59557f), 0.3f);
                break;
            case BossController.BossStates.Walk: // Wags tail out of excitement. "Get over here!"
                Vector3 finalPos = startingPos;
                finalPos.x = Mathf.Sin(Time.time * 5) * 50;
                transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, 0.5f);
                break;
            case BossController.BossStates.Attack: // Lifts and lowers tail while stomping. "Take this!"
                transform.localPosition = AnimMath.Lerp(transform.localPosition, new Vector3(Mathf.Sin(Time.time * 10) * 25, 40, -51.59557f), 0.5f);
                break;
            case BossController.BossStates.Dead: // Slowly hangs tail downward in defeat. "I guess you win..."
                transform.localPosition = AnimMath.Lerp(transform.localPosition, new Vector3(Mathf.Sin(Time.time)*10, -30, -51.59557f), 0.001f);
                break;
        }
    }
}
