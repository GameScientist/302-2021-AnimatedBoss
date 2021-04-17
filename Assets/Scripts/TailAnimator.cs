using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAnimator : MonoBehaviour
{
    private Vector3 startingPos;
    private BossController boss;
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
            case BossController.BossStates.Idle:

                transform.localPosition = AnimMath.Lerp(transform.localPosition, new Vector3 (0, -7.186531f, -51.59557f), 0.3f);
                break;
            case BossController.BossStates.Walk:
                Vector3 finalPos = startingPos;

                // lateral movement: (z + x)
                finalPos.x = Mathf.Sin(Time.time * 5) * 50;

                transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, 0.5f);
                break;
            case BossController.BossStates.Attack:
                transform.localPosition = AnimMath.Lerp(transform.localPosition, new Vector3(Mathf.Sin(Time.time * 10) * 25, 40, -51.59557f), 0.5f);
                break;
            case BossController.BossStates.Dead:
                transform.localPosition = AnimMath.Lerp(transform.localPosition, new Vector3(Mathf.Sin(Time.time)*10, -30, -51.59557f), 0.001f);
                break;
        }
    }
}
