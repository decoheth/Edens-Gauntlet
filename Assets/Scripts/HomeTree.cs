using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTree : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    private GameManager gameManager;

    void Awake()
    {
        currentHealth = maxHealth;

        gameManager = GameObject.Find("/Managers/Game Manager/").GetComponent<GameManager>();
        
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

    void Death()
    {
        gameManager.GameOverState();
    }
}
