using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;

    CapsuleCollider triggerBox;
    public PlayerCombat playerCombat;

    public void Start()
    {
        triggerBox = GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make generic instead of raider only
        var enemy = other.gameObject.GetComponent<Raider>();
        if (other.tag == "Enemy" && playerCombat.isAttacking)
        {
            // Hit Particle effects
            // Enemy play hit animation
            enemy.TakeDamage(damage);
        }
    }

    public void EnableTriggerBox()
    {
        triggerBox.enabled = true;
    }

    public void DisableTriggerBox()
    {
        triggerBox.enabled = false;
    }

}
