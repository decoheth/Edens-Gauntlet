using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Plant_SporeSpreader : MonoBehaviour
{
    [Header("Attack")]
    public float attackDamage = 5f;
    public bool canAttack;
    public bool attackActive;
    public float attackDuration = 6f;
    public float attackCooldown = 20f;



    void Start()
    {
        canAttack = true;
        attackActive = false;
    }


    void OnTriggerEnter (Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if(canAttack == true)
            {
                attackActive = true;
                // Play atack animation
                // Create spore cloud effect
                StartCoroutine(AttackDuration());
            }
                
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        if(attackActive == true)
        {
            if(other.transform.TryGetComponent<EnemyStats>(out EnemyStats T))
            { 
                T.TakeDamage(attackDamage);
            }
        }
    }




    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
    IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(attackDuration);
        attackActive = false;
        StartCoroutine(ResetAttackCooldown());
    }

}
