using System.Collections.Generic;
using UnityEngine;

public class AsteroidsMgr : MonoBehaviour
{
    [SerializeField] private GameObject asteroidPrefab;    
    [SerializeField] private GameObject planet;           
    [SerializeField] private Transform player;
    [SerializeField] private Transform  camera;

    [SerializeField] private float spawnRadius = 50f;      // Radius within which asteroids spawn
    [SerializeField] private float spawnAngle = 90f; // Maximum angle within which asteroids can spawn
    [SerializeField] private float forwardAngle = 60f;       // Camera forward angle to activate asteroids
    [SerializeField] private float minDistanceFromPlayer = 5f;  // Minimum distance from the player for asteroids to spawn
    [SerializeField] private float minDistanceFromPlanet = 5f;
    [SerializeField] private int asteroidCount = 10;       // Number of asteroids to spawn
    [SerializeField] private int maxAsteroidCount = 50;  // Maximum number of active asteroids

    [SerializeField] private float minAsteroidSize = 0.5f;  
    [SerializeField] private float maxAsteroidSize = 3f;

    private bool playerInSpawnRange = false; 

    private List<GameObject> activeAsteroids = new List<GameObject>();
    private List<GameObject> inactiveAsteroids = new List<GameObject>();


    private void Start()
    {
        SpawnAsteroidsAroundPlanet();      
    }

    private void Update()
    {
        CheckPlayerDistance();
        UpdateAsteroidsVisibility();
    }

    //    private void SpawnAsteroids()
    //    {

    //        while (activeAsteroids.Count < maxAsteroidCount)
    //        {
    //            Vector3 spawnPosition = GenerateValidSpawnPosition();

    //            if (spawnPosition != Vector3.zero)
    //            {
    //                Quaternion randomRotation = Random.rotation;  // Rotazione casuale
    //                GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, randomRotation);
    //                activeAsteroids.Add(asteroid);

    //            }
    //            else
    //            {
    //                Debug.LogWarning("Failed to generate a valid spawn position.");
    //                break; // Interrompe il ciclo se non riesce a generare una posizione valida
    //            }
    //        }
    //    }

    //    private Vector3 GenerateValidSpawnPosition()
    //    {
    //        for (int i = 0; i < 500; i++) // Limited attempts to avoid infinite loops
    //        {
    //            // Generates a random location within a sphere around the planet
    //            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
    //            Vector3 spawnPosition = planet.transform.position + randomPosition;


    //            float distanceToPlayer = Vector3.Distance(spawnPosition, player.transform.position);

    //            // Check if the spawn location is valid
    //            if (distanceToPlayer >= minDistanceFromPlayer)
    //            {

    //                Vector3 directionToSpawn = (spawnPosition - player.transform.position).normalized;

    //                // Calculate the angle between the player's forward and the direction towards the spawn position
    //                float angleToSpawn = Vector3.Angle(camera.forward, directionToSpawn);

    //                if (angleToSpawn <= spawnAngle)
    //                {

    //                    return spawnPosition;

    //                }

    //            }

    //        }

    //        // Return Vector3.zero if the position is invalid after the maximum number of attempts
    //        Debug.LogWarning("Failed to find a valid spawn position.");
    //        return Vector3.zero;
    //    }

    //    private void UpdateAsteroids()
    //    {
    //        List<GameObject> asteroidsToRemove = new List<GameObject>();

    //        foreach (GameObject asteroid in activeAsteroids)
    //        {
    //            if (asteroid == null) continue;

    //            bool isInEnableCone = player.IsInEnableCone(asteroid.transform);
    //            bool isInDisableCone = player.IsInDisableCone(asteroid.transform);

    //            if (isInDisableCone)
    //            {
    //                // Destroy asteroids in the disable cone
    //                asteroidsToRemove.Add(asteroid);
    //            }
    //            else if (!isInEnableCone)
    //            {
    //                // Optionally, you can deactivate asteroids outside the enable cone if needed
    //                asteroid.SetActive(false);
    //            }
    //        }

    //        foreach (GameObject asteroid in asteroidsToRemove)
    //        {
    //            activeAsteroids.Remove(asteroid);
    //            Destroy(asteroid);
    //        }

    //        SpawnAsteroids();
    //    }


    //    //// To view the spawn radius and minimum distance in the editor
    //    //private void OnDrawGizmosSelected()
    //    //{
    //    //    Gizmos.color = Color.yellow;
    //    //    Gizmos.DrawWireSphere(planet.transform.position, spawnRadius);

    //    //    // Draw a sphere around the player to display the minimum spawn distance
    //    //    if (player != null)
    //    //    {
    //    //        Gizmos.color = Color.red;
    //    //        Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);


