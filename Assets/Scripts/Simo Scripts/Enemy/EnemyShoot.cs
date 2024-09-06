using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour, IDamageable, IDestroyable, IEnemy
{   
    [SerializeField] private float health = 100f; // Enemy's current health
    [SerializeField] private float maxHealth = 100f; // Enemy's max health
    
    private Rigidbody rb;
    private SpawnManager spawnManager;

    public event System.Action<GameObject> OnDestroyed;

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

        if (health <= 0f)
        {
            Die();
        }
    }


    public void Die() 
    {

        

        // Gestione della morte del nemico
        OnDestroyed?.Invoke(gameObject); // Notifica lo SpawnManager della distruzione

        spawnManager.OnEnemyDestroyed(gameObject);

        //Destroy(gameObject);
        gameObject.SetActive(false);

    }

}
