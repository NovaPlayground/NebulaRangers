using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyShield : MonoBehaviour, IDamageable, IDestroyable, IEnemy
{
    [SerializeField] private float health = 100f; // Enemy's current health
    [SerializeField] private float maxHealth = 100f; // Enemy's max health
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private Shield shield;
    

    // SHIELD
    private float barrierMaxHealth = 100f;
    private float reenableColliderTime = 1.5f; // Time to wait before re-enabling the collider
    private float immuneTimeAfterShield = 0.5f; // Time during which the enemy is immune to damage after shield is destroyed
    private float reenableColliderTimer = 0f; // Tracks the remaining time to re-enable the collider
    private float immuneTimer = 0f; // Timer to handle immune state
    private bool isColliderDisabled = false;
    private bool isImmune = false;

    private Rigidbody rb;
    private SpawnManager spawnManager;


    public event System.Action<GameObject> OnDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();

        health = maxHealth;

        if (boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider>();
        }

        if (shield != null)
        {
            boxCollider.enabled = false; // Disable BoxCollider while shield is active
            shield.Initialize(50f); // Initialize shield with desired health
            shield.Activate(); // Ensure shield is active at spawn

            Debug.Log("Shield activated and collider disabled.");
        }
        else
        {
            Debug.LogError("Shield component is not assigned or is missing.");
        }

        Debug.Log("Enemy spawned at position: " + transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
    }

    private void UpdateTimers()
    {
        if (isColliderDisabled)
        {
            reenableColliderTimer -= Time.deltaTime; // Decrement the timer
            if (reenableColliderTimer <= 0f)
            {
                EnableCollider(); // Re-enable the collider
            }
        }

        if (isImmune)
        {
            immuneTimer -= Time.deltaTime; // Decrement the immune timer
            if (immuneTimer <= 0f)
            {
                isImmune = false; // Reset the immune state
            }
        }
    }

    



    public Rigidbody GetRigidbody()
    {
        rb = GetComponent<Rigidbody>();

        return rb;
    }

    public float GetHealth() { return health; }
    public float GetMaxHealth() { return maxHealth; }

    public void SetHealth(float newHealth)
    {
        health = Mathf.Clamp(newHealth, 0, maxHealth);
    }


    public void TakeDamage(float damage)
    {
        if (isImmune) // Check if enemy is currently immune
        {
            return;
        }

        if (shield != null && shield.IsShieldActive())
        {
            bool damageAbsorbed = shield.AbsorbDamage(damage);

            if (!damageAbsorbed)
            {
                if (!isColliderDisabled) // Check if collider is already disabled
                {
                    DisableCollider();
                    reenableColliderTimer = reenableColliderTime;
                    isImmune = true;
                    immuneTimer = immuneTimeAfterShield;
                }
            }
        }
        else
        {
            // If shield is not active or not enough to absorb damage, reduce enemy health
            health -= damage;

            if (health <= 0f)
            {
                Die();
            }
        }
    }

    private void DisableCollider()
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
            isColliderDisabled = true;
        }
    }

    private void EnableCollider()
    {
        if (boxCollider != null)
        {
            boxCollider.enabled = true;
            isColliderDisabled = false;
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