    //    //        Gizmos.color = Color.blue;
    //    //        Vector3 forward = camera != null ? camera.forward : player.forward;
    //    //        Gizmos.DrawRay(player.position, forward * spawnRadius);
    //    //        Gizmos.DrawWireSphere(player.position + forward * spawnRadius, 2f);

    //    //    }
    //    //}

    //    //private void Start()
    //    //{
    //    //    PrecalculateSpawnPositions();
    //    //    SpawnAsteroids();
    //    //}

    //    //private void Update()
    //    //{
    //    //    UpdateAsteroids();
    //    //}

    //    //private void PrecalculateSpawnPositions()
    //    //{
    //    //    int numPositions = 1000;  // Numero di posizioni precalcolate
    //    //    for (int i = 0; i < numPositions; i++)
    //    //    {
    //    //        Vector3 randomDirection = Random.onUnitSphere; // Direzione casuale sulla superficie di una sfera unitaria
    //    //        Vector3 spawnPosition = planet.transform.position + randomDirection * spawnRadius;

    //    //        precalculatedSpawnPositions.Add(spawnPosition);
    //    //    }
    //    //}

    //    //private void SpawnAsteroids()
    //    //{
    //    //    while (activeAsteroids.Count < maxAsteroidCount)
    //    //    {
    //    //        Vector3 spawnPosition = GenerateValidSpawnPosition();

    //    //        if (spawnPosition != Vector3.zero)
    //    //        {
    //    //            Quaternion randomRotation = Random.rotation;
    //    //            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, randomRotation);
    //    //            activeAsteroids.Add(asteroid);
    //    //        }
    //    //        else
    //    //        {
    //    //            Debug.LogWarning("Failed to generate a valid spawn position.");
    //    //            break;
    //    //        }
    //    //    }
    //    //}

    //    //private Vector3 GenerateValidSpawnPosition()
    //    //{
    //    //    for (int i = 0; i < precalculatedSpawnPositions.Count; i++)
    //    //    {
    //    //        Vector3 spawnPosition = precalculatedSpawnPositions[Random.Range(0, precalculatedSpawnPositions.Count)];
    //    //        float distanceToPlayer = Vector3.Distance(spawnPosition, player.position);

    //    //        if (distanceToPlayer >= minDistanceFromPlayer)
    //    //        {
    //    //            Vector3 directionToSpawn = (spawnPosition - player.position).normalized;
    //    //            float angleToSpawn = Vector3.Angle(camera.forward, directionToSpawn);

    //    //            if (angleToSpawn <= spawnAngle)
    //    //            {
    //    //                return spawnPosition;
    //    //            }
    //    //        }
    //    //    }

    //    //    Debug.LogWarning("Failed to find a valid spawn position.");
    //    //    return Vector3.zero;
    //    //}

    //    //private void UpdateAsteroids()
    //    //{
    //    //    List<GameObject> asteroidsToRemove = new List<GameObject>();

    //    //    foreach (GameObject asteroid in activeAsteroids)
    //    //    {
    //    //        if (asteroid == null) continue;

    //    //        Vector3 directionToAsteroid = (asteroid.transform.position - player.position).normalized;
    //    //        float angleAsteroid = Vector3.Angle(camera.forward, directionToAsteroid);

    //    //        if (angleAsteroid > spawnAngle)
    //    //        {
    //    //            asteroidsToRemove.Add(asteroid);
    //    //        }
    //    //    }

    //    //    foreach (GameObject asteroid in asteroidsToRemove)
    //    //    {
    //    //        activeAsteroids.Remove(asteroid);
    //    //        Destroy(asteroid);
    //    //    }

    //    //    SpawnAsteroids();
    //    //}


    //    private void OnDrawGizmosSelected()
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawWireSphere(planet.transform.position, spawnRadius);

    //        // Draw a sphere around the player to display the minimum spawn distance
    //        if (player != null)
    //        {
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawWireSphere(player.transform.position, minDistanceFromPlayer);

    //            Gizmos.color = Color.blue;
    //            Vector3 forward = camera != null ? camera.forward : player.transform.forward;
    //            Gizmos.DrawRay(player.transform.position, forward * spawnRadius);
    //            Gizmos.DrawWireSphere(player.transform.position + forward * spawnRadius, 2f);
    //        }
    //    }


