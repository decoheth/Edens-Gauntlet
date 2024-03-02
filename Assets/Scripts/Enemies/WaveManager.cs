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
    public List<Enemy> Enemies = new List<Enemy>();

    [Header("Spawning Zones")]
    [SerializeField] Transform SpawnField;
    float spawning_x_dim;
    float spawning_z_dim;


    [Header("Waves")]

    public List<GameObject> currentEnemyWave = new List<GameObject>();
    //public float scalingModifer;

    // Managers
    UIManager uIManager;
    BuildingManager buildingManager;
    NavMeshManager navMeshManager;


    void Awake()
    {
        uIManager = GameObject.Find("/Managers/UI Manager").GetComponent<UIManager>();
        buildingManager = GameObject.Find("/Managers/Building Manager").GetComponent<BuildingManager>();
        navMeshManager = GetComponent<NavMeshManager>();


    }

    void Start()
    {
        // Get Spawning Field dimensions
        spawning_x_dim = SpawnField.GetComponent<MeshRenderer>().bounds.size.x;
        spawning_z_dim = SpawnField.GetComponent<MeshRenderer>().bounds.size.z;
        spawning_x_dim /= 2;
        spawning_z_dim /= 2; 

        waveActive = false;
    }


    void Update()
    {
        if(waveActive == true)
        {
            // Check if all enemies have been killed
            if(currentEnemyWave.Count == 0 )
            {
                // Wave ended
                waveActive = false;
                currentWave++;
                uIManager.UpdateWaveCounter(currentWave);
                buildingManager.ToggleCanBuild(true);
            }
        }
    }

    public void SpawnWave()
    {
        navMeshManager.BakeNavMesh(false);

        waveActive = true;
        buildingManager.ToggleCanBuild(false);

        int currentWaveCost = 0;
        var selectedEnemy = Enemies[0];

        int waveMaxCost = 15 + (5*currentWave);

        for(int i = 0; currentWaveCost < waveMaxCost; i++)
        {
            GameObject newSpawn = Instantiate(selectedEnemy.enemyPrefab, SpawnLocation(), Quaternion.identity, EnemyParent.transform);
            currentEnemyWave.Add(newSpawn);
            currentWaveCost += selectedEnemy.enemyCost;
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


[System.Serializable]
public class Enemy
{
    public GameObject enemyPrefab;
    public int enemyCost;
    public Faction selectedFaction;
}