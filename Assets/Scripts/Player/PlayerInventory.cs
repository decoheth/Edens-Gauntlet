using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Resources")]
    public int wood;
    public int stone;
    public int metal;
    public int seeds;

    // Managers
    private SaveManager saveManager;

    
    void Awake()
    {
        saveManager = GameObject.Find("/Managers/Save Manager/").GetComponent<SaveManager>();
    }
    void Start()
    {
        // Load saved data
        SaveData data = saveManager.LoadGame();

        wood = data.savedWoodCount;
        stone = data.savedStoneCount;
        metal = data.savedMetalCount;
        seeds = data.savedSeedsCount;
    }
}
