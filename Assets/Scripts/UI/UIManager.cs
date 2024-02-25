using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    // References
    Player player;
    HomeTree tree;
    WaveManager enemyManager;
    BuildingManager buildingManager;

    [Header("Combat HUD")]
    [SerializeField] private GameObject combatHUD;
    public GameObject buildIndicator;
    public TMP_Text healthText;
    public TMP_Text treeHealthText;
    public TMP_Text waveCounterText;

    [Header("Build HUD")]
    [SerializeField] private GameObject buildHUD;
    [SerializeField] private GameObject buildMenu;
    [SerializeField] public TMP_Text woodCountText;
    [SerializeField] public TMP_Text stoneCountText;
    [SerializeField] public TMP_Text metalCountText;
    [SerializeField] public TMP_Text seedsCountText;

    [Header("Popups")]
    [SerializeField] public GameObject popupParent;
    [SerializeField] public GameObject popupPrefab;
    [SerializeField] public List<ResourcePopupSO> resourcePopup;

    [Header("Other")]
    [SerializeField] private bool isCombatHUD;
    [SerializeField] private bool isBuildHUD;


    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        tree = GameObject.FindWithTag("HomeTree").GetComponent<HomeTree>();
        enemyManager = GameObject.Find("/Managers/Enemy Manager").GetComponent<WaveManager>();
        buildingManager = GameObject.Find("/Managers/Building Manager").GetComponent<BuildingManager>();
        
    isCombatHUD = true;
    isBuildHUD = false;

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
            healthText.text = "Health: " + player.currentHealth;
            treeHealthText.text = "Tree Health: " + tree.currentHealth;
            waveCounterText.text = "Wave: " + enemyManager.currentWave;
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

    }

    public void NewResourcePopup(int type, int amount)
    {
        //ResourcePopupSO resourceType = resourcePopup[i];

        Debug.Log("New Popup");
        var popup = Instantiate (popupPrefab.transform, transform.position , Quaternion.identity, popupParent.transform);
        var popupTemplate = popup.GetComponent<ResourcePopupTemplate>();
        popupTemplate.typeText.text = resourcePopup[type].title;
        popupTemplate.amountText.text = "+" + amount;
        popupTemplate.image.sprite = resourcePopup[type].image;
        
    }
}
