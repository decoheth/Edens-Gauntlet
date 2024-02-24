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


    private Player player;
    private WaveManager waveManager;

    void Awake()
    {
        SetPaths();


        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        waveManager = GameObject.Find("/Managers/Enemy Manager").GetComponent<WaveManager>();
        }   
    }


    public void Update ()
    {
        if(Input.GetKeyDown(KeyCode.O))
            SaveGame();
        if(Input.GetKeyDown(KeyCode.P))
            ResetData();

    }



    public SaveData GetData ()
    {
        // Fetch variables to be saved
        float currentHealth = player.currentHealth;
        float maxHealth = player.maxHealth;
        int wave = waveManager.currentWave;
        int woodCount = player.wood;
        int stoneCount = player.stone;
        int metalCount = player.metal;
        int seedsCount = player.seeds;


//float health, int wave, int woodCount, int stoneCount, int metalCount, int seedsCount

        // Assign data to class
        SaveData data = new SaveData(currentHealth,maxHealth,wave, woodCount, stoneCount, metalCount, seedsCount);

        return data;
    }
    
    private void SetPaths()
    {
        path = Application.persistentDataPath + "/savaData.json";
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
        SaveData saveData = new SaveData(defaultMaxHealth,defaultMaxHealth, 0,0,0,0,0);

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
            Debug.Log("Save file found");
            // Load existing data
            using StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();

            data = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.Log("No save data file found");
            // Create new data
            NewSaveGame();
            Debug.Log("New save data file created");
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