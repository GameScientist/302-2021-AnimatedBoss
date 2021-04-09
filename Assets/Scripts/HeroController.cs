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
    public float moveSpeed = 5;
    public float stepSpeed = 5;
    public float jumpImpulse = 5;
    public Transform cam;

    private bool isJumpHeld = false;

    private bool spinning = false;
    public float spinTime = 0;

    private float verticalVelocity = 0;

    public Vector3 walkScale = Vector3.one;

    private float timeLeftGrounded = 0;

    private Rigidbody[] limbs;

    private bool gibbed;

    private float startMoveSpeed;
    private float startStepSpeed;
    private float startJumpImpulse;
    private float momentum = 0;

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
        limbs = GetComponentsInChildren<Rigidbody>();
        startMoveSpeed = moveSpeed;
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
            moveSpeed = Mathf.Lerp(startMoveSpeed, startMoveSpeed * 2, momentum);

            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            moveDir = new Vector3(h, 0f, v).normalized;

            if (moveDir.sqrMagnitude > 1) moveDir.Normalize();

            if (Input.GetButtonDown("Jump")) isJumpHeld = true;
            else isJumpHeld = false;

            verticalVelocity += jumpImpulse * 2 * Time.deltaTime;
            Vector3 moveDelta;
            if (moveDir.magnitude >= 0.1f)
            {
                float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
                transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

                moveDir = transform.rotation * Vector3.forward;
                moveDelta = moveDir * moveSpeed + verticalVelocity * Vector3.down;
            }
            else moveDelta = verticalVelocity * Vector3.down;
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
            if (Input.GetButtonDown("Fire1") && !spinning) spinning = true;
            if (spinning)
            {
                if (spinTime >= 0.6f)
                {
                    spinTime = 0f;
                    spinning = false;
                }
                else
                {
                    if (spinTime <= 0.2f)
                    {
                        state = States.Attack;
                        spinTime += Time.deltaTime;
                        if (!isGrounded) verticalVelocity = -jumpImpulse;
                        return;
                    }
                    else if(isGrounded) spinTime += Time.deltaTime;
                }
            }
            if (isGrounded)
            {
                if (Input.GetButton("Fire3") && moveDir.magnitude > .1f)
                {
                    momentum += Time.deltaTime;
                    if(momentum>=1)
                    {
                        stepSpeed = startStepSpeed * 2;
                        state = States.Run;
                    }
                    else
                    {
                        stepSpeed = startStepSpeed * 1.5f;
                        state = States.Jog;
                    }
                }
                else
                {
                    state = (moveDir.magnitude > .1f) ? States.Walk : States.Idle;
                    momentum -= Time.deltaTime;
                    stepSpeed = startStepSpeed;
                }
            }
            else state = (verticalVelocity > 0) ? States.Fall : States.Jump;
            momentum = Mathf.Clamp(momentum, 0f, 1f);
            print(isGrounded);
        }
    }
}
