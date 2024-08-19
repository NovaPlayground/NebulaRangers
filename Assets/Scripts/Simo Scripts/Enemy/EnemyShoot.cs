using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour, IDamageable
{   
    [SerializeField] private float health = 100f; // Enemy's current health
    [SerializeField] private float maxHealth = 100f; // Enemy's max health

    
    [SerializeField] private GameObject healthPickup;
    private Rigidbody rb;
    //private bool isHit = false;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
       
    }

  

    public Rigidbody GetRigidbody() 
    {
        rb = GetComponent<Rigidbody>();

        return rb;
    }

    public float GetHealth() {  return health; }
    public float GetMaxHealth() { return maxHealth; }
    //public bool IsHit() { return isHit;  }
    //public void ResetHit() { isHit = false; }
    





    public void TakeDamage(float damage) 
    {
        //isHit = true;
        health -= damage; 

        if(health <= 0f) 
        {
            Die();
        }
    }

    public void Die() 
    {

        if (healthPickup != null)
        {
            Instantiate(healthPickup, transform.position, Quaternion.identity);
            Debug.Log("Health pickup spawned.");
        }

        Destroy(gameObject);

     
    }

}
