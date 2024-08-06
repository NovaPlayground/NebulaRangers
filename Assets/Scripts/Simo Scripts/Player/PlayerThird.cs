using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows;

public class PlayerThird : MonoBehaviour, IDamageable
{
    // CAMERA
    [SerializeField] private Camera mainCamera;

    // MOVEMENT 
    [SerializeField] private float lookRateSpeed = 90f;
    [SerializeField] private float rollSpeed = 90f;
    [SerializeField] private float rollAcceleration = 3f;

    [SerializeField] private float forwardSpeed = 10f;
    [SerializeField] private float strafeSpeed = 8f;
    [SerializeField] private float hoverSpeed = 5f;

    [SerializeField] private float forwardAcceleration = 5f;
    [SerializeField] private float strafeAcceleration = 5f;
    [SerializeField] private float hoverAcceleration = 5f;

    // SHOOT 
    [SerializeField] private DebugNormalBullet debugNormalBullet;
    [SerializeField] private float shootDelay;
    [SerializeField] private GameObject[] muzzles;

    //HEALTH
    [SerializeField] private float health = 100f;
    private float maxHealth = 100f; 

    // MOVEMENT 
    private float activeForwardSpeed;
    private float activeStrafeSpeed;
    private float activeHoverSpeed;

    private float rotationAngle = 0.0f;

    private PlayerControllerThird playerController;
    private Rigidbody rbThird;


    // SHOOT
    private float shootCooldown;

    // COIN 
    private int coinCount = 0;



    // BARRIER 


    private void Start()
    {
        rbThird = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerControllerThird>();
        shootCooldown = 0.0f;
    }

    private void FixedUpdate()
    {
        Rotate();
        Move();
        MoveUpDown();
        Shoot();
    }

    // MOVEMENT

    private void Rotate()
    {
        
        Vector2 mousePos = playerController.GetMouseLook();

        // Calculate the normalized distance of the mouse from the center of the screen
        Vector2 mouseDistance = new Vector2(mousePos.x - Screen.width * 0.5f, mousePos.y - Screen.height * 0.5f) / Screen.height;

        // Calculate rotation angles for pitch (X-axis) and yaw (Y-axis)
        float pitch = -mouseDistance.y * lookRateSpeed * Time.fixedDeltaTime;
        float yaw = mouseDistance.x * lookRateSpeed * Time.fixedDeltaTime;

        // Calculate pitch and yaw rotations as Quaternions
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);

        // Combine current rotations with the new pitch and yaw rotations
        Quaternion targetRotation = transform.rotation * yawRotation * pitchRotation;

        // Apply the new rotation to the object using a smooth interpolation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rollSpeed * Time.fixedDeltaTime);

        // If you want to add roll rotation, apply a Quaternion for roll here
        float newRollInput = playerController.GetRoll();
        Quaternion rollRotation = Quaternion.Euler(0f, 0f, newRollInput * rollSpeed * Time.fixedDeltaTime);

       
        transform.rotation *= rollRotation;

    }

    private Quaternion Roll() 
    {
        
        float rollInput = playerController.GetRoll();

        // Calculate the amount of roll rotation based on input and speed
        rotationAngle += rollInput * rollSpeed * Time.fixedDeltaTime;

        // Create a Quaternion representing the new rotation with roll applied
        Quaternion newRotation = Quaternion.Euler(0f, 0f, rotationAngle);

        // Interpolate between the current rotation and the new rotation over time
        newRotation = Quaternion.Lerp(transform.rotation, newRotation, Time.fixedDeltaTime);

        return newRotation;

    }

    private void Move()
    {
        Vector2 input = playerController.GetMovement();

        Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

        moveDirection.Normalize();

        // Calculate the movement vector based on the normalized direction and current orientation of the object
        Vector3 movement = moveDirection.x * transform.right + moveDirection.z * transform.forward;

        // Calculate the target velocity based on the movement direction and forward speed
        Vector3 targetVelocity = moveDirection * forwardSpeed * Time.fixedDeltaTime;

        // Extract forward (Y-axis) and strafe (X-axis) inputs from the movement input
        float inputForward = input.y;
        float inputStrafe = input.x;

        // Smoothly interpolate forward speed based on input and acceleration
        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, inputForward * forwardSpeed, forwardAcceleration * Time.fixedDeltaTime);

        // Smoothly interpolate strafe speed based on input and acceleration
        activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, inputStrafe * strafeSpeed, strafeAcceleration * Time.fixedDeltaTime);

        // Smoothly decrease hover speed assuming it doesn't affect movement
        activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, 0f, hoverAcceleration * Time.deltaTime);

        // Move the Rigidbody rbThird to its new position based on the calculated movement and forward speed
        rbThird.MovePosition(rbThird.position + movement * forwardSpeed * Time.fixedDeltaTime);

    }

    private void MoveUpDown() 
    {
        float input = playerController.GetMovementUpDown();

        Vector3 movement = input * transform.up;

        rbThird.MovePosition(rbThird.position + movement * forwardSpeed * Time.fixedDeltaTime);
    }


    // SHOOT 
    private void Shoot()
    {

        if (playerController.GetShoot() > 0)
        {
            if (shootCooldown >= shootDelay)
            {
                for (int i = 0; i < muzzles.Length; i++)
                {
                    Instantiate(debugNormalBullet, muzzles[i].transform.position, transform.rotation);
                }

                shootCooldown = 0;
            }
            else
            {
                shootCooldown += Time.fixedDeltaTime;
            }
        }
        else
        {
            shootCooldown = shootDelay;
        }
    }


    // DAMAGE

    public void TakeDamage(float damage) 
    {
        health -= damage;

        if (health <= 0f) 
        {
            Destroy(gameObject);
            Debug.Log("Player has died.");
        }
    }

    // PICKALE OBJ
    private void AddCoin(int amount) 
    {
        coinCount += amount;

        Debug.Log("Coins collected: " + coinCount);
    }

    private void AddHealth(float amount) 
    {
        health += amount;
        
        if (health > maxHealth)
        {
            health = maxHealth; 
        }

        Debug.Log("Picked up health! Added " + amount + " health. Current health: " + health);
    }

    private void OnTriggerEnter(Collider other)
    {
        IPickable pickable = other.GetComponent<IPickable>();
            
        if (pickable is Coin coin)
        {
            // add the coin value to player score
            AddCoin(coin.Value);
            
            pickable.PickUp(gameObject);
        }
        else if (pickable is Health Health)
        {
            // add the health value to player 
            AddHealth(Health.Value);
        }
    }



}


