using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    /// <summary>
    /// How much time is left until post hit invulnerability has ended.
    /// </summary>
    private float postHitTime = 7.5f;
    /// <summary>
    /// When the character become invulnerable and its sprite starts blinking repeatedly.
    /// </summary>
    public bool postHit;
    /// <summary>
    /// For the boss, when it is able to be attacked. The player is always vulnerable.
    /// </summary>
    public bool vulnerable;
    /// <summary>
    /// How many hits this character can take before dying.
    /// </summary>
    public int health;

    /// <summary>
    /// Gets the particle system component and the sprite component while also setting the 
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        //blood = GetComponentInChildren<ParticleSystem>();
    }

    /// <summary>
    /// Keeps the health bar updating while checking if the character is hit or dead.
    /// </summary>
    // Update is called once per frame
    void Update()
    {
        if (postHit && health > 0)
        {
            PostHitInvulnerability();
        }
    }

    /// <summary>
    /// Makes the player invulnerable for a limited amount of time, indicated by a blinking sprite.
    /// </summary>
    private void PostHitInvulnerability()
    {
        postHitTime -= Time.deltaTime;
        if (postHitTime <= 0)
        {
            postHitTime = 7.5f;
            postHit = false;
        }
    }
    /// <summary>
    /// Reduces the player's health, sets post-hit invulnerability, and plays the "blood" prefab.
    /// </summary>
    public void Damage()
    {
        if (!postHit && vulnerable)
        {
            health--;
            postHit = true;
        }
    }
}