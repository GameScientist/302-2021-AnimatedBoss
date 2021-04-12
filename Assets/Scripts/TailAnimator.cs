using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TailAnimator : MonoBehaviour
{
    private Vector3 startingPos;
    // Start is called before the first frame update
    void Start()
    {
        startingPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 finalPos = startingPos;

        // lateral movement: (z + x)
        finalPos.x = Mathf.Sin(Time.time * 5) * 50;

        transform.localPosition = AnimMath.Lerp(transform.localPosition, finalPos, 0.5f);
    }
}
