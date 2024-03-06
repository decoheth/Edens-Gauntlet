using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public bool canAttack = true;

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
        anim = GetComponent<Animator>();
        comboCounter = 0;
    }

    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Attack();
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
        lastComboEnd = Time.time;
    }
    
}
