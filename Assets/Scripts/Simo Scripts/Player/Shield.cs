using UnityEngine;

public class Shield : MonoBehaviour
{ 
    [SerializeField] private GameObject shield;   
    private float shieldHealth;
    private bool isShieldActive;


    public void Initialize(float maxHealth)
    {
        shieldHealth = maxHealth;
        shield.SetActive(false); // set shield
    }

    public void InitializeEnemy(float maxHealth)
    {
        shieldHealth = maxHealth;
        
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
    public void AbsorbDamage(float damage) 
    {

        Debug.Log(shieldHealth);
        shieldHealth -= damage;

        if (shieldHealth <= 0f)
        {
            Deactivate();
                      
        }
    }


    // Check if the barrier is active
    public bool IsShieldActive()
    {
        return isShieldActive;
    }

    
}
