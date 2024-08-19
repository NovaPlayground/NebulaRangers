using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private GameObject shield;
    [SerializeField] private float maxHealth;
    [SerializeField] private float currentHealth;
    private bool isShieldActive;
    


    public void Initialize(float shieldMaxHealt)
    {
        maxHealth = shieldMaxHealt;
        currentHealth = maxHealth;
        isShieldActive = false;

        if (shield != null) 
        {
            shield.SetActive(false); // Ensure the barrier is initially deactivated
        }
        else
        {
            Debug.LogError("shield sphere is not assigned!");
        }


        
    }

    // Activate the barrier with a certain amount of health
    public void Activate() 
    {
        isShieldActive = true;
        currentHealth = maxHealth;

        if (shield != null)
        {
            shield.SetActive(true); // Show barrier visually
        }

        Debug.Log("Shield activated with " + currentHealth + " health.");
    }


    // Deactivate the barrier
    public void Deactivate() 
    {
         isShieldActive = false;

        if (shield != null)
        {
            shield.SetActive(false); // Ensure the barrier is initially deactivated
        }
        else
        {
            Debug.LogError("shield sphere is deactivated");
        }
    }


    // Absorb damage using the barrier
    public bool AbsorbDamage(float damage) 
    {
        if (!isShieldActive) return false;
       
        currentHealth -= damage;
        Debug.Log("shield absorbed damage. Remaining shield health :  " + currentHealth);

        if (currentHealth <= 0f) 
        {
            Deactivate();
        }

        return true; // Damage was absorbed by the barrier
    }

    // Check if the barrier is active
    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}
