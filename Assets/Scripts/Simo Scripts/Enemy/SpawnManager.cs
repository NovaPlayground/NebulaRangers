using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; 
    [SerializeField] private Transform[] spawnPoints;  
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private int maxEnemiesInScene = 5;
    [SerializeField] private int totalEnemiesToSpawn = 5;
    
    private float timer;
    private int spawnedEnemiesCount; // Counter for enemies already spawned
    private int currentEnemyCount; // Counter for the number of enemies currently active
    private int currentEnemyCountText; // Counter for the number of enemies to show in UI
    private List<GameObject> activeEnemies = new List<GameObject>();
    private List<GameObject> enemyPool = new List<GameObject>();

    // KEY
    [SerializeField] private GameObject keyPrefab; 
    [SerializeField] private Transform keySpawnPoint;
    [SerializeField] private bool isKeySpawned; 

    // TEXT
    [SerializeField] private TextMeshProUGUI enemyCounterText;

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval;
        currentEnemyCount = 0;
        spawnedEnemiesCount = 0;
        currentEnemyCountText = totalEnemiesToSpawn;

        isKeySpawned = false;

        InitializeEnemyPool();

        UpdateEnemyCounter();

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


    private void InitializeEnemyPool()
    {
        
        foreach (var enemyPrefab in enemyPrefabs)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false); 
            enemyPool.Add(enemy);
        }
    }

    private GameObject GetPooledEnemy()
    {
        // get an enemy from the pool
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }
        return null;  // if there is no enemy in the the pool
    }

    private void SpawnEnemy()
    {
        if (activeEnemies.Count < maxEnemiesInScene && spawnedEnemiesCount < totalEnemiesToSpawn)
        {
            // Randomly select one of two enemies from the pool
            GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

            // Get an inactive enemy from the pool
            GameObject pooledEnemy = GetPooledEnemy();

            if (pooledEnemy != null)
            {
                //Set enemy location randomly between spawn points
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                pooledEnemy.transform.position = spawnPoint.position;

                //active the enemy
                pooledEnemy.SetActive(true);

                activeEnemies.Add(pooledEnemy);
            
                IDestroyable destroyable = pooledEnemy.GetComponent<IDestroyable>();
                if (destroyable != null)
                {
                    destroyable.OnDestroyed += OnEnemyDestroyed;
                }

                
                spawnedEnemiesCount++;
                UpdateEnemyCounter();
            }
        }
    }

    private void Spawnkey() 
    {
        if(keyPrefab != null && keySpawnPoint != null) 
        {
            Instantiate(keyPrefab,keySpawnPoint.position, Quaternion.identity);
            isKeySpawned = true;
        }
    }

    private void UpdateEnemyCounter()
    {
        

        int enemiesRemainingToSpawn = totalEnemiesToSpawn - spawnedEnemiesCount;

        enemiesRemainingToSpawn = Mathf.Max(0, enemiesRemainingToSpawn);

        if (enemyCounterText != null) 
        {
            enemyCounterText.text = currentEnemyCountText.ToString();
        }
    }

    public void OnEnemyDestroyed(GameObject enemy)
    {

        // Remove the enenmy from the list
        if (enemy != null && activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);

            enemy.SetActive(false);
            currentEnemyCount--;
            currentEnemyCountText--;

            UpdateEnemyCounter();

            // check to spawn the key
            if (activeEnemies.Count == 0 && spawnedEnemiesCount == totalEnemiesToSpawn)
            {
                Spawnkey();
            }

        }

       

    }

}

