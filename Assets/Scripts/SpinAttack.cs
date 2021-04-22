using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttack : MonoBehaviour
{
    public AudioManager audioManager;
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
            audioManager.Play("Sinking");
            audioManager.Play("Hit");
            audioManager.Play("Boss Damage");
        }
    }
}
