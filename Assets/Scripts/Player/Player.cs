using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    



    [Header("Stats")]
    public float maxHealth = 100f;
    public float playerReach = 1.5f;
    public float currentHealth;

    [Header("Inventory")]
    public int wood;
    public int stone;
    public int metal;
    public int seeds;

    [Header("Combat")]
    public bool canAttack = true;
    public float attackDamage = 5f;
    public float attackCooldown = .5f;
    public float attackDistance = 3f;

    public LayerMask attackLayer;
    public Transform hand;


    [Header("Camera")]
    private Camera cam;
    public GameObject camGO;
    public bool isCursorHidden;

    public bool inMenu = false;
    
    [Header("Misc")]
    private GameObject interactable;

    void Awake()
    {
        currentHealth = maxHealth;

        cam = Camera.main;

        inMenu = false;
        isCursorHidden = true;

    }

    void Start()
    {
        // Load saved HP
        // Load saved Resources
        wood = 0;
        stone = 0;
        metal = 0;
        seeds = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if(inMenu == true && isCursorHidden == true)
        {
            // Show Cursor
            camGO.GetComponent<MouseLook>().cursorHidden = false;
            // Update bool
            isCursorHidden = false;
        }
        else if (inMenu == false && isCursorHidden == false)
        {
            // Hide Cursor
            camGO.GetComponent<MouseLook>().cursorHidden = true;
            // Update bool
            isCursorHidden = true;
        }


        // Check if not in any menu

        if(inMenu == false)
        {


            // Attack on left click
            if(Input.GetMouseButtonDown(0))
            {
                if (canAttack)
                {
                    Attack();
                }
            }


            // Check if interact key pressed
            if(Input.GetKeyDown(KeyCode.E))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // If ray hits
                if (Physics.Raycast(ray, out hit, playerReach))
                {
                    if (hit.collider.GetComponent<WaveController>() != null) 
                    {
                        hit.collider.GetComponent<WaveController>().Interact();
                    }
                    else if (hit.collider.GetComponent<Interactable>() != null)
                    {
                        hit.collider.GetComponent<Interactable>().Interact();
                    }
                    else
                    {
                        return;
                    }
                }
            }


        }
  

    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if(currentHealth <= 0)
        { Death(); }
    }

    void Death()
    {
        Debug.Log("GAME OVER, YOU DIED");
    }

    public void Attack()
    {
        canAttack = false;
        //Animator anim = Knife.GetComponent<Animator>();
        //anim.SetTrigger("Attack");
        AttackRaycast();
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    void AttackRaycast()
    {
        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit hit, attackDistance, attackLayer))
        { 
            if(hit.transform.TryGetComponent<Raider>(out Raider T))
            { T.TakeDamage(attackDamage); }
        } 
    }

}
