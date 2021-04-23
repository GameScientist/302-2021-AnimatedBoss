using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Lifts the ribs up and down when stomping.
/// </summary>
public class BossRibsAnimator : MonoBehaviour
{
    /// <summary>
    /// The state controller.
    /// </summary>
    BossController boss;
    /// <summary>
    /// Decides on whether the start position has been stored at the very beginning of the attack.
    /// </summary>
    private bool startPositionStored;
    /// <summary>
    /// How much time has been spent during the attack state.
    /// </summary>
    private float currentTime = 0;
    /// <summary>
    /// The position of the ribs before the state was started.
    /// </summary>
    private Vector3 startPosition;
    /// <summary>
    /// Adjusts the elevation of the ribs during the attack state.
    /// </summary>
    public AnimationCurve stomp;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.bossState == BossController.BossStates.Attack) // The ribs are only adjusted during the attack state.
        {
            if (!startPositionStored)
            {
                startPosition = transform.position;
                startPositionStored = true;
            }
            currentTime += Time.deltaTime;
            float p = currentTime / 1f;
            Vector3 targetPosition = startPosition;
            targetPosition.y += stomp.Evaluate(p);
            transform.position = targetPosition;

        }
        else
        {
            startPositionStored = false;
            currentTime = 0;
        }
    }
}
