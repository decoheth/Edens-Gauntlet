using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkSelection : MonoBehaviour
{

    PerkManager perkManager;
    UIManager uiManager;

    public GameObject perkCardParent;
    public GameObject perkCardPrefab;


    public int perkChoiceNumber = 3;

    // Initialize lists
    private List<PerkSO> availablePerks = new();

    void Awake()
    {
        perkManager = GameObject.Find("/Managers/Game Manager").GetComponent<PerkManager>();
        uiManager = GameObject.Find("/Managers/UI Manager").GetComponent<UIManager>();
    }

    private void OnEnable()
    {
        // Clear any perks from last time function was called
        foreach (Transform child in perkCardParent.transform) 
        {
            GameObject.Destroy(child.gameObject);
        }

        DisplayPerkSelection();

    }

    public void SelectPerk(PerkSO selectedPerk)
    {
        // Unlock the PerkSO corresponding to the button pressed
        perkManager.UnlockPerk(selectedPerk);
        uiManager.TogglePerkSelectionScreen(false);
    }


    public void DisplayPerkSelection()
    {

        // Get perks which are available to be unlocked
        availablePerks = perkManager.AvailablePerks();

        for (int i = 0; i < perkChoiceNumber; i++)
        {
            if(availablePerks.Count < 1)
            {
                Debug.Log("No available perks");
                return;
            }
            else
            {
                // Get random perk index from pool of available perks
                int index = Random.Range(0, availablePerks.Count);

                // Create card GameObject and fill it with data
                var perkCard = Instantiate (perkCardPrefab, transform.position , Quaternion.identity, perkCardParent.transform);
                var perkTemplate = perkCard.GetComponent<PerkCardTemplate>();
                perkTemplate.PerkTitle.text = availablePerks[index].title;
                perkTemplate.PerkDescription.text = availablePerks[index].description;
                perkTemplate.PerkImage.sprite = availablePerks[index].image;
                perkTemplate.PerkCode = availablePerks[index].code;

                PerkSO tmp_perk = availablePerks[index];
                perkCard.GetComponent<Button>().onClick.AddListener(() => SelectPerk(tmp_perk));
            
                // Remove chosen perk from available perks
                availablePerks.RemoveAt(index);
            }
        }


    }
    
}