    private void SpawnAsteroids()
    {
        while (activeAsteroids.Count + inactiveAsteroids.Count < maxAsteroidCount)
        {
            Vector3 spawnPosition = GenerateValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                Quaternion randomRotation = Random.rotation;
                GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, randomRotation);

                // Random Size
                float randomSize = Random.Range(minAsteroidSize, maxAsteroidSize);
                asteroid.transform.localScale = Vector3.one * randomSize;

                
                asteroid.SetActive(false);
                inactiveAsteroids.Add(asteroid);
            }
            else
            {               
                break;
            }
        }
    }

    private void SpawnAsteroidsAroundPlanet()
    {
        while (activeAsteroids.Count + inactiveAsteroids.Count < maxAsteroidCount)
        {
            Vector3 spawnPosition = GenerateValidSpawnPositionAroundPlanet();

            if (spawnPosition != Vector3.zero)
            {
                Quaternion randomRotation = Random.rotation;
                GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, randomRotation);

                // Disattiva inizialmente gli asteroidi
                asteroid.SetActive(false);
                inactiveAsteroids.Add(asteroid);
            }
            else
            {
                Debug.LogWarning("Failed to generate a valid spawn position.");
                break;
            }
        }
    }

    // Genera una posizione di spawn valida
    private Vector3 GenerateValidSpawnPosition()
    {
        for (int i = 0; i < 500; i++) 
        {
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPosition = planet.transform.position + randomPosition;

            float distanceToPlayer = Vector3.Distance(spawnPosition, player.transform.position);
            float distanceToPlanet = Vector3.Distance(spawnPosition, planet.transform.position);

            if (distanceToPlayer >= minDistanceFromPlayer && distanceToPlanet >= minDistanceFromPlanet)
            {
                Vector3 directionToSpawn = (spawnPosition - player.transform.position).normalized;
                float angleToSpawn = Vector3.Angle(camera.forward, directionToSpawn);

                if (angleToSpawn > forwardAngle) // Spawn only outside the player's forward
                {
                    return spawnPosition;
                }
            }
        }

        return Vector3.zero;
    }

    private Vector3 GenerateValidSpawnPositionAroundPlanet()
    {
        for (int i = 0; i < 500; i++) // Evita loop infiniti con tentativi limitati
        {
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPosition = planet.transform.position + randomPosition;

            float distanceToPlayer = Vector3.Distance(spawnPosition, player.transform.position);
            float distanceToPlanet = Vector3.Distance(spawnPosition, planet.transform.position);

            // Verifica che lo spawn sia alla distanza minima dal pianeta (ignora la distanza dal player)
            if (distanceToPlanet >= minDistanceFromPlanet)
            {
                return spawnPosition;
            }
        }

        return Vector3.zero;
    }

    // Gestione della visibilità degli asteroidi
    private void UpdateAsteroidsVisibility()
    {
        List<GameObject> asteroidsToActivate = new List<GameObject>();
        List<GameObject> asteroidsToDeactivate = new List<GameObject>();

        // Controlla asteroidi inattivi per attivarli quando sono nel forward
        foreach (GameObject asteroid in inactiveAsteroids)
        {
            if (asteroid == null) continue;

            Vector3 directionToAsteroid = (asteroid.transform.position - player.transform.position).normalized;
            float angleToAsteroid = Vector3.Angle(camera.forward, directionToAsteroid);

            if (angleToAsteroid <= forwardAngle)
            {
                // L'asteroide è nel campo visivo del player, quindi attivalo
                asteroid.SetActive(true);
                asteroidsToActivate.Add(asteroid);
            }
        }

        // Controlla asteroidi attivi per disattivarli quando escono dal forward
        foreach (GameObject asteroid in activeAsteroids)
        {
            if (asteroid == null) continue;

            Vector3 directionToAsteroid = (asteroid.transform.position - player.transform.position).normalized;
            float angleToAsteroid = Vector3.Angle(camera.forward, directionToAsteroid);

            if (angleToAsteroid > forwardAngle)
            {
                // L'asteroide non è più nel forward, disattivalo
                asteroid.SetActive(false);
                asteroidsToDeactivate.Add(asteroid);
            }
        }

        // Aggiorna le liste degli asteroidi
        foreach (GameObject asteroid in asteroidsToActivate)
        {
            inactiveAsteroids.Remove(asteroid);
            activeAsteroids.Add(asteroid);
        }

        foreach (GameObject asteroid in asteroidsToDeactivate)
        {
            activeAsteroids.Remove(asteroid);
            inactiveAsteroids.Add(asteroid);
        }
    }


    private void CheckPlayerDistance() 
    {
        float distanceToPlayer = Vector3.Distance(player.position, planet.transform.position);

        if(distanceToPlayer <= spawnRadius && !playerInSpawnRange) 
        {
            playerInSpawnRange = true;
            SpawnAsteroids();

        }
        else if(distanceToPlayer > spawnRadius && playerInSpawnRange) 
        {
            playerInSpawnRange = false;
        }
    }

    // Gizmo to display spawn radius and minimum distance from player
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(planet.transform.position, spawnRadius);

        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.transform.position, minDistanceFromPlayer);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(planet.transform.position, minDistanceFromPlanet);

            Gizmos.color = Color.blue;
            Vector3 forward = camera != null ? camera.forward : player.transform.forward;
            Gizmos.DrawRay(player.transform.position, forward * spawnRadius);
        }
    }

}
