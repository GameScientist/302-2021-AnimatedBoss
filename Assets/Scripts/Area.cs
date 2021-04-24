using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Sinks a raft downward when a signal is recieved.
/// </summary>
public class Area : MonoBehaviour
{
    /// <summary>
    /// If the raft is currently sinking.
    /// </summary>
    public bool sinking;

    /// <summary>
    /// All of the particle effects that play while the raft is sinking.
    /// </summary>
    public List<ParticleSystem> bubbles = new List<ParticleSystem>();

    // Update is called once per frame
    void Update()
    {
        if (sinking && transform.position.y > 0) Sink();
    }

    private void Sink()
    {
        foreach (ParticleSystem bubble in bubbles)
        {
            if (!bubble.isPlaying) bubble.Play();
        }
        transform.position -= transform.up * 7.5f * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            HeroController hero = other.GetComponent<HeroController>();
            hero.currentArea = this;
        }
    }
}
