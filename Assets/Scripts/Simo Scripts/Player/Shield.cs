using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    [SerializeField] private GameObject shield;
    [SerializeField] private float shieldHealth;
    private bool isShieldActive;


    public void Initialize(float maxHealth)
    {
        shieldHealth = maxHealth;
        shield.SetActive(false); // set shield
    }

    
    public void Activate() 
    {
        isShieldActive = true;
        shield.SetActive(true); // Shows the visual appearance of the shield
    }


    public void Deactivate() 
    {
        isShieldActive = false;
        shield.SetActive(false); // Hide the shield
    }


    // Absorb damage using the shield
    public bool AbsorbDamage(float damage) 
    {
        if (!isShieldActive) return false; // If the shield is disabled, it cannot absorb damage

        shieldHealth -= damage;
        if (shieldHealth <= 0f)
        {
            Deactivate();
            return false; // Damage not fully absorbed
        }
        return true; // Damage absorbed
    }


    // Check if the barrier is active
    public bool IsShieldActive()
    {
        return isShieldActive;
    }
}
