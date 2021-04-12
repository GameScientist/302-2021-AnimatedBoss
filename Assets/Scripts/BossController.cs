using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    public enum BossStates
    {
        Idle,
        Walk,
        Attack,
        Dead
    }
    public static class States
    {
        /// <summary>
        /// Sets the template for how switching between states is handled.
        /// </summary>
        public class State
        {
            /// <summary>
            /// Used to reference anything inside of that boss class that a state can update.
            /// </summary>
            protected BossController boss;
            /// <summary>
            /// Repeats each frame.
            /// </summary>
            /// <returns></returns>
            virtual public State Update()
            {
                return null;
            }
            /// <summary>
            /// Activates as soon as a state is started, while also referencing the boss class.
            /// </summary>
            /// <param name="boss"></param>
            virtual public void OnStart(BossController boss)
            {
                this.boss = boss;
            }
            /// <summary>
            /// Activates once the state is switched to something else.
            /// </summary>
            virtual public void OnEnd()
            {

            }
        }
        public class Idle : State
        {
            public float stateTime = 0;
            public override State Update()
            {
                stateTime += Time.deltaTime;
                if (stateTime >= 1) return new States.Walk();
                return null;
            }
            public override void OnStart(BossController boss)
            {
                boss.bossState = BossStates.Idle;
                base.OnStart(boss);
            }
            public override void OnEnd()
            {
                base.OnEnd();
            }
        }
        public class Walk : State
        {
            public override State Update()
            {
                boss.agent.SetDestination(boss.hero.position);
                if (Vector3.Distance(boss.transform.position, boss.hero.position) <= 50) return new States.Attack();
                return null;
            }
            public override void OnStart(BossController boss)
            {
                boss.bossState = BossStates.Walk;
                base.OnStart(boss);
            }
            public override void OnEnd()
            {
                base.OnEnd();
            }
        }
        public class Attack : State
        {
            public override State Update()
            {
                return base.Update();
            }
            public override void OnStart(BossController boss)
            {
                boss.bossState = BossStates.Attack;
                base.OnStart(boss);
            }
            public override void OnEnd()
            {
                base.OnEnd();
            }
        }
        public class Dead : State
        {
            public override State Update()
            {
                return base.Update();
            }
            public override void OnStart(BossController boss)
            {
                boss.bossState = BossStates.Dead;
                base.OnStart(boss);
            }
            public override void OnEnd()
            {
                base.OnEnd();
            }
        }
    }
    public Transform hero;
    private NavMeshAgent agent;
    public States.State state;
    public BossStates bossState { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        print(state);
    }
    
    // Update is called once per frame
    void Update()
    {
        StateManagement();
    }
    private void StateManagement()
    {
        if (state == null) SwitchState(new States.Idle());

        if (state != null)
        {
            if (state != null) SwitchState(state.Update());
        };
    }
    /// <summary>
    /// Changes the state to something different.
    /// </summary>
    /// <param name="newState"></param>
    private void SwitchState(States.State newState)
    {
        if (newState == null) return;

        if (state != null) state.OnEnd();

        state = newState;

        state.OnStart(this);
    }
}
