using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public bool canAttack;
    public bool canBlock;
    public bool isAttacking;
    public bool isBlocking;

    public float blockCooldown = 1f;

    [SerializeField] private Weapon weapon;

    // Combo
    Animator anim;
    public List<AttackSO> combo;
    float lastClickedTime;
    float lastComboEnd;
    int comboCounter;

    void Start()
    {
        canAttack = true;
        canBlock = true;
        isAttacking = false;
        isBlocking = false;
        anim = GetComponent<Animator>();
        comboCounter = 0;
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
        if(Input.GetButtonDown("Fire2"))
        {
            Block();
        }
        ExitAttack();

    }

    public void Attack()
    {
        if(canAttack)
        {
            if(Time.time - lastComboEnd > 0.01f && comboCounter < combo.Count)
            {


                CancelInvoke("EndCombo");

                if(Time.time - lastClickedTime >= 0.5f)
                {
                    isAttacking = true;
                    anim.runtimeAnimatorController = combo[comboCounter].animatorOV;
                    anim.Play("Attack", 0, 0);
                    weapon.damage = combo[comboCounter].damage;
                    // Play sound from SO
                    comboCounter++;
                    lastClickedTime = Time.time;

                    if(comboCounter > combo.Count)
                    {
                        comboCounter = 0;
                    }
                }

                
            }

        }
        
        
    }

    void ExitAttack()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && anim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo",0.8f);
            // 1 second buffer to start next input
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        isAttacking = false;
        lastComboEnd = Time.time;
    }

    public void Block()
    {
        if(canBlock)
        {
            canBlock = false;
            
            isBlocking = true;
            anim.SetTrigger("Blocking");
            Invoke("EndCombo",0f);
            StartCoroutine(ResetBlockCooldown());
        }


    }

    IEnumerator ResetBlockCooldown()
    {
        yield return new WaitForSeconds(blockCooldown);
        canBlock = true;
    }


    public void ResetBlockBool()
    {
        isBlocking = false;
    }
}
