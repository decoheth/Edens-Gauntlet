using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("Combat HUD")]
    [SerializeField] private GameObject combatHUD;
    public GameObject buildIndicator;
    public Slider healthBar;
    public TMP_Text treeHealthText;
    public TMP_Text waveCounterText;

    [Header("Build HUD")]
    [SerializeField] private GameObject buildHUD;
    [SerializeField] private GameObject buildMenu;
    [SerializeField] public TMP_Text woodCountText;
    [SerializeField] public TMP_Text stoneCountText;
    [SerializeField] public TMP_Text metalCountText;
    [SerializeField] public TMP_Text seedsCountText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Popups")]
    [SerializeField] public GameObject popupParent;
    [SerializeField] public GameObject popupPrefab;
    [SerializeField] public List<ResourcePopupSO> resourcePopup;

    [Header("Other")]
    [SerializeField] private GameObject playerMenu;
    [SerializeField] private bool isCombatHUD;
    [SerializeField] private bool isBuildHUD;

    // References
    Player player;
    HomeTree tree;
    WaveManager enemyManager;
    BuildingManager buildingManager;
    MouseLook mouseLook;

    void Awake()
    {
        Application.targetFrameRate = 60;


        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        mouseLook = GameObject.FindWithTag("Player").GetComponentInChildren<MouseLook>();
        tree = GameObject.FindWithTag("HomeTree").GetComponent<HomeTree>();
        enemyManager = GameObject.Find("/Managers/Enemy Manager").GetComponent<WaveManager>();
        buildingManager = GameObject.Find("/Managers/Building Manager").GetComponent<BuildingManager>();
        
        isCombatHUD = true;
        isBuildHUD = false;

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
        if(isCombatHUD == true && combatHUD.activeInHierarchy && buildMenu.activeInHierarchy == false)
        {
            // Update HUD
            treeHealthText.text = "Tree Health: " + tree.currentHealth;
            
        }

        if(isCombatHUD == false && combatHUD.activeInHierarchy)
        {
            combatHUD.SetActive(false);
        }

        // Build HUD

        if(isBuildHUD == true && buildHUD.activeInHierarchy)
        {
            // Update HUD
            // Resources
            woodCountText.text = player.wood.ToString();
            stoneCountText.text = player.stone.ToString();
            metalCountText.text = player.metal.ToString();
            seedsCountText.text = player.seeds.ToString();
        }

        if(isBuildHUD == false && buildHUD.activeInHierarchy)
        {
            buildHUD.SetActive(false);
        }

        // Pause Menu
        if(isCombatHUD && !playerMenu.activeInHierarchy)
        {
        if(Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenu(!pauseMenu.activeInHierarchy);
            }

        }

    }

    public void NewResourcePopup(int type, int amount)
    {
        //ResourcePopupSO resourceType = resourcePopup[i];

        var popup = Instantiate (popupPrefab.transform, transform.position , Quaternion.identity, popupParent.transform);
        var popupTemplate = popup.GetComponent<ResourcePopupTemplate>();
        popupTemplate.typeText.text = resourcePopup[type].title;
        popupTemplate.amountText.text = "+" + amount;
        popupTemplate.image.sprite = resourcePopup[type].image;
        
    }
    
    public void UpdateWaveCounter (int wave)
    {
        waveCounterText.text = wave.ToString();
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
        Time.timeScale = paused ? 0f : 1f;
        mouseLook.cursorHidden = !paused;
        if(paused == true)
        {
            buildingManager.ToggleBuildingMenu(false);
            buildingManager.canBuild = false;
        }
    }
    
}
