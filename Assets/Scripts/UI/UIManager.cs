using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Combat HUD")]
    [SerializeField] private GameObject combatHUD;
    public Slider healthBar;
    public TMP_Text treeHealthText;
    public TMP_Text waveCounterText;
    public Image waveCounterImage;

    [Header("Build HUD")]
    [SerializeField] private GameObject buildHUD;
    [SerializeField] private GameObject buildMenu;
    [SerializeField] public TMP_Text woodCountText;
    [SerializeField] public TMP_Text stoneCountText;
    [SerializeField] public TMP_Text metalCountText;
    [SerializeField] public TMP_Text seedsCountText;

    [Header("Pause Menu")]
    [SerializeField] public GameObject pauseMenu;

    [Header("Game Over Menu")]
    [SerializeField] public GameObject gameOverMenu;

    [Header("Popups")]
    [SerializeField] public GameObject popupParent;
    [SerializeField] public GameObject popupPrefab;
    [SerializeField] public List<ResourcePopupSO> resourcePopup;

    [Header("Other")]
    [SerializeField] private GameObject playerMenu;
    [SerializeField] private bool isCombatHUD;
    [SerializeField] private bool isBuildHUD;
    [SerializeField] public bool isDead;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerCombat playerCombat;

    // References
    Player player;
    PlayerInventory playerInventory;
    HomeTree tree;
    WaveManager enemyManager;
    BuildingManager buildingManager;


    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();
        tree = GameObject.FindWithTag("HomeTree").GetComponent<HomeTree>();
        enemyManager = GameObject.Find("/Managers/Enemy Manager").GetComponent<WaveManager>();
        buildingManager = GameObject.Find("/Managers/Building Manager").GetComponent<BuildingManager>();
        
        isCombatHUD = true;
        isBuildHUD = false;
        isDead = false;
        gameOverMenu.SetActive(false);
        playerMenu.SetActive(false);
        pauseMenu.SetActive(false);
        GameOverMenu(false);

        float maxHealth = player.maxHealth;

        

    }

    void Update()
    {   
        if(buildingManager.isBuilding == true)
        {
            isCombatHUD = false;
            isBuildHUD = true;
            buildHUD.SetActive(true);
        }
        else if (buildingManager.isBuilding == false && buildMenu.activeInHierarchy == false)
        {
            isCombatHUD = true;
            isBuildHUD = false;
            combatHUD.SetActive(true);
        }
            


        // Combat HUD
        if(isCombatHUD == true && combatHUD.activeInHierarchy && buildMenu.activeInHierarchy == false && !isDead)
        {
            // Update HUD
            treeHealthText.text = "Tree Health: " + tree.currentHealth;
            
        }

        if(isCombatHUD == false && combatHUD.activeInHierarchy)
        {
            combatHUD.SetActive(false);
        }

        // Build HUD

        if(isBuildHUD == true && buildHUD.activeInHierarchy && !isDead)
        {
            // Update HUD
            // Resources
            woodCountText.text = playerInventory.wood.ToString();
            stoneCountText.text = playerInventory.stone.ToString();
            metalCountText.text = playerInventory.metal.ToString();
            seedsCountText.text = playerInventory.seeds.ToString();
        }

        if(isBuildHUD == false && buildHUD.activeInHierarchy)
        {
            buildHUD.SetActive(false);
        }

        // Pause Menu
        if(isCombatHUD && !playerMenu.activeInHierarchy && !isDead)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
                {
                    TogglePauseMenu(!pauseMenu.activeInHierarchy);
                }

        }


        // Player Menu
        if(Input.GetKeyDown(KeyCode.Tab) && !pauseMenu.activeInHierarchy && isCombatHUD)
        {
            TogglePlayerMenu(!playerMenu.activeInHierarchy);
        }

        if(Input.GetKeyDown(KeyCode.Escape) && playerMenu.activeInHierarchy)
        {
            TogglePlayerMenu(false);  
        }

    }

    public void NewResourcePopup(int type, int amount)
    {
        var popup = Instantiate (popupPrefab.transform, transform.position , Quaternion.identity, popupParent.transform);
        var popupTemplate = popup.GetComponent<ResourcePopupTemplate>();
        popupTemplate.typeText.text = resourcePopup[type].title;
        popupTemplate.amountText.text = "+" + amount;
        popupTemplate.image.sprite = resourcePopup[type].image;
        
    }
    
    public void UpdateWaveCounter (int wave, bool waveActive)
    {
        waveCounterText.text = wave.ToString();
        if(waveActive)
            waveCounterImage.color = new Color32(255,0,0,255);
        else
            waveCounterImage.color = new Color32(0,0,0,255);
    }
    public void UpdateHealthBar (float health)
    {
        healthBar.value = health;
    }
    public void SetHealthBar (float maxHealth)
    {
        healthBar.maxValue = maxHealth;
    }

    public void TogglePauseMenu(bool paused)
    {
        pauseMenu.SetActive(paused);

        playerMovement.canMove = !paused;
        playerCombat.canAttack = !paused;

        Cursor.visible = paused;
        Cursor.lockState = paused ? CursorLockMode.Confined : CursorLockMode.Locked;

        Time.timeScale = paused ? 0f : 1f;
        
        if(paused == true)
        {
            buildingManager.canBuild = false;
        }
    }


    public void GameOverMenu(bool active)
    {
        gameOverMenu.SetActive(active);
        isDead = active;

        playerMovement.canMove = !active;
        playerCombat.canAttack = !active;

        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.Confined : CursorLockMode.Locked;

        Time.timeScale = active ? 0f : 1f;
    }

        public void TogglePlayerMenu(bool active)
    {
        playerMenu.SetActive(active);
        playerMovement.canMove = !active;
        playerCombat.canAttack = !active;
            
        Cursor.visible = active;
        Cursor.lockState = active ? CursorLockMode.Confined : CursorLockMode.Locked;

        Time.timeScale = active ? 0f : 1f;
        


    }
    
}
