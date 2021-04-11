using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController2 : MonoBehaviour
{
    private Vector3 moveDir;
    private bool isJumpHeld;
    private float verticalVelocity;
    public float moveSpeed = 10;
    public float stepSpeed = 10;
    public float jumpImpulse = 20;
    private CharacterController pawn;
    private bool isGrounded;
    private float timeLeftGrounded = 0.16f;
    public Transform cam;
    private float planarVelocity;
    // Start is called before the first frame update
    void Start()
    {
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
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
                verticalVelocity = -jumpImpulse;
                timeLeftGrounded = 0; // not on ground (for animation's sake)
            }
        }
    }
}
