using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName="LootTable", menuName="Scriptable Objects/New LootTable")]
public class DropLoot : MonoBehaviour
{

    private GameObject pickupParent;

    [System.Serializable]
    public class DropTableContent
    {
        [SerializeField] public GameObject dropPrefab;
        [SerializeField] [Range(0f,100f)] public float dropChance;
        [SerializeField] public int minAmount;
        [SerializeField] public int maxAmount;
    }

    void Awake()
    {
        pickupParent = GameObject.Find("Pickups");
    }

    [SerializeField][NonReorderable] DropTableContent[] DropTable;

    public void dropLoot()
    {
        foreach(DropTableContent drop in DropTable)
        {
            // Chance of spawn
            if(Random.Range(0f,100f) <= drop.dropChance)
            {
                int spawnAmount = Random.Range(drop.minAmount, drop.maxAmount + 1);

                GameObject spawnedDrop = Instantiate(drop.dropPrefab, transform.position, Quaternion.identity, pickupParent.transform);
                // Set Quantity
                spawnedDrop.GetComponent<resourcePickup>().pickupQuantity = spawnAmount;

            }

        }
    }
}
