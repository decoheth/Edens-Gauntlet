using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("Tracking Values")]
    public int currentWave;
    public bool waveActive;
    
    [SerializeField] GameObject EnemyParent;

    [Header("Enemy Types")]
    public GameObject enemyPrefab;

    
    [System.Serializable]
    public class WaveContent
    {
        
        [SerializeField][NonReorderable] GameObject[] enemySpawn;

        public GameObject[] GetEnemySpawnList()
        {
            return enemySpawn;
        }
            
    }

    [Header("Spawning Zones")]
    [SerializeField] Transform SpawnField;
    float spawning_x_dim;
    float spawning_z_dim;



    [Header("Waves")]
    [SerializeField][NonReorderable] WaveContent[] waves;

    public List<GameObject> currentEnemyWave = new List<GameObject>();



    // Start is called before the first frame update
    void Start()
    {
        // Get Spawning Field dimensions
        spawning_x_dim = SpawnField.GetComponent<MeshRenderer>().bounds.size.x;
        spawning_z_dim = SpawnField.GetComponent<MeshRenderer>().bounds.size.z;
        spawning_x_dim /= 2;
        spawning_z_dim /= 2; 

        waveActive = false;
        //SpawnWave();
    }

    // Update is called once per frame
    void Update()
    {
        
        // Check if all enemies have been killed
        if(currentEnemyWave.Count == 0 && waveActive == true)
        {
            // Wave ended
            waveActive = false;
            currentWave++;
        }
    }

    public void SpawnWave()
    {
        waveActive = true;

        for(int i = 0; i < waves[currentWave].GetEnemySpawnList().Length; i++)
        {
            GameObject newSpawn = Instantiate(waves[currentWave].GetEnemySpawnList()[i], SpawnLocation(), Quaternion.identity, EnemyParent.transform);
            ///newSpawn.SetParent
            currentEnemyWave.Add(newSpawn);
        }
    }

    public Vector3 SpawnLocation()
    {
        Vector3 SpawnLoc;

        var x_rand = Random.Range(-spawning_x_dim, spawning_x_dim);
        var z_rand = Random.Range(-spawning_z_dim, spawning_z_dim);

        var sp_centre = SpawnField.GetComponent<MeshRenderer>().bounds.center;

        SpawnLoc = new Vector3(sp_centre.x + x_rand,1,sp_centre.z + z_rand);
        return SpawnLoc;
    }
}
