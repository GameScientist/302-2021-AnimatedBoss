using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
/// <summary>
/// Controls the boss's behavior.
/// </summary>
public class BossController : MonoBehaviour
{
    /// <summary>
    /// Helps other components identify the current state of the boss.
    /// </summary>
    public enum BossStates
    {
        Idle,
        Walk,
        Attack,
        Dead
    }
    /// <summary>
    /// The many different types of behavior the boss has.
    /// </summary>
    public static class States
    {
        /// <summary>
        /// One of the behaviors of the boss.
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
        /// <summary>
        /// The boss has lost track of the player and is standing around trying to find it!
        /// </summary>
        public class Idle : State
        {
            /// <summary>
            /// The amount of time this state has been active.
            /// </summary>
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
        /// <summary>
        /// The boss walks towards the player.
        /// </summary>
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
        /// <summary>
        /// The boss stomps on the player.
        /// </summary>
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
                foreach(Chips chip in boss.chips)
                {
                    ParticleSystem particles = chip.GetComponent<ParticleSystem>();
                    particles.Play();
                }
                base.OnEnd();
            }
        }
        /// <summary>
        /// The boss gives up and retreats back into the sea.
        /// </summary>
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
                boss.tip.gameObject.SetActive(true);
                Text text = boss.tip.GetComponentInChildren<Text>();
                text.text = "You won! You are now a land dweller! Press the quit button to quit the game.";
                base.OnStart(boss);
            }
            public override void OnEnd()
            {
                base.OnEnd();
            }
        }
    }
    /// <summary>
    /// Tracks how much health the boss has left.
    /// </summary>
    private Health health;
    /// <summary>
    /// Sets the boss's current destination.
    /// </summary>
    private NavMeshAgent agent;
    /// <summary>
    /// The particle effects that play when the boss stomps.
    /// </summary>
    public List<Chips> chips = new List<Chips>();
    /// <summary>
    /// The player the boss is trying to attack.
    /// </summary>
    public Transform hero;
    /// <summary>
    /// The area inside of the sea the boss travels to on defeat.
    /// </summary>
    public Transform exit;
    /// <summary>
    /// The boss's current state.
    /// </summary>
    public States.State state;
    /// <summary>
    /// The feet being controlled by the boss.
    /// </summary>
    public List<StickyFeet> feet = new List<StickyFeet>();
    /// <summary>
    /// Controls all of the sounds the boss makes.
    /// </summary>
    public AudioManager audioManager;
    /// <summary>
    /// Removes this once the player gets its firt hit in.
    /// </summary>
    public Transform tip;
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
        if (health.health == 3) tip.gameObject.SetActive(false);
        StateManagement();
        MoveFeet(0, 0);
    }
    /// <summary>
    /// Determines if the SwitchState method needs to be executed.
    /// </summary>
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
    /// <summary>
    /// Sends signals to each foot animator if a foot needs to be moved.
    /// </summary>
    /// <param name="feetStepping"></param>
    /// <param name="feetMoved"></param>
    private void MoveFeet(int feetStepping, int feetMoved)
    {
        foreach (StickyFeet foot in feet)
        {
            if (foot.IsAnimating) feetStepping++;
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
    }
}
