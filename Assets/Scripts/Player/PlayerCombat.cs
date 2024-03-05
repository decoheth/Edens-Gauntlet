using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public bool canAttack = true;
    public bool isAttacking = false;
    public float attackCooldown = .5f;
    [SerializeField] private GameObject shovel;


    void Start()
    {
        canAttack = true;
    }

    public void Attack()
    {
        if(canAttack)
        {
            ShovelAttack();
        }

    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    IEnumerator ResetAttackingBool()
    {
        yield return new WaitForSeconds(.6f);
        isAttacking = false;
    }

    public void ShovelAttack()
    {
        canAttack = false;
        isAttacking = true;
        Animator anim = shovel.GetComponent<Animator>();
        anim.SetTrigger("Attack");
        StartCoroutine(ResetAttackCooldown());
        StartCoroutine(ResetAttackingBool());
        
    }
}
