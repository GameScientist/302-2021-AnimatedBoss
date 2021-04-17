using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAttack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Eye"))
        {
            Health health = other.GetComponentInParent<Health>();
            health.Damage();
            HeroController hero = GetComponentInParent<HeroController>();
            Area area = hero.currentArea.GetComponent<Area>();
            area.sinking = true;
            hero.currentArea = null;
        }
    }
}
