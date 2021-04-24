using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Plays a particle system when hitting the water to simulate a splash.
/// </summary>
public class Splash : MonoBehaviour
{
    private ParticleSystem bubbles;
    // Start is called before the first frame update
    void Start()
    {
        bubbles = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y <= 0 && !bubbles.isPlaying) bubbles.Play();
    }
}
