using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
/// <summary>
/// Controls the state and movement of the player character.
/// </summary>
public class HeroController : MonoBehaviour
{
    /// <summary>
    /// Helps other components identify the current state of the boss.
    /// </summary>
    public enum States
    {
        Idle,
        Walk,
        Jog,
        Run,
        Jump,
        Fall,
        Attack,
        Dead
    }
    /// <summary>
    /// If the player has gibbed from death or not.
    /// </summary>
    private bool gameOver;
    /// <summary>
    /// If the jump button was pressed to jump.
    /// </summary>
    private bool isJumpPressed = false;
    /// <summary>
    /// If the "speed up" button was pressed to speed the character up.
    /// </summary>
    private bool isSpeedUpPressed;
    /// <summary>
    /// If the player is currently playing its spinning animation.
    /// </summary>
    private bool spinning = false;
    /// <summary>
    /// The degree to which gravity is reduced when the player jumps.
    /// </summary>
    private readonly float jumpImpulse = 10;
    /// <summary>
    /// The amount of speed added onto the player while sprinting.
    /// </summary>
    private float momentum = 0;
    /// <summary>
    /// How much distance the player pawn travels.
    /// </summary>
    private float moveSpeed = 0;
    /// <summary>
    /// The default speed of how quickly the player character moves its feet and arms.
    /// </summary>
    private float startStepSpeed;
    /// <summary>
    /// The degree to which the player is being pulled down into the earth.
    /// </summary>
    private float gravity = 0.1f;

    /// <summary>
    /// The component used to move the character and detect if it is grounded.
    /// </summary>
    private CharacterController pawn;
    /// <summary>
    /// All of the gibs produced by the player character on death.
    /// </summary>
    private Rigidbody[] limbs;
    /// <summary>
    /// Tracks how much health the player has left.
    /// </summary>
    private Health health;
    /// <summary>
    /// The speed at which the player moves its feet.
    /// </summary>
    public float stepSpeed = 10;
    /// <summary>
    /// The amount of time spent within the spin animation.
    /// </summary>
    public float spinTime = 0;

    /// <summary>
    /// The last raft the player has visited.
    /// </summary>
    public Area currentArea = null;
    /// <summary>
    /// Controls audio made by the player.
    /// </summary>
    public AudioManager audioManager;
    /// <summary>
    /// The camera that follows the player.
    /// </summary>
    public Transform cam;
    /// <summary>
    /// The trails coming from the player when spinning.
    /// </summary>
    public TrailRenderer[] armTrails;
    /// <summary>
    /// The trails coming from the player when speeding up.
    /// </summary>
    public TrailRenderer[] legTrails;
    /// <summary>
    /// Is instantiated when the player hits the water.
    /// </summary>
    public Transform splash;
    /// <summary>
    /// Plays a message when the player recieves a game over.
    /// </summary>
    public Transform tip;

    /// <summary>
    /// How far the player character moves its limbs when walking.
    /// </summary>
    public Vector3 walkScale = Vector3.one;

    /// <summary>
    /// Checks if the player is currently on the ground.
    /// </summary>
    public bool IsGrounded
    {
        get
        { // return true is pawn is on ground OR "coyote time"
            return pawn.isGrounded;// || timeLeftGrounded > 0;
        }
    }
    /// <summary>
    /// The player's current state.
    /// </summary>
    public States State { get; private set; }
    /// <summary>
    /// The direction the player moves in when moving.
    /// </summary>
    public Vector3 MoveDir { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        audioManager.Play("Ambience");
        audioManager.Play("Music");
        Cursor.lockState = CursorLockMode.Locked;
        startStepSpeed = stepSpeed;
        pawn = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        limbs = GetComponentsInChildren<Rigidbody>();
        State = States.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (State == States.Dead)Death();
        else SetPosition();
        print(gravity);
    }

