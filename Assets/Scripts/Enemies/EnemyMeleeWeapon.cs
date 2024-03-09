using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMeleeWeapon : MonoBehaviour
{

    public float damage;



    private void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Player")
        {

            if(other.GetComponent<PlayerCombat>().isBlocking)
            { 
                // Play Attack VFX 
                // Play Attack SFX 
                Debug.Log("Attack Blocked!");
            }
            else if(other.TryGetComponent<Player>(out Player P))
            { 
                P.TakeDamage(damage);   
            }
        }
    }
}
