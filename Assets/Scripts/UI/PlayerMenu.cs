using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerMenu : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private GameObject statsTab;
    [SerializeField] private PlayerInventory playerInventory;
    [SerializeField] public TMP_Text playerHealthText;
    [SerializeField] public TMP_Text treeHealthText;
    [SerializeField] public TMP_Text woodCountText;
    [SerializeField] public TMP_Text stoneCountText;
    [SerializeField] public TMP_Text metalCountText;
    [SerializeField] public TMP_Text seedsCountText;

    [Header("Perks")]
    [SerializeField] private GameObject perksTab;


    [Header("Database")]
    [SerializeField] private GameObject databaseTab;
    

    Player player;


    void Awake()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();


        statsTab.SetActive(true);
        perksTab.SetActive(false);
        databaseTab.SetActive(false);
    }

    void Update()
    {
        if(statsTab.activeInHierarchy)
        {
            woodCountText.text = playerInventory.wood.ToString();
            stoneCountText.text = playerInventory.stone.ToString();
            metalCountText.text = playerInventory.metal.ToString();
            seedsCountText.text = playerInventory.seeds.ToString();

            playerHealthText.text = (player.currentHealth + "/" + player.maxHealth);
        }

    }


}
