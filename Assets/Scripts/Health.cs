using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public int health = 4;

    public RawImage[] eyes = new RawImage[4];

    public Texture closed;

    public RawImage panel;

    public AudioManager audioManager;

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
        int eyeIndex = 0;
        foreach (RawImage eye in eyes)
        {
            eyeIndex++;
            if (eyeIndex > health) eye.texture = closed;
        }
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
        panel.color = Color.Lerp(new Color(1, 1, 1, 0.2f), Color.red, Mathf.Sin(Time.time * 60));
        if (postHitTime <= 0)
        {
            postHitTime = 7.5f;
            postHit = false;
            panel.color = new Color(1, 1, 1, 0.2f);
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
            HeroController player = GetComponent<HeroController>();
            if (player != null) audioManager.Play("Player Damage");
        }
    }
}