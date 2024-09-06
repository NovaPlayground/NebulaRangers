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
    //[SerializeField] private int poolSize = 10; 
    

    private float timer;
    private int spawnedEnemiesCount; // Counter for enemies already spawned
    private int currentEnemyCount; // Counter for the number of enemies currently active
    //private Queue<GameObject> enemyPool;  

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval;
        currentEnemyCount = 0;
        spawnedEnemiesCount = 0;

        //InitializePool();
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

    //private void InitializePool()
    //{
    //    enemyPool = new Queue<GameObject>();

    //    for (int i = 0; i < poolSize; i++)
    //    {
    //        //GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
    //        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
    //        GameObject enemy = Instantiate(enemyPrefab);
    //        enemy.SetActive(false); // Deactivate the object until it is needed
    //        enemyPool.Enqueue(enemy);
    //    }
    //}

    private void SpawnEnemy()
    {

        if (currentEnemyCount < maxEnemiesInScene && spawnedEnemiesCount < totalEnemiesToSpawn)
        {

            GameObject enemy = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject newEnemy = Instantiate(enemy, spawnPoint.position, Quaternion.identity);

            IDestroyable destroyable = newEnemy.GetComponent<IDestroyable>();
            if(destroyable != null) 
            {
                destroyable.OnDestroyed += OnEnemyDestroyed;
            }

            currentEnemyCount++;
            spawnedEnemiesCount++;
        }
    }

   

    public void OnEnemyDestroyed(GameObject enemy)
    {
        currentEnemyCount--;
        Destroy(enemy);

        Debug.Log($"Enemy destroyed: {enemy.name}");
    }
}