    /// <summary>
    /// Executes the GameOver animation if not already done and waits for input from the player before resetting the scene.
    /// </summary>
    private void Death()
    {
        if (gameOver)
        {
            if (Input.GetButtonDown("Fire1")) SceneManager.LoadScene("AnimatedBoss");
        }
        else GameOver();
    }

    /// <summary>
    /// Gibs the player and gives a message.
    /// </summary>
    private void GameOver()
    {
        Gib();
        GameOverMessage();
        gameOver = true;
    }

    /// <summary>
    /// Causes each of the player character's limbs to fall apart.
    /// </summary>
    private void Gib()
    {
        foreach (Rigidbody limb in limbs)
        {
            limb.useGravity = true;
            BoxCollider box = limb.GetComponent<BoxCollider>();
            if (box != null) box.enabled = true;
            else
            {
                CapsuleCollider capsule = limb.GetComponent<CapsuleCollider>();
                if (capsule != null) capsule.enabled = true;
            }
            limb.transform.parent = null;
        }
    }
    
    /// <summary>
    /// The message that is shown during game over.
    /// </summary>
    private void GameOverMessage()
    {
        tip.gameObject.SetActive(true);
        Text text = tip.GetComponentInChildren<Text>();
        text.text = "Game over! Press the attack button to restart.";
    }

    /// <summary>
    /// Calculates the position of the player and sets its current state.
    /// </summary>
    private void SetPosition()
    {
        if (transform.position.y <= 0) Respawn();
        else MovePlayer();
        State = SetState();
    }
    
    /// <summary>
    /// Teleports the player back to the start point.
    /// </summary>
    public void Respawn()
    {
        Instantiate(splash, transform.position, transform.rotation);
        momentum = 0;
        gravity = 0;
        pawn.enabled = false;
        transform.position = new Vector3(0, 8.5f, 0);
        pawn.enabled = true;
    }

    /// <summary>
    /// Sets the player's position based on user input.
    /// </summary>
    private void MovePlayer()
    {
        gravity += jumpImpulse * Time.deltaTime * 2;
        JumpInput();
        SpinInput();
        MovementInput();
        MovePawn();
    }

    /// <summary>
    /// Detects jump input to adjust its variable and checks if the player is grounded.
    /// </summary>
    private void JumpInput()
    {
        if (Input.GetButtonDown("Jump")) isJumpPressed = true;
        else isJumpPressed = false;
        if (IsGrounded)GroundPlayer();
    }

    /// <summary>
    /// Keeps the player gravitating towards the ground unless the jump button is pressed, in which case the player will shoot into the air.
    /// </summary>
    private void GroundPlayer()
    {
        gravity = 0.1f; // on ground, zero-out gravity below.

        if (isJumpPressed)
        {
            gravity = -jumpImpulse * (momentum + 1);
            audioManager.Play("Jump");
        }
    }
    
    /// <summary>
    /// Checks for if the spin attack button is pressed and updates the spin if it is.
    /// </summary>
    private void SpinInput()
    {
        if (Input.GetButtonDown("Fire1") && !spinning) spinning = true;
        if (spinning) SpinUpdate();
    }

    /// <summary>
    /// Keeps the spin activated or resets based on how long it has been active for.
    /// </summary>
    private void SpinUpdate()
    {
        if (spinTime >= 1) SpinReset();
        else SpinActivate();
    }
    
    /// <summary>
    /// Stops the player from spinning.
    /// </summary>
    private void SpinReset()
    {
        spinTime = 0f;
        spinning = false;
        foreach (TrailRenderer trail in armTrails) trail.emitting = false;
    }
    
    /// <summary>
    /// Increases the amount of time the spin has been activated while checking if an attack or delay is necessary.
    /// </summary>
    private void SpinActivate()
    {
        if (spinTime <= 0.25f) SpinAttack();
        else SpinDelay();
        spinTime += Time.deltaTime;
    }
    
