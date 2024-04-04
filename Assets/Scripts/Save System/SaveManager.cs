using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveManager: MonoBehaviour
{

    string path = "";

    [Header("Default Values")]
    public float defaultMaxHealth = 100f;
    public float defaultTreeMaxHealth = 250f;


    private Player player;
    private PlayerInventory playerInventory;
    private HomeTree homeTree;
    private WaveManager waveManager;

    void Awake()
    {
        SetPaths();


        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        homeTree = GameObject.FindWithTag("HomeTree").GetComponent<HomeTree>();
        waveManager = GameObject.Find("/Managers/Enemy Manager").GetComponent<WaveManager>();
        playerInventory = GameObject.FindWithTag("Player").GetComponent<PlayerInventory>();

        }   
    }


    public SaveData GetData ()
    {
        // Fetch variables to be saved
        float maxHealth = player.maxHealth;
        float currentHealth = player.currentHealth;
        int wave = waveManager.currentWave;
        float maxTreeHealth = homeTree.maxHealth;
        float currentTreeHealth = homeTree.currentHealth;
        int woodCount = playerInventory.wood;
        int stoneCount = playerInventory.stone;
        int metalCount = playerInventory.metal;
        int seedsCount = playerInventory.seeds;


//float health, int wave, int woodCount, int stoneCount, int metalCount, int seedsCount

        // Assign data to class
        SaveData data = new SaveData(maxHealth, currentHealth,wave, maxTreeHealth, currentTreeHealth, woodCount, stoneCount, metalCount, seedsCount);

        return data;
    }
    
    private void SetPaths()
    {
        path = Application.persistentDataPath + "/runData.json";
    }


    public void SaveGame ()
    {
        SaveData saveData = GetData();

        Debug.Log("Saving data at " + path);
        string json = JsonUtility.ToJson(saveData);

        using StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
    }

    public void NewSaveGame ()
    {
        // Create blank save data file
        SaveData saveData = new SaveData(defaultMaxHealth,defaultMaxHealth, 0,defaultTreeMaxHealth,defaultTreeMaxHealth,0,0,0,0);

        string json = JsonUtility.ToJson(saveData);

        using StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
    }

    public SaveData LoadGame ()
    {
        SaveData data;

        // Check if data exists
        if (System.IO.File.Exists(path))
        {
            //Debug.Log("Save file found");
            // Load existing data
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();

            data = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            //Debug.Log("No save data file found");
            // Create new data
            NewSaveGame();
            //Debug.Log("New save data file created");
            // Load new data
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();

            data = JsonUtility.FromJson<SaveData>(json);
        }

        return data;
    }

    public void ResetData()
    {
        // Create a blank data file
        NewSaveGame();
        // Return to Main Menu
        SceneManager.LoadScene(0);

    }

}