using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour, IDamageable
{   
    [SerializeField] private float health = 100f; // Enemy's current health
    [SerializeField] private float maxHealth = 100f; // Enemy's max health
    [SerializeField] private GameObject healthPickup;

    
    private Rigidbody rb;
    private SpawnManager spawnManager;

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
  
    public void SetHealth(float newHealth) 
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
    }


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

        spawnManager.OnEnemyDestroyed(gameObject);

        //Destroy(gameObject);
        gameObject.SetActive(false);

    }

}
