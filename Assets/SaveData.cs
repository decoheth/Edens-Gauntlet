using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SaveData
{

    public float savedMaxHealth;
    public float savedCurrentHealth;
    public int savedWave;
    public float savedCurrentTreeHealth;
    public float savedMaxTreeHealth;

    // Resources
    public int savedWoodCount;
    public int savedStoneCount;
    public int savedMetalCount;
    public int savedSeedsCount;


    public SaveData (float maxHealth, float currentHealth, int wave, float maxTreeHealth, float currentTreeHealth, int woodCount, int stoneCount, int metalCount, int seedsCount) 
    {
        savedMaxHealth = maxHealth;
        savedCurrentHealth = currentHealth;
        savedWave = wave;
        savedMaxTreeHealth = maxTreeHealth;
        savedCurrentTreeHealth = currentTreeHealth;

        savedWoodCount = woodCount;
        savedStoneCount = stoneCount;
        savedMetalCount = metalCount;
        savedSeedsCount = seedsCount;

    }


}