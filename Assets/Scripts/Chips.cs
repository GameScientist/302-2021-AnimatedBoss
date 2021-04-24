using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chips : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Health health = other.GetComponent<Health>();
            health.Damage();
        }
    }
}
