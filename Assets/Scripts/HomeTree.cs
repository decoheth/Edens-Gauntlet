using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeTree : MonoBehaviour
{
    public float maxHealth;
    public float currentHealth;

    public int level;
    public int buildRadius;

    void Awake()
    {
        currentHealth = maxHealth;
        level = 1;
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
        // Game Manager: End Game
        Debug.Log("GAME OVER, Home Tree Destroyed");
    }
}
