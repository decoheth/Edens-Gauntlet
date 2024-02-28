using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevResourceDrop : MonoBehaviour, IInteractable
{
    DropLoot loot;

    void Awake()
    {
        loot = GetComponent<DropLoot>();
    }
    
    public void Interact()
    {
        loot.dropLoot();
    }
}
