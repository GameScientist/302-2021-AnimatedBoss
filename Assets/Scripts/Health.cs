using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float postHitTimeLimit;
    /// <summary>
    /// How much time is left until post hit invulnerability has ended.
    /// </summary>
    private float postHitTimeCurrent;
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
    /// <summary>
    /// UI objects the represents each character's health.
    /// </summary>
    public RawImage[] eyes = new RawImage[4];
    /// <summary>
    /// A UI object that represents damaged health.
    /// </summary>
    public Texture closed;
    /// <summary>
    /// The panel that contains a character's health.
    /// </summary>
    public RawImage panel;
    /// <summary>
    /// The sound effects that are played upon taking damage.
    /// </summary>
    public AudioManager audioManager;

    /// <summary>
    /// Gets the particle system component and the sprite component while also setting the 
    /// </summary>
    // Start is called before the first frame update
    void Start()
    {
        postHitTimeCurrent = postHitTimeLimit;
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
        postHitTimeCurrent -= Time.deltaTime;
        panel.color = Color.Lerp(new Color(1, 1, 1, 0.2f), Color.red, Mathf.Sin(Time.time * 60));
        if (postHitTimeCurrent <= 0)
        {
            postHitTimeCurrent = postHitTimeLimit;
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
            if (player != null)
            {
                audioManager.Play("Player Damage");
            }
        }
    }
}