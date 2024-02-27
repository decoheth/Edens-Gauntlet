using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    



    [Header("Stats")]
    
    public float maxHealth = 100f;
    public float currentHealth;
    public float playerReach = 1.5f;


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
    
    [Header("Other")]

    private PlayerCombat playerCombat;
    private UIManager uIManager;
    private GameManager gameManager;
    private SaveManager saveManager;
    public GameObject toolHolder;

    void Awake()
    {
        inMenu = false;
        isCursorHidden = true;

        cam = Camera.main;
        gameManager = GameObject.Find("/Managers/Game Manager/").GetComponent<GameManager>();
        saveManager = GameObject.Find("/Managers/Save Manager/").GetComponent<SaveManager>();
        uIManager = GameObject.Find("/Managers/UI Manager").GetComponent<UIManager>();
        
        playerCombat = GetComponent<PlayerCombat>();

    }

    void Start()
    {
        // Load saved data
        SaveData data = saveManager.LoadGame();

        maxHealth = data.savedMaxHealth;
        currentHealth = data.savedCurrentHealth;

        wood = data.savedWoodCount;
        stone = data.savedStoneCount;
        metal = data.savedMetalCount;
        seeds = data.savedSeedsCount;

        ToggleCombat(true);

        uIManager.SetHealthBar(maxHealth);
        uIManager.UpdateHealthBar(currentHealth);


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
        uIManager.UpdateHealthBar(currentHealth);
        if(currentHealth <= 0)
        { Death(); }
    }

    void Death()
    {
        gameManager.GameOverState();
    }

    public void ToggleCombat(bool toggle)
    {
        playerCombat.enabled = toggle;
        toolHolder.SetActive(toggle);
    }


}
