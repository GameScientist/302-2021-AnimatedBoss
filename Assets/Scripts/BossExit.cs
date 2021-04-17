using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossExit : MonoBehaviour
{
    private bool sinking;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (sinking) transform.position -= transform.up * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        sinking = true;
    }
}
