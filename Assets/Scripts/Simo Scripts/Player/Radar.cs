using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField] private Transform player; // Reference to the player's transform
    [SerializeField] private GameObject arrowPrefab; // Prefab for the arrow indicator
    [SerializeField] private GameObject inViewCrosshairPrefab; // Prefab for the crosshair when the enemy is in view
    [SerializeField] private GameObject lockedOnCrosshairPrefab; //Prefab for the crosshair when the enemy is locked on
    private GameObject currentLockedEnemy; // Track the currently locked-on enemy
    private float detectionRange = 100f; // Range within which to detect enemies
    private float fieldOfView = 60f; // Field of view for detecting enemies


    private List<GameObject> enemiesInScene = new List<GameObject>();


    void Start()
    {
        // Assicurati che la lista dei nemici sia inizializzata
        enemiesInScene = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
    }

    void Update()
    {
        DetectEnemies();
        ManageIndicators();
    }

    void DetectEnemies()
    {
        enemiesInScene.Clear();
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            float distance = Vector3.Distance(player.position, enemy.transform.position);
            if (distance <= detectionRange)
            {
                Vector3 directionToEnemy = (enemy.transform.position - player.position).normalized;
                float angle = Vector3.Angle(player.forward, directionToEnemy);

                if (angle <= fieldOfView / 2)
                {
                    enemiesInScene.Add(enemy);
                }
            }
        }
    }

    void ManageIndicators()
    {
        foreach (var enemy in enemiesInScene)
        {
            Vector3 directionToEnemy = (enemy.transform.position - player.position).normalized;
            float angle = Vector3.Angle(player.forward, directionToEnemy);
            bool isFacingEnemy = angle <= fieldOfView  * 0.5f;

            if (isFacingEnemy)
            {
                if (currentLockedEnemy == enemy)
                {
                    ShowLockedOnCrosshair(enemy);
                }
                else
                {
                    ShowInViewCrosshair(enemy);
                }
            }
            else
            {
                ShowArrow(enemy);
            }
        }
    }

    void ShowArrow(GameObject enemy)
    {
        // Create and position arrow indicator
        Vector3 directionToEnemy = (enemy.transform.position - player.position).normalized;
        GameObject arrow = Instantiate(arrowPrefab, GetScreenPosition(enemy), Quaternion.LookRotation(directionToEnemy));
        arrow.transform.SetParent(transform); // Optional: Set parent to the radar for easier management
    }

    void ShowInViewCrosshair(GameObject enemy)
    {
        // Position crosshair indicator at enemy's position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(enemy.transform.position);
        GameObject crosshair = Instantiate(inViewCrosshairPrefab, screenPosition, Quaternion.identity);
        crosshair.transform.SetParent(transform); // Optional: Set parent to the radar for easier management
    }

    void ShowLockedOnCrosshair(GameObject enemy)
    {
        // Position crosshair indicator at enemy's position
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(enemy.transform.position);
        GameObject crosshair = Instantiate(lockedOnCrosshairPrefab, screenPosition, Quaternion.identity);
        crosshair.transform.SetParent(transform); // Optional: Set parent to the radar for easier management
    }

    Vector3 GetScreenPosition(GameObject enemy)
    {
        // Convert world position to screen position
        return Camera.main.WorldToScreenPoint(enemy.transform.position);
    }

    public void LockOnEnemy(GameObject enemy)
    {
        // Set the current locked-on enemy
        currentLockedEnemy = enemy;
    }

    public void UnlockEnemy()
    {
        // Clear the locked-on enemy
        currentLockedEnemy = null;
    }
}
