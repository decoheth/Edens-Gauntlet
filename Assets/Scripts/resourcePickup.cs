using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resourcePickup : MonoBehaviour
{
    public enum pickupObject{WOOD,STONE,METAL,SEEDS};
    public pickupObject currentObject;
    public int pickupQuantity;

    GameObject player;

    void Awake ()
    {
       player = GameObject.FindWithTag("Player");

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(currentObject == pickupObject.WOOD)
            {
                player.GetComponent<Player>().wood += pickupQuantity;
            }
            else if(currentObject == pickupObject.STONE)
            {
                player.GetComponent<Player>().stone += pickupQuantity;
            }
            else if(currentObject == pickupObject.METAL)
            {
                player.GetComponent<Player>().metal += pickupQuantity;
            }
            else if(currentObject == pickupObject.SEEDS)
            {
                player.GetComponent<Player>().seeds += pickupQuantity;
            }

            // Destory after pickup
            Destroy(gameObject);
        }

    }
}
