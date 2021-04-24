using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// A trigger that spawns around the player to detect if the boss's eye was hit or not. Also plays several effects when the boss is hit.
/// </summary>
public class SpinAttack : MonoBehaviour
{
    /// <summary>
    /// Plays the sound effects for when the attack collides.
    /// </summary>
    public AudioManager audioManager;
    /// <summary>
    /// Plays when the boss has been hit.
    /// </summary>
    public ParticleSystem tears;
    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponentInParent<Health>();
        if (other.gameObject.CompareTag("Eye") && !health.postHit)
        {
            health.Damage();
            HeroController hero = GetComponentInParent<HeroController>();
            Area area = hero.currentArea.GetComponent<Area>();
            if (area == null) return;
            else area.sinking = true;
            hero.currentArea = null;
            tears.Play();
        }
    }
}
