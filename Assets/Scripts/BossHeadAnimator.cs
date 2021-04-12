using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadAnimator : MonoBehaviour
{
    public Transform hero;
    private BossController boss;
    private float idleTime = 0.5f;
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
            case BossController.BossStates.Idle:
                if (idleTime <= 0)
                transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(-45, -45, 30.2969398f), 0.5f);
                else
                {
                    transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(45, -45, 30.2969398f), 0.5f);
                    idleTime -= Time.deltaTime;
                }
                break;
            case BossController.BossStates.Walk:
                idleTime = 0.5f;
                transform.position = AnimMath.Slide(transform.position, hero.position, 0.5f);
                break;
            case BossController.BossStates.Attack:
                break;
            case BossController.BossStates.Dead:
                break;
        }
    }
}
