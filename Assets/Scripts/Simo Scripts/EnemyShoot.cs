using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShoot : MonoBehaviour, IDamageable
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float health = 1f;
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float speedRotation = 5f;
    [SerializeField] private Transform player;

    [SerializeField] private DebugNormalBullet debugNormalBullet;
    [SerializeField] private float shootDelay = 1f;
    [SerializeField] private GameObject[] muzzles;

    private bool isPlayerInRange = false;
    private float shootCooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRadius)
        {
            Debug.Log("Player entered in the radius");

            if (!isPlayerInRange)
            {
                isPlayerInRange = true;
            }

            RotateTowardsPlayer();
            Debug.Log(" rotate towards to player");

            shootCooldown -= Time.deltaTime;

            if (shootCooldown <= 0f) 
            {
                Shoot();
                Debug.Log("Player firing.");
                shootCooldown = shootDelay; // Reset cooldown
            }
        }
    }

    



    private void RotateTowardsPlayer()
    {
        // Direction to player 
        Vector3 directionToPlayer = (player.position - transform.position).normalized;

        // calculate the target rotation
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        // rotate
        float rotationSpeed = speedRotation * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed);
    }

    public void TakeDamage(float damage) 
    {
        health -= damage; 

        if(health <= 0f) 
        {
            Die();
        }
    }

    public void Die() 
    {
        Destroy(gameObject);
    }


    public void Shoot()
    {
        foreach (var muzzle in muzzles) 
        {
            // instantiate a bullet
            Instantiate(debugNormalBullet, muzzle.transform.position, muzzle.transform.rotation);
        }
    }
}
