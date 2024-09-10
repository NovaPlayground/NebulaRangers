using UnityEngine;

public class EnemyShield : MonoBehaviour, IDamageable, IDestroyable, IEnemy
{
    [SerializeField] private float health = 100f; // Enemy's current health
    [SerializeField] private float maxHealth = 100f; // Enemy's max health
    [SerializeField] private BoxCollider enemyBoxCollider;
    


    // SHIELD
    [SerializeField] private Shield shield;
    [SerializeField] private float shieldHealth = 50f;
    [SerializeField] private float shieldDelay = 1.5f;
    private float shieldDelayTimer;
    private bool isColliderDisabled = false;

    private Rigidbody rb;
    private SpawnManager spawnManager;


    public event System.Action<GameObject> OnDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();

        health = maxHealth;

        if (enemyBoxCollider == null)
        {
            enemyBoxCollider = GetComponent<BoxCollider>();
        }

        shield.InitializeEnemy(shieldHealth);
        EnableShield();

    }

    // Update is called once per frame
    void Update()
    {
        if (!shield.IsShieldActive())
        {

            shieldDelayTimer -= Time.deltaTime;

            if (shieldDelayTimer <= 0f)
            {
                EnablePlayerCollider();
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
        // SHIELD
        if (shield.IsShieldActive())
        {
            shield.AbsorbDamage(damage); // Redirects damage to the shield          
            return; // Exit the method if the shield takes damage
        }

        health -= damage;
        
        if (health <= 0f)
        {
            //Destroy(gameObject);
            Debug.Log("enemy has died.");
            Die();


        }
    }

    private void EnableShield()
    {

        shield.Activate();
        DisablePlayerCollider(); // Disables the player's collider when the shield is active
        shieldDelayTimer = shieldDelay;
        Debug.Log("Shield activated. Player's collider disabled.");
    }


    private void EnablePlayerCollider()
    {
        if (enemyBoxCollider != null)
        {
            enemyBoxCollider.enabled = true;
            isColliderDisabled = false;

        }
    }

    private void DisablePlayerCollider()
    {
        if (enemyBoxCollider != null)
        {
            enemyBoxCollider.enabled = false;
            isColliderDisabled = true;
        }
    }



    public void Die()
    {

        // Management of enemy death
        OnDestroyed?.Invoke(gameObject); // Notifies SpawnManager of destruction


        spawnManager.OnEnemyDestroyed(gameObject);
        
        
        gameObject.SetActive(false);

    }
}
