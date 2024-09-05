using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; 
    [SerializeField] private Transform[] spawnPoints;  
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private int maxEnemiesInScene = 5;
    [SerializeField] private int totalEnemiesToSpawn = 10; 
    [SerializeField] private int poolSize = 10; 

    private float timer;
    private int spawnedEnemiesCount; // Counter for enemies already spawned
    private int currentEnemyCount; // Counter for the number of enemies currently active
    private Queue<GameObject> enemyPool;  

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval;
        currentEnemyCount = 0;
        spawnedEnemiesCount = 0;

        InitializePool();
    }

    // Update is called once per frame
    void Update()
    {
    
        if (spawnedEnemiesCount < totalEnemiesToSpawn && currentEnemyCount < maxEnemiesInScene) 
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnEnemy();
                timer = 0f;
            }
        }
        
    }

    private void InitializePool()
    {
        enemyPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
            enemy.SetActive(false); // Deactivate the object until it is needed
            enemyPool.Enqueue(enemy);
        }
    }

    private void SpawnEnemy()
    {
        
        if (enemyPool.Count > 0 && currentEnemyCount < maxEnemiesInScene && spawnedEnemiesCount < totalEnemiesToSpawn)
        {
           
            GameObject enemy = enemyPool.Dequeue();
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = Quaternion.identity;
            enemy.SetActive(true);

            currentEnemyCount++;
            spawnedEnemiesCount++;
        }
    }

   

    public void OnEnemyDestroyed(GameObject enemy)
    {
        // Don't put the enemy back into the pool, just deactivate it
        enemy.SetActive(false);

        currentEnemyCount--;
    }
}

