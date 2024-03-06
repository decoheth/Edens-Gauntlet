using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    



    [Header("Stats")]
    
    public float maxHealth = 100f;
    public float currentHealth;
    public float playerReach = 1.5f;



    [Header("Camera")]
    private Camera cam;
    
    [Header("Other")]

    private PlayerCombat playerCombat;
    private UIManager uIManager;
    private BuildingManager buildingManager;
    private WaveManager waveManager;
    private GameManager gameManager;
    private SaveManager saveManager;
    public GameObject toolHolder;

    void Awake()
    {
        cam = Camera.main;
        gameManager = GameObject.Find("/Managers/Game Manager/").GetComponent<GameManager>();
        saveManager = GameObject.Find("/Managers/Save Manager/").GetComponent<SaveManager>();
        uIManager = GameObject.Find("/Managers/UI Manager").GetComponent<UIManager>();
        buildingManager = GameObject.Find("/Managers/Building Manager").GetComponent<BuildingManager>();
        waveManager = GameObject.Find("/Managers/Enemy Manager").GetComponent<WaveManager>();
        
        playerCombat = GetComponent<PlayerCombat>();

    }

    void Start()
    {
        // Load saved data
        SaveData data = saveManager.LoadGame();

        maxHealth = data.savedMaxHealth;
        currentHealth = data.savedCurrentHealth;



        ToggleCombat(true);

        uIManager.SetHealthBar(maxHealth);
        uIManager.UpdateHealthBar(currentHealth);


    }

    void Update()
    {



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

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        uIManager.UpdateHealthBar(currentHealth);
        if(currentHealth <= 0)
        { Death(); }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        uIManager.UpdateHealthBar(currentHealth);
        if(currentHealth >= maxHealth)
        { currentHealth = maxHealth; }
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

    private void OnTriggerStay(Collider other)
    {
        if(waveManager.waveActive == false)
        {
            if(other.CompareTag("BuildRegion") && buildingManager.canBuild == false)
            {
                buildingManager.ToggleCanBuild(true);
            }

            if(other.CompareTag("NoBuildRegion") && buildingManager.canBuild == true)
            {
                buildingManager.ToggleCanBuild(false); 
            }
        }

    }

}
