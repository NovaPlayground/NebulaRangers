using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidsMgr : MonoBehaviour
{
    [SerializeField] private GameObject asteroidPrefab;    
    [SerializeField] private GameObject planet;           
    [SerializeField] private Transform player;
    [SerializeField] private Transform  camera;
    [SerializeField] private int asteroidCount = 10;       // Number of asteroids to spawn
    [SerializeField] private float spawnRadius = 50f;      // Radius within which asteroids spawn
    [SerializeField] private float minDistanceFromPlayer = 5f;  // Minimum distance from the player for asteroids to spawn
    [SerializeField] private float spawnAngle = 90f; // Maximum angle within which asteroids can spawn
    [SerializeField] private int maxAsteroidCount = 50;  // Maximum number of active asteroids

    private List<GameObject> activeAsteroids = new List<GameObject>();
    private List<Vector3> precalculatedSpawnPositions = new List<Vector3>();

    private void Start()
    {
        SpawnAsteroids();
    }

    private void Update()
    {
        UpdateAsteroids();
    }

    private void SpawnAsteroids()
    {

        while (activeAsteroids.Count < maxAsteroidCount)
        {
            Vector3 spawnPosition = GenerateValidSpawnPosition();

            if (spawnPosition != Vector3.zero)
            {
                Quaternion randomRotation = Random.rotation;  // Rotazione casuale
                GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, randomRotation);
                activeAsteroids.Add(asteroid);

            }
            else
            {
                Debug.LogWarning("Failed to generate a valid spawn position.");
                break; // Interrompe il ciclo se non riesce a generare una posizione valida
            }
        }
    }

    private Vector3 GenerateValidSpawnPosition()
    {
        for (int i = 0; i < 500; i++) // Limited attempts to avoid infinite loops
        {
            // Generates a random location within a sphere around the planet
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            Vector3 spawnPosition = planet.transform.position + randomPosition;


            float distanceToPlayer = Vector3.Distance(spawnPosition, player.position);

            // Check if the spawn location is valid
            if (distanceToPlayer >= minDistanceFromPlayer)
            {

                Vector3 directionToSpawn = (spawnPosition - player.position).normalized;

                // Calculate the angle between the player's forward and the direction towards the spawn position
                float angleToSpawn = Vector3.Angle(camera.forward, directionToSpawn);

                if (angleToSpawn <= spawnAngle)
                {

                    return spawnPosition;

                }

            }

        }

        // Return Vector3.zero if the position is invalid after the maximum number of attempts
        Debug.LogWarning("Failed to find a valid spawn position.");
        return Vector3.zero;
    }

    private void UpdateAsteroids()
    {
        List<GameObject> asteroidsToRemove = new List<GameObject>();

        foreach (GameObject asteroid in activeAsteroids)
        {
            if (asteroid == null) continue;

            Vector3 directionToAsteroid = (asteroid.transform.position - player.position).normalized;
            float angleAsteroid = Vector3.Angle(camera.forward, directionToAsteroid);

            if (angleAsteroid > spawnAngle)
            {
                asteroidsToRemove.Add(asteroid);
            }
        }

        foreach (GameObject asteroid in asteroidsToRemove)
        {
            activeAsteroids.Remove(asteroid);
            Destroy(asteroid);
        }

        SpawnAsteroids();
    }


    //// To view the spawn radius and minimum distance in the editor
    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.color = Color.yellow;
    //    Gizmos.DrawWireSphere(planet.transform.position, spawnRadius);

    //    // Draw a sphere around the player to display the minimum spawn distance
    //    if (player != null)
    //    {
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);


    //        Gizmos.color = Color.blue;
    //        Vector3 forward = camera != null ? camera.forward : player.forward;
    //        Gizmos.DrawRay(player.position, forward * spawnRadius);
    //        Gizmos.DrawWireSphere(player.position + forward * spawnRadius, 2f);

    //    }
    //}

    //private void Start()
    //{
    //    PrecalculateSpawnPositions();
    //    SpawnAsteroids();
    //}

    //private void Update()
    //{
    //    UpdateAsteroids();
    //}

    //private void PrecalculateSpawnPositions()
    //{
    //    int numPositions = 1000;  // Numero di posizioni precalcolate
    //    for (int i = 0; i < numPositions; i++)
    //    {
    //        Vector3 randomDirection = Random.onUnitSphere; // Direzione casuale sulla superficie di una sfera unitaria
    //        Vector3 spawnPosition = planet.transform.position + randomDirection * spawnRadius;

    //        precalculatedSpawnPositions.Add(spawnPosition);
    //    }
    //}

    //private void SpawnAsteroids()
    //{
    //    while (activeAsteroids.Count < maxAsteroidCount)
    //    {
    //        Vector3 spawnPosition = GenerateValidSpawnPosition();

    //        if (spawnPosition != Vector3.zero)
    //        {
    //            Quaternion randomRotation = Random.rotation;
    //            GameObject asteroid = Instantiate(asteroidPrefab, spawnPosition, randomRotation);
    //            activeAsteroids.Add(asteroid);
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Failed to generate a valid spawn position.");
    //            break;
    //        }
    //    }
    //}

    //private Vector3 GenerateValidSpawnPosition()
    //{
    //    for (int i = 0; i < precalculatedSpawnPositions.Count; i++)
    //    {
    //        Vector3 spawnPosition = precalculatedSpawnPositions[Random.Range(0, precalculatedSpawnPositions.Count)];
    //        float distanceToPlayer = Vector3.Distance(spawnPosition, player.position);

    //        if (distanceToPlayer >= minDistanceFromPlayer)
    //        {
    //            Vector3 directionToSpawn = (spawnPosition - player.position).normalized;
    //            float angleToSpawn = Vector3.Angle(camera.forward, directionToSpawn);

    //            if (angleToSpawn <= spawnAngle)
    //            {
    //                return spawnPosition;
    //            }
    //        }
    //    }

    //    Debug.LogWarning("Failed to find a valid spawn position.");
    //    return Vector3.zero;
    //}

    //private void UpdateAsteroids()
    //{
    //    List<GameObject> asteroidsToRemove = new List<GameObject>();

    //    foreach (GameObject asteroid in activeAsteroids)
    //    {
    //        if (asteroid == null) continue;

    //        Vector3 directionToAsteroid = (asteroid.transform.position - player.position).normalized;
    //        float angleAsteroid = Vector3.Angle(camera.forward, directionToAsteroid);

    //        if (angleAsteroid > spawnAngle)
    //        {
    //            asteroidsToRemove.Add(asteroid);
    //        }
    //    }

    //    foreach (GameObject asteroid in asteroidsToRemove)
    //    {
    //        activeAsteroids.Remove(asteroid);
    //        Destroy(asteroid);
    //    }

    //    SpawnAsteroids();
    //}


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(planet.transform.position, spawnRadius);

        // Draw a sphere around the player to display the minimum spawn distance
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);


            Gizmos.color = Color.blue;
            Vector3 forward = camera != null ? camera.forward : player.forward;
            Gizmos.DrawRay(player.position, forward * spawnRadius);
            Gizmos.DrawWireSphere(player.position + forward * spawnRadius, 2f);

        }
    }
}
