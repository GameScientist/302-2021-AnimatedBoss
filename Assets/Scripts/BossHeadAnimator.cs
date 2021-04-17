using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHeadAnimator : MonoBehaviour
{
    public Transform hero;
    private BossController boss;
    private float idleTime = 3.75f;
    private float attackTime = 0.5f;
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
                attackTime = 0.5f;
                if (idleTime <= 0) transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(-45, -45, 30.2969398f), 0.75f);
                else
                {
                    transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(45, -45, 30.2969398f), 0.75f);
                    idleTime -= Time.deltaTime;
                }
                break;
            case BossController.BossStates.Walk:
                idleTime = 3.75f;
                transform.position = AnimMath.Slide(transform.position, hero.position, 0.5f);
                break;
            case BossController.BossStates.Attack:
                if (attackTime <= 0) transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(0, -45, 58.30734f), 0.125f);
                else
                {
                    transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(0, 77, 30.2969398f), 0.125f);
                    attackTime -= Time.deltaTime;
                }
                break;
            case BossController.BossStates.Dead:
                transform.localPosition = AnimMath.Slide(transform.localPosition, new Vector3(0, -50, 30.29694f), 0.999f);
                break;
        }
    }
}
