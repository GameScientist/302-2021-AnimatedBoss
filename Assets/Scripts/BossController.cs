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
                if (stateTime >= 7.5f) return new States.Walk();
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
                boss.agent.ResetPath();
                base.OnEnd();
            }
        }
        public class Attack : State
        {
            float stateTime = 0;
            public override State Update()
            {
                stateTime += Time.deltaTime;
                if (stateTime >= 1) return new States.Idle();
                return null;
            }
            public override void OnStart(BossController boss)
            {
                boss.bossState = BossStates.Attack;
                boss.audioManager.Play("Roar");
                base.OnStart(boss);
            }
            public override void OnEnd()
            {
                boss.audioManager.Play("Stomp");
                if(Vector3.Distance(boss.transform.position, boss.hero.position) <= 50)
                {
                    Health health = boss.hero.GetComponent<Health>();
                    health.Damage();
                }
                base.OnEnd();
            }
        }
        public class Dead : State
        {
            public override State Update()
            {
                boss.agent.SetDestination(boss.exit.position);
                return null;
            }
            public override void OnStart(BossController boss)
            {
                boss.bossState = BossStates.Dead;
                boss.audioManager.Play("Monster Defeat");
                base.OnStart(boss);
            }
            public override void OnEnd()
            {
                base.OnEnd();
            }
        }
    }
    public Transform hero;
    public Transform exit;
    private NavMeshAgent agent;
    public States.State state;
    public List<StickyFeet> feet = new List<StickyFeet>();
    private Health health;
    public AudioManager audioManager;
    public BossStates bossState { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
    }
    
    // Update is called once per frame
    void Update()
    {
        StateManagement();
        int feetStepping = 0;
        int feetMoved = 0;
        foreach (StickyFeet foot in feet)
        {
            if (foot.isAnimating) feetStepping++;
            if (foot.footHasMoved) feetMoved++;
        }
        if (feetMoved >= 4)
        {
            foreach (StickyFeet foot in feet)
            {
                foot.footHasMoved = false;
            }
        }
        foreach (StickyFeet foot in feet)
        {
            if (feetStepping < 2)
            {
                if (foot.TryToStep()) feetStepping++;
            }
        }

        agent.speed = 20 - health.health * 4;
    }
    private void StateManagement()
    {
        if (health.health <= 0) SwitchState(new States.Dead());
        else if (state == null) SwitchState(new States.Walk());

        if (state != null)
        {
            if (state != null)
            {
                SwitchState(state.Update());
            }
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
