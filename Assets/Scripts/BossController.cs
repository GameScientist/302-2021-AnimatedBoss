using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : MonoBehaviour
{
    public enum States
    {
        Idle,
        Walk,
        Attack,
        Dead
    }
    public States state { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        state = States.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
