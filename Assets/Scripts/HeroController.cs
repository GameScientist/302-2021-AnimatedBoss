using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroController : MonoBehaviour
{
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

    private CharacterController pawn;
    private float acceleration = 0;
    public float moveSpeed = 10;
    public float stepSpeed = 10;
    public float jumpImpulse = 20;
    public Transform cam;

    private bool isJumpHeld = false;

    private bool spinning = false;
    public float spinTime = 0;

    public float verticalVelocity = 0;

    public Vector3 walkScale = Vector3.one;

    private float timeLeftGrounded = 0;

    private Rigidbody[] limbs;
    public TrailRenderer[] armTrails;
    public TrailRenderer[] legTrails;

    private bool gibbed;

    private float maxSpeed = 20;
    private float startStepSpeed;
    public float momentum = 0;
    private bool running;
    private Health health;
    public Area currentArea = null;
    public AudioManager audioManager;
    public Transform tip;

    public bool isGrounded
    {
        get
        { // return true is pawn is on ground OR "coyote time"
            return pawn.isGrounded;// || timeLeftGrounded > 0;
        }
    }
    public States state { get; private set; }
    public Vector3 moveDir { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        state = States.Idle;
        pawn = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        limbs = GetComponentsInChildren<Rigidbody>();
        startStepSpeed = stepSpeed;
        audioManager.Play("Ambience");
        audioManager.Play("Music");
    }

    // Update is called once per frame
    void Update()
    {
        if (state == States.Dead)
        {
            if (gibbed) return;
            else
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
                tip.gameObject.SetActive(true);
                Text text = tip.GetComponentInChildren<Text>();
                text.text = "Game over! Please restart.";
                gibbed = true;
            }
        }
        else
        {
            MovePlayer();
            momentum = Mathf.Clamp(momentum, 0f, 1f);
            if (momentum >= 1) audioManager.Play("Max Speed");
            else audioManager.Stop("Max Speed");
            state = SetState();
            //print(state);
        }
    }
    private void MovePlayer()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDir = new Vector3(h, 0f, v);//.normalized;

        if (moveDir.sqrMagnitude > 1)
        {
            moveDir.Normalize();
            //audioManager.Play("Running");
        }
        else audioManager.Stop("Running");

        if (Input.GetButtonDown("Jump"))
        {
            isJumpHeld = true;
        }
        else isJumpHeld = false;
        if (Input.GetButtonDown("Fire1") && !spinning)
        {
            spinning = true;
            //audioManager.Play("Spin");
        }

        verticalVelocity += 20 * Time.deltaTime;
        if (isGrounded)
        {
            verticalVelocity = 0.1f; // on ground, zero-out gravity below.
            if (pawn.isGrounded) timeLeftGrounded = 0.16f;
            else timeLeftGrounded -= Time.deltaTime;

            if (isJumpHeld)
            {
                verticalVelocity = -jumpImpulse * (momentum + 1);
                timeLeftGrounded = 0; // not on ground (for animation's sake)
                audioManager.Play("Jump");
            }
        }
        if (spinning)
        {
            if (spinTime >= 1)
            {
                spinTime = 0f;
                spinning = false;
                foreach (TrailRenderer trail in armTrails) trail.emitting = false;
            }
            else
            {
                if (spinTime <= 0.25f)
                {
                    SpinAttack hitBox = GetComponentInChildren<SpinAttack>(true);
                    hitBox.gameObject.SetActive(true);
                    foreach (TrailRenderer trail in armTrails) trail.emitting = true;
                    if (!isGrounded) verticalVelocity = -jumpImpulse/2;
                    spinTime += Time.deltaTime;
                }
                else
                {
                    if (isGrounded) spinTime += Time.deltaTime;
                    SpinAttack hitBox = GetComponentInChildren<SpinAttack>();
                    if(hitBox != null)hitBox.gameObject.SetActive(false);
                }
                spinTime += Time.deltaTime;

            }
        }
        Vector3 moveDelta;
        if (moveDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            //moveDir = transform.rotation * Vector3.forward;

            momentum += Time.deltaTime / 2;

            if (momentum >= 1) foreach (TrailRenderer trail in legTrails) trail.emitting = true;
            else foreach (TrailRenderer trail in legTrails) trail.emitting = false;
        }
        else
        {
            momentum -= Time.deltaTime / 2;
            if (momentum < 0) momentum = 0;
        }
        moveDelta = transform.forward * moveSpeed + verticalVelocity * Vector3.down;
        if (isGrounded)
        {
            if (Input.GetButton("Fire3") && moveDir.magnitude > .1f)
            {
                running = true;
                momentum = Mathf.Clamp01(momentum);
                if (momentum >= 1)
                {
                    stepSpeed = startStepSpeed * 2;
                }
                else
                {
                    stepSpeed = startStepSpeed * 1.5f;
                }
            }
            else
            {
                momentum = Mathf.Clamp(momentum, 0, 0.5f);
                stepSpeed = startStepSpeed;
                running = false;
            }
        }
        moveSpeed = Mathf.Lerp(0, maxSpeed, momentum);
        pawn.Move(moveDelta * Time.deltaTime);
        if (transform.position.y <= 0)
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        momentum = 0;
        verticalVelocity = 0;
        pawn.enabled = false;
        transform.position = new Vector3(0, 8.5f, 0);
        pawn.enabled = true;
    }

    private States SetState()
    {
        if (health.health <= 0) return States.Dead;
        else if (spinning && spinTime <= 0.2f) return States.Attack;
        else if (isGrounded)
        {
            if (running) return (momentum >= 1) ? States.Run : States.Jog;
            else return (momentum > 0) ? States.Walk : States.Idle;
        }
        else return (verticalVelocity > 0) ? States.Fall : States.Jump;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Damage") || hit.gameObject.CompareTag("Eye")) health.Damage();
    }
}
