using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRibsAnimator : MonoBehaviour
{
    BossController boss;
    private float currentTime = 0;
    private float timeLength = 1;
    public AnimationCurve stomp;
    private Vector3 startPosition;
    private bool startPositionStored;
    // Start is called before the first frame update
    void Start()
    {
        boss = GetComponentInParent<BossController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boss.bossState == BossController.BossStates.Attack)
        {
            if (!startPositionStored)
            {
                startPosition = transform.position;
                startPositionStored = true;
            }
            currentTime += Time.deltaTime;
            float p = currentTime / timeLength;
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
