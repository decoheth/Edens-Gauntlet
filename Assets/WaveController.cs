using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{
    public float radius = 1.5f;
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

    void OnDrawGizmosSelected ()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


}
