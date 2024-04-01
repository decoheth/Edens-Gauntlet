using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerkManager : MonoBehaviour
{
    


    public List<PerkSO> allPerks;
    private List<PerkSO> activePerks = new();
    private List<PerkSO> availablePerks = new();

    // Modifiers to be referenced by other scripts
    // This will make more sense a little later


    void Start()
    {
        // Apply all active perks
    }


    public void UnlockPerk(PerkSO perkToActivate)
    {
        activePerks.Add(perkToActivate);
        AddPerkEffects(perkToActivate);

        Debug.Log("Perk Activated: " + perkToActivate.title);
    }



    // Returns a list of all perks that can currently be unlocked
    public List<PerkSO> AvailablePerks()
    {
        // Clear the list
        availablePerks = new();
        // Repopulate it
        foreach (PerkSO PerkSO in allPerks)
        {
            if (IsPerkAvailable(PerkSO)) availablePerks.Add(PerkSO);
        }
        return availablePerks;
    }

    // This function determines if a given PerkSO should be shown to the player
    private bool IsPerkAvailable(PerkSO PerkSO)
    {
        // If the PerkSO is already unlocked, it isn't available
        if (IsPerkActive(PerkSO.code)) return false;
        // If a required PerkSO is missing, then this PerkSO isn't available
        foreach(PerkSO requiredPerk in PerkSO.requiredPerks)
        {
            if (!activePerks.Contains(requiredPerk)) return false;
        }
        // Otherwise, the PerkSO is available
        return true;
    }

    // Pretty simply returns whether or not the player already has a given PerkSO
    public bool IsPerkActive(int code)
    {
        foreach (PerkSO activePerk in activePerks)
        {
            if (activePerk.code == code) return true;
        }
        return false;
    }


    private void AddPerkEffects(PerkSO perk)
    {
        int func = perk.function;
        float val1 = perk.value1;
        float val2 = perk.value2;

        switch (func)
        {
            case 0:
                break;
            case 1:
                Debug.Log("Health increased by +" + val1);
                break;
            case 2:
                Debug.Log("Tree Health increased by +" + val1);
                break;
            case 3:
                Debug.Log("Shovel damage increased by +" + val1);
                break;
        }
    }

    
}


[System.Serializable]
public enum PerkTier
{
    TIER_1,
    TIER_2,
    TIER_3,
    MUTATED,
    GOLD,
}
