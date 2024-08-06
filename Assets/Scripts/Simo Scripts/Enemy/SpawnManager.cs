using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject EnemyShoot;
    [SerializeField] private Transform player;
    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private float spawnDistance = 10f;
    private float spawnInterval = 5f;
    public float fieldOfViewAngle = 90f;

    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval) 
        {
            SpawnEnemyShoot();
            timer = 0; 
        }

        UpdateSpawnPointsPosition();
    }


    private void SpawnEnemyShoot() 
    {
        Vector3 spawnPosition = GetSpawnPosition();
        Instantiate(EnemyShoot, spawnPosition, Quaternion.identity);
    }


   
    private Vector3 GetSpawnPosition() 
    {
        // allow enemies to spawn in front of, to the sides of or behind the player , as long as they are outside the player’s fov

        List<Vector3> validSpawnPoints = new List<Vector3>();

        Vector3 playerPosition = player.position;
        Vector3 playerForward = player.forward;

        foreach (Transform spawnPoint in spawnPoints)
        {
            // Calculate the direction from the player to the spawn point
            Vector3 directionToSpawnPoint = (spawnPoint.position - playerPosition).normalized;

            // Check the distance from the player to the spawn point
            bool isAtValidDistance = Vector3.Distance(playerPosition, spawnPoint.position) > spawnDistance;

            // Check if the spawn point is outside the fov
            bool isOutsideFov = Vector3.Angle(playerForward, directionToSpawnPoint) > fieldOfViewAngle * 0.5f;

            // If the spawn point is at a valid distance and outside the field of view, add it to the list
            if (isAtValidDistance && isOutsideFov)
            {
                validSpawnPoints.Add(spawnPoint.position);
            }
        }

        // If there are valid points, return a random position from the valid points list
        if (validSpawnPoints.Count > 0)
        {
            return validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];
        }
        else
        {
            // If no valid points are found, return a position behind the player
            return playerPosition - playerForward * spawnDistance;
        }
    }

    void UpdateSpawnPointsPosition()
    {
        // Update each spawn point's position relative to the player
        foreach (Transform spawnPoint in spawnPoints)
        {
            spawnPoint.position = player.position + spawnPoint.localPosition; // Adjust this line if you want to position differently
        }
    }
}
