using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class resourcePickup : MonoBehaviour
{
    
    public pickupObject currentObject;
    public int pickupQuantity;

    private GameObject player;
    private PlayerInventory playerInventory;
    private UIManager uiManager;

    void Awake ()
    {
       player = GameObject.FindWithTag("Player");
       playerInventory = player.GetComponent<PlayerInventory>();
       uiManager = GameObject.Find("/Managers/UI Manager/").GetComponent<UIManager>();

    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            if(currentObject == pickupObject.WOOD)
            {
                playerInventory.wood += pickupQuantity;
                uiManager.NewResourcePopup(0, pickupQuantity);

            }
            else if(currentObject == pickupObject.STONE)
            {
                playerInventory.stone += pickupQuantity;
                uiManager.NewResourcePopup(1, pickupQuantity);
            }
            else if(currentObject == pickupObject.METAL)
            {
                playerInventory.metal += pickupQuantity;
                uiManager.NewResourcePopup(2, pickupQuantity);
            }
            else if(currentObject == pickupObject.SEEDS)
            {
                playerInventory.seeds += pickupQuantity;
                uiManager.NewResourcePopup(3, pickupQuantity);
            }

            // Destory after pickup
            Destroy(gameObject);
        }

    }
}

[System.Serializable] public enum pickupObject{WOOD,STONE,METAL,SEEDS};