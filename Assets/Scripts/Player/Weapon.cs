using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class Weapon : MonoBehaviour
{
    public float damage;

    CapsuleCollider triggerBox;
    public PlayerCombat playerCombat;
    public VisualEffect bloodSplatter;

    public void Start()
    {
        triggerBox = GetComponent<CapsuleCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Make generic instead of raider only
        var enemy = other.gameObject.GetComponent<EnemyStats>();
        if (other.tag == "Enemy")
        {
            // Enemy play hit animation
            var vfx = Instantiate(bloodSplatter, transform.position, Quaternion.identity);
            vfx.Play();
            Destroy(vfx, 0.5f);
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
