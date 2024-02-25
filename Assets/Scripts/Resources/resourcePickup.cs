using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resourcePickup : MonoBehaviour
{
    
    public pickupObject currentObject;
    public int pickupQuantity;

    private GameObject player;
    private UIManager uiManager;

    void Awake ()
    {
       player = GameObject.FindWithTag("Player");
       uiManager = GameObject.Find("/Managers/UI Manager/").GetComponent<UIManager>();

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(currentObject == pickupObject.WOOD)
            {
                player.GetComponent<Player>().wood += pickupQuantity;
                uiManager.NewResourcePopup(0, pickupQuantity);

            }
            else if(currentObject == pickupObject.STONE)
            {
                player.GetComponent<Player>().stone += pickupQuantity;
                uiManager.NewResourcePopup(1, pickupQuantity);
            }
            else if(currentObject == pickupObject.METAL)
            {
                player.GetComponent<Player>().metal += pickupQuantity;
                uiManager.NewResourcePopup(2, pickupQuantity);
            }
            else if(currentObject == pickupObject.SEEDS)
            {
                player.GetComponent<Player>().seeds += pickupQuantity;
                uiManager.NewResourcePopup(3, pickupQuantity);
            }

            // Destory after pickup
            Destroy(gameObject);
        }

    }
}

[System.Serializable] public enum pickupObject{WOOD,STONE,METAL,SEEDS};