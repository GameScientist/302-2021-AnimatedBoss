using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroController : MonoBehaviour
{
    public enum States
    {
        Idle,
        Move,
        Jump,
        Fall,
        Attack,
        Dead
    }

    private CharacterController pawn;
    public float moveSpeed = 5;
    public float stepSpeed = 5;
    public Transform cam;

    public Vector3 walkScale = Vector3.one;
    public States state { get; private set; }
    public Vector3 moveDir { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        state = States.Idle;
        pawn = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        moveDir = new Vector3(h, 0f, v).normalized;

        if (moveDir.sqrMagnitude > 1) moveDir.Normalize();

        if(moveDir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);

            moveDir = transform.rotation * Vector3.forward;
            pawn.SimpleMove(moveDir * moveSpeed);
        }

        
        state = (moveDir.magnitude > .1f) ? States.Move : States.Idle;
    }
}
