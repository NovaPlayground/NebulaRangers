using UnityEngine;

public class EnemyShoot : MonoBehaviour, IDamageable, IDestroyable, IEnemy
{   
    [SerializeField] private float health = 100f; // Enemy's current health
    [SerializeField] private float maxHealth = 100f; // Enemy's max health
    [SerializeField] private GameObject explosionPrefab;

    private Rigidbody rb;
    private SpawnManager spawnManager;

    public event System.Action<GameObject> OnDestroyed;

    // Start is called before the first frame update
    void Start()
    {
        spawnManager = FindObjectOfType<SpawnManager>();

        health = maxHealth;
   
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
        
        health -= damage;
        

        if (health <= 0f)
        {

            Die();
        }
    }


    public void Die()
    {


        // Instantiate the explosion effect at the enemy's position and rotation
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, transform.rotation);
        }

        // Management of enemy death
        OnDestroyed?.Invoke(gameObject); // Notifies SpawnManager of destruction


        spawnManager.OnEnemyDestroyed(gameObject);


        gameObject.SetActive(false);

    }

}


