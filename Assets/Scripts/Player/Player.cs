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



    [Header("Camera")]
    private Camera cam;
    public GameObject camGO;
    public bool isCursorHidden;

    public bool inMenu = false;
    
    [Header("Misc")]

    private PlayerCombat playerCombat;

    void Awake()
    {
        currentHealth = maxHealth;
        inMenu = false;
        isCursorHidden = true;

        cam = Camera.main;
        playerCombat = GetComponent<PlayerCombat>();



    }

    void Start()
    {
        // Load saved data

        wood = 0;
        stone = 0;
        metal = 0;
        seeds = 0;

    }

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
                playerCombat.Attack();
            }

            // Check if interact key pressed
            if(Input.GetKeyDown(KeyCode.E))
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                // If ray hits
                if (Physics.Raycast(ray, out hit, playerReach))
                {
                    if (hit.collider.GetComponent<IInteractable>() != null) 
                        hit.collider.GetComponent<IInteractable>().Interact(); 
                    else
                        return;
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


}
