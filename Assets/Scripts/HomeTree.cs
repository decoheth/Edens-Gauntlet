using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTree : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    private GameManager gameManager;
    private SaveManager saveManager;

    void Awake()
    {
        currentHealth = maxHealth;

        gameManager = GameObject.Find("/Managers/Game Manager/").GetComponent<GameManager>();
        saveManager = GameObject.Find("/Managers/Save Manager/").GetComponent<SaveManager>();
        
    }

    void Start()
    {
        // Load saved data
        SaveData data = saveManager.LoadGame();

        maxHealth = data.savedMaxTreeHealth;
        currentHealth = data.savedCurrentTreeHealth;
    }


    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log(amount + " damage dealt to Home Tree");

        // Alert when tree is being attacked
        // Alert when tree is below half health
        if(currentHealth <= 0)
        { Death(); }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
        //uIManager.UpdateTreeHealthBar(currentHealth);
        if(currentHealth >= maxHealth)
        { currentHealth = maxHealth; }
    }


    void Death()
    {
        gameManager.GameOverState();
    }
}
