using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerTop : MonoBehaviour, IDamageable, IPlayer
{
    // Start is called before the first frame update
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private GameObject[] muzzles;
    [SerializeField] private GameObject ship;

    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float baseSpeed;

    [SerializeField] private float health = 100.0f;
    [SerializeField] private float maxHealth = 100.0f;
    [SerializeField] private int keyCount = 0;

    private PlayerControllerTop playerController;
    private Rigidbody rb;

    void Start()
    {
        playerController = GetComponent<PlayerControllerTop>();
        rb = GetComponent<Rigidbody>();

        speed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        LookAtMouse();
        Boost(); 
        Move();
        Flip();
    }

    private void LookAtMouse()
    {
        Vector2 mouseScreenPos = playerController.GetMousePosition2D();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, mainCamera.transform.position.y));

        if (IsNearThan(mouseWorldPos, transform.position, 0.5f)) return;

        Vector3 directionToMouse = mouseWorldPos - transform.position;
        directionToMouse.y = 0;

        Quaternion targetRotation = Quaternion.LookRotation(directionToMouse);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }

    private bool IsNearThan(Vector3 a, Vector3 b, float distance)
    {
        return (b - a).magnitude <= distance;
    }

    private void Move()
    {
        Vector2 input = playerController.GetMovement();

        Vector3 movementDirection = new Vector3(input.x, 0f, input.y);
        movementDirection.Normalize();


        Vector3 movement = movementDirection.x * transform.right + movementDirection.z * transform.forward;

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    private void Boost()
    {
        if (playerController.GetShoot() > 0)
        {
            if (true)
            {
                speed = Mathf.Lerp(speed, maxSpeed, Time.fixedDeltaTime);
            }
        }
        else
        {
            speed = Mathf.Lerp(speed, baseSpeed, Time.fixedDeltaTime);
        }
    }

    private void Flip()
    {
        if (playerController.GetFlip() > 0)
        {
            ship.transform.Rotate(0, 0, 100.0f * Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
    }
    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float currentHealth)
    {
        health = currentHealth;
    }
    
    public float GetNormalaizedHealth()
    {
        return health / maxHealth;
    }

    public int GetKeyCount()
    {
        return keyCount;
    }

    public void SetKeyCount(int currentKeyCount)
    {
        keyCount = currentKeyCount;
    }
}
