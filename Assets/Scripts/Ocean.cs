using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = AnimMath.Slide(transform.position, new Vector3(0, 0, Mathf.Sin(Time.time)) * 250, 0.9f);
    }
}