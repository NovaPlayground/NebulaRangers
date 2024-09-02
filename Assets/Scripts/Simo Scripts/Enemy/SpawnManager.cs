using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; 
    [SerializeField] private Transform[] spawnPoints;  
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private int maxEnemies = 5;
    [SerializeField] private int poolSize = 10; // Dimensione dell'Object Pool


    private float timer;
    private int currentEnemyCount; // Counter for the number of enemies currently active
    private Queue<GameObject> enemyPool; // Object Pool per i nemici

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval;
        currentEnemyCount = 0;

        InitializePool();
    }

    // Update is called once per frame
    void Update()
    {
        //if (currentEnemyCount < maxEnemies)
        //{
        //    timer += Time.deltaTime;

        //    if (timer >= spawnInterval)
        //    {
        //        SpawnEnemy();
        //        timer = 0;
        //    }
        //}

        timer += Time.deltaTime;

        if (timer >= spawnInterval && currentEnemyCount < maxEnemies)
        {
            SpawnEnemy();
            timer = 0f;
        }
    }

    private void InitializePool()
    {
        enemyPool = new Queue<GameObject>();

        for (int i = 0; i < poolSize; i++)
        {
            GameObject enemy = Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)]);
            enemy.SetActive(false); // Disattiva l'oggetto finché non è necessario
            enemyPool.Enqueue(enemy);
        }
    }

    private void SpawnEnemy()
    {
        //foreach (Transform spawnPoint in spawnPoints)
        //{
        //    if (currentEnemyCount < maxEnemies) 
        //    {
        //        // Select a random enemy from the array
        //        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

        //        // Spawn enemy 
        //        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

        //        currentEnemyCount++;
        //    }
        //}

        if (enemyPool.Count > 0 && currentEnemyCount < maxEnemies)
        {
            // Ricicla un nemico dall'Object Pool
            GameObject enemy = enemyPool.Dequeue();
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = Quaternion.identity;
            enemy.SetActive(true);

            currentEnemyCount++;
        }
    }

    //public void OnEnemyDestroyed()
    //{
    //    //be careful if you don't use this the counter won't update properly and you may reach a situation where no more enemies are spawning even though some have been eliminated.
    //    currentEnemyCount--;
    //}

    public void OnEnemyDestroyed(GameObject enemy)
    {
        // Quando un nemico viene distrutto, disattivalo e rimettilo nell'Object Pool
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);

        // Aggiorna il contatore dei nemici attivi
        currentEnemyCount--;
    }
}

