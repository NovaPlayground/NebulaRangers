using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject[] enemyPrefabs; 
    [SerializeField] private Transform[] spawnPoints;  
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private int maxEnemiesInScene = 5;
    [SerializeField] private int totalEnemiesToSpawn = 10;
    
    private float timer;
    private int spawnedEnemiesCount; // Counter for enemies already spawned
    private int currentEnemyCount; // Counter for the number of enemies currently active


    // KEY

    [SerializeField] private GameObject keyPrefab; 
    [SerializeField] private Transform keySpawnPoint; // Punto dove la chiave apparirà

    // Start is called before the first frame update
    void Start()
    {
        timer = spawnInterval;
        currentEnemyCount = 0;
        spawnedEnemiesCount = 0;

        
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

    private void Spawnkey() 
    {
        if(keyPrefab != null && keySpawnPoint != null) 
        {
            Instantiate(keyPrefab,keySpawnPoint.position, Quaternion.identity);
            Debug.Log("Key spawned at position: " + keySpawnPoint.position);
        }
    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        currentEnemyCount--;
        Destroy(enemy);
      
        if(currentEnemyCount == 0 && spawnedEnemiesCount == totalEnemiesToSpawn) 
        {
            Spawnkey();
        }
    }
}