    /// <summary>
    /// Damages the boss on impact on levitates the player slightly if in the air.
    /// </summary>
    private void SpinAttack()
    {
        SpinAttack hitBox = GetComponentInChildren<SpinAttack>(true);
        hitBox.gameObject.SetActive(true);
        foreach (TrailRenderer trail in armTrails) trail.emitting = true;
        if (!IsGrounded) gravity = -jumpImpulse / 2;
    }
    
    /// <summary>
    /// Prevents the player from spinning again until the spintime runs out.
    /// </summary>
    private void SpinDelay()
    {
        SpinAttack hitBox = GetComponentInChildren<SpinAttack>();
        if (hitBox != null) hitBox.gameObject.SetActive(false);
    }
    
    /// <summary>
    /// Sets the movement direction based on player input.
    /// </summary>
    private void MovementInput()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        MoveDir = new Vector3(h, 0f, v);
    }
    
    /// <summary>
    /// Moves the player in the desired direction using a calculated speed.
    /// </summary>
    private void MovePawn()
    {
        if (MoveDir.sqrMagnitude > 1) MoveDir.Normalize();
        if (MoveDir.magnitude >= 0.1f) Accelerate();
        else Deaccelerate();
        if (IsGrounded) SpeedUpInput();
        moveSpeed = Mathf.Lerp(0, 20, momentum);
        pawn.Move((transform.forward * moveSpeed + gravity * Vector3.down) * Time.deltaTime);
    }

    /// <summary>
    /// The player character becomes faster the more it keeps moving.
    /// </summary>
    private void Accelerate()
    {
        float targetAngle = Mathf.Atan2(MoveDir.x, MoveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

        momentum += Time.deltaTime / 2;

        if (momentum >= 1)
        {
            momentum = 1;
            foreach (TrailRenderer trail in legTrails) trail.emitting = true;
        }
        else foreach (TrailRenderer trail in legTrails) trail.emitting = false;
    }

    /// <summary>
    /// The player character slows down until it stops.
    /// </summary>
    private void Deaccelerate()
    {
        momentum -= Time.deltaTime / 2;
        if (momentum < 0) momentum = 0;
    }

    /// <summary>
    /// The player's maximum speed is set based on if the "speed up" button has been pressed.
    /// </summary>
    private void SpeedUpInput()
    {
        if (Input.GetButton("Fire3") && MoveDir.magnitude > .1f) CalculateMoveState(1f, true);
        else CalculateMoveState(0.5f, false);
    }

    /// <summary>
    /// Sets the player's maximum move speed.
    /// </summary>
    /// <param name="maxMomentum"></param>
    /// <param name="speedUpButton"></param>
    private void CalculateMoveState(float maxMomentum, bool speedUpButton)
    {
        momentum = Mathf.Clamp(momentum, 0, maxMomentum);
        isSpeedUpPressed = speedUpButton;
        if (speedUpButton) CalculateStepSpeed();
        else stepSpeed = startStepSpeed;
    }

    /// <summary>
    /// Adjusts the player's step speed based on how high its momentum is.
    /// </summary>
    private void CalculateStepSpeed()
    {
        if (momentum >= 1) stepSpeed = startStepSpeed * 2;
        else stepSpeed = startStepSpeed * 1.5f;
    }

    /// <summary>
    /// Sets the current state of the player based on several factors.
    /// </summary>
    /// <returns></returns>
    private States SetState()
    {
        if (health.health <= 0) return States.Dead;
        else if (spinning && spinTime <= 0.2f) return States.Attack;
        else if (IsGrounded)
        {
            if (isSpeedUpPressed) return (momentum >= 1) ? States.Run : States.Jog;
            else return (momentum > 0) ? States.Walk : States.Idle;
        }
        else return (gravity > 0) ? States.Fall : States.Jump;
    }

    /// <summary>
    /// Reduces health on collision with the boss.
    /// </summary>
    /// <param name="hit"></param>
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Damage") || hit.gameObject.CompareTag("Eye")) health.Damage();
    }
}
