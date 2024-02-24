using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{

    public float savedCurrentHealth;
    public float savedMaxHealth;
    public int savedWave;
    // Resources
    public int savedWoodCount;
    public int savedStoneCount;
    public int saveMetalCount;
    public int savedSeedsCount;


    public SaveData (float currentHealth, float maxHealth, int wave, int woodCount, int stoneCount, int metalCount, int seedsCount) 
    {
        savedCurrentHealth = currentHealth;
        savedMaxHealth = maxHealth;
        savedWave = wave;

        savedWoodCount = woodCount;
        savedStoneCount = stoneCount;
        saveMetalCount = metalCount;
        savedSeedsCount = seedsCount;

    }


}