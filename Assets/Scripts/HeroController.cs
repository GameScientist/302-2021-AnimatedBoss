using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private float verticalVelocity = 0;

    public Vector3 walkScale = Vector3.one;

    private float timeLeftGrounded = 0;

    private Rigidbody[] limbs;

    private bool gibbed;

    private float maxSpeed = 20;
    private float startStepSpeed;
    private float startJumpImpulse;
    private float momentum = 0;
    private bool running;
    private bool vulnerable = true;
    private float postHitInvulnerabilityTime = 0;
    private Health health;
    public Area currentArea = null;

    public bool isGrounded
    {
        get
        { // return true is pawn is on ground OR "coyote time"
            return pawn.isGrounded || timeLeftGrounded > 0;
        }
    }
    public States state { get; private set; }
    public Vector3 moveDir { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        state = States.Idle;
        pawn = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        limbs = GetComponentsInChildren<Rigidbody>();
        startStepSpeed = stepSpeed;
        startJumpImpulse = jumpImpulse;
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
                gibbed = true;
            }
        }
        else
        {
            MovePlayer();
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
            momentum = Mathf.Clamp(momentum, 0f, 1f);
            state = SetState();
            //print(state);
        }
    }
    private void MovePlayer()
    {

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDir = new Vector3(h, 0f, v).normalized;

        if (moveDir.sqrMagnitude > 1) moveDir.Normalize();

        if (Input.GetButtonDown("Jump")) isJumpHeld = true;
        else isJumpHeld = false;
        if (Input.GetButtonDown("Fire1") && !spinning) spinning = true;

        verticalVelocity += jumpImpulse * 2 * Time.deltaTime;
        if (spinning)
        {
            if (spinTime >= 1)
            {
                spinTime = 0f;
                spinning = false;
            }
            else
            {
                if (spinTime <= 0.25f)
                {
                    SpinAttack hitBox = GetComponentInChildren<SpinAttack>(true);
                    hitBox.gameObject.SetActive(true);
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

            moveDir = transform.rotation * Vector3.forward;

            momentum += Time.deltaTime / 2;

            moveDelta = moveDir * moveSpeed + verticalVelocity * Vector3.down;

        }
        else
        {
            momentum -= Time.deltaTime / 2;
            moveDelta = transform.forward * moveSpeed + verticalVelocity * Vector3.down;

        }
        moveSpeed = Mathf.Lerp(0, maxSpeed, momentum);
        pawn.Move(moveDelta * Time.deltaTime);
        if (isGrounded)
        {
            verticalVelocity = 0; // on ground, zero-out gravity below.
            if (pawn.isGrounded) timeLeftGrounded = 0.16f;
            else timeLeftGrounded -= Time.deltaTime;

            if (isJumpHeld)
            {
                verticalVelocity = -jumpImpulse * (momentum + 1);
                timeLeftGrounded = 0; // not on ground (for animation's sake)
            }
        }
    }
    private States SetState()
    {
        if (spinning && spinTime <= 0.2f) return States.Attack;
        else if (isGrounded)
        {
            if (running) return (momentum >= 1) ? States.Run : States.Jog;
            else return (momentum > 0) ? States.Walk : States.Idle;
        }
        else return (verticalVelocity > 0) ? States.Fall : States.Jump;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Damage")) health.Damage();
    }
}
