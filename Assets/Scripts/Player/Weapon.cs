using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class Weapon : MonoBehaviour
{
    public float damage;

    public PlayerCombat playerCombat;
    public VisualEffect bloodSplatter;



    private void OnTriggerEnter(Collider other)
    {
        // Check if player attacking
        if(playerCombat.isAttacking)
        {
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
    }

}
