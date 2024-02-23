using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour, IInteractable
{
    GameObject enemyManager;

    void Awake ()
    {
        enemyManager = GameObject.Find("/Managers/Enemy Manager");
    }

    public void Interact()
    {
        if ( enemyManager.GetComponent<WaveManager>().waveActive == true)
        {
            Debug.Log("Wave Already Active");
        }
        else 
        {
            Debug.Log("Spawning Next Wave");
            enemyManager.GetComponent<WaveManager>().SpawnWave();
        }


    }

}