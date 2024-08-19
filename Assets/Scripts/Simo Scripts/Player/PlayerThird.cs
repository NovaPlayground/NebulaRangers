using System.Threading;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerThird : MonoBehaviour, IDamageable
{
    // CAMERA
    [SerializeField] private Camera mainCamera;

    // RIGIDBODY 
    private Rigidbody rbThird;

    // BOX COLLIDER

    private BoxCollider playerCollider;

    // CONTROLLER
    private PlayerControllerThird playerController;

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

    private float activeForwardSpeed;
    private float activeStrafeSpeed;
    private float activeHoverSpeed;

    private float rotationAngle = 0.0f;

    // SHOOT 
    // MACHINEGUN
    [SerializeField] private DebugNormalBullet debugNormalBullet;
    [SerializeField] private float shootDelay;
    [SerializeField] private GameObject[] muzzles;

    private float shootCooldown;

    // MISSILE 
    [SerializeField] private Missile missilePrefab;
    [SerializeField] private Transform missileSpawnPoint;
    //[SerializeField] private float lockOnRange = 50f; //Snap radius
    [SerializeField] private float missileCooldown = 5f;

    private float missileCooldownTimer = 0f; // Missile cooldown timer
    private Transform lockedTarget;

    // CONE VISION
    [SerializeField] private float visionConeAngleHorizontal = 45f; 
    [SerializeField] private float visionConeAngleVertical = 30f; 
    [SerializeField] private float visionConeDistance = 50f; 

    //HEALTH
    [SerializeField] private float health = 100f;

    private float maxHealth = 100f;

    // SHIELD
    [SerializeField] private Shield shield;
    [SerializeField] private float barrierMaxHealth = 100f;
    [SerializeField] private float reenableColliderTime = 1.5f; // Time to wait before re-enabling the collider
    [SerializeField] private float immuneTimeAfterShield = 0.5f; // Time during which the player is immune to damage after shield is destroyed
    private float reenableColliderTimer = 0f; // Tracks the remaining time to re-enable the collider
    private float immuneTimer = 0f; // Timer to handle immune state
    private bool isColliderDisabled = false; 
    private bool isImmune = false; 

    // KEY
    private int keyCount = 0;


    


    private void Start()
    {
        rbThird = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerControllerThird>();
        playerCollider = GetComponent<BoxCollider>();
        shootCooldown = 0.0f;

        // SHIELD INIT
        if(shield != null) 
        {
            shield.Initialize(barrierMaxHealth);
            ActivateShield();
        }
    }

    private void FixedUpdate()
    {
        Rotate();
        Move();
        MoveUpDown();
        Shoot();
        ShootMissile();
        TargetWithinCone();
    }

    private void Update()
    {
        
    }


    // MOVEMENT LOGIC
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



    // SHOOT LOGIC  
    // MACHINEGUN
    private void Shoot()
    {

        if (playerController.GetShoot() > 0)
        {
            if (shootCooldown >= shootDelay)
            {
                foreach (var muzzle in muzzles)
                {
                    Instantiate(debugNormalBullet, muzzle.transform.position, transform.rotation);
                }

                shootCooldown = 0;
            }
            else
            {
                shootCooldown += Time.deltaTime;
            }
        }
        else
        {
            shootCooldown = shootDelay;
        }
    }

    // MISSILE 
    // RIMANE DA DEBUGGARE I RAYCAST, PERCHè LA CONDIZIONE IF NON VIENE MAI SODDISFATTA MA ENTRA SOLO NELL'ELSE
    private void TargetWithinCone()
    {
        // check if target is outside the player's cone vision
        // if not, the target is not locked anymore

        if (lockedTarget != null)
        {
            // calcuate the direction toward target
            Vector3 directionTarget = (lockedTarget.position - transform.position).normalized;

            // calcuate the distance to target
            float distanceToTarget = Vector3.Distance(transform.position, lockedTarget.position);

            // Calcolate angle horizontal & vertical between the player's direction and the direction towards the target

            //float angleHorizontal = Vector3.Angle(transform.forward, directionTarget);
            //float angleVertical = Vector3.Angle(Vector3.ProjectOnPlane(directionTarget, transform.up), transform.forward);

            float angleHorizontal = Mathf.Atan2(directionTarget.z, directionTarget.x) * Mathf.Rad2Deg;
            float forwardAngleHorizontal = Mathf.Atan2(transform.forward.z, transform.forward.x) * Mathf.Rad2Deg;
            float deltaAngleHorizontal = Mathf.DeltaAngle(forwardAngleHorizontal, angleHorizontal);

            float angleVertical = Mathf.Atan2(directionTarget.y, new Vector2(directionTarget.x, directionTarget.z).magnitude) * Mathf.Rad2Deg;
            float forwardAngleVertical = Mathf.Atan2(transform.forward.y, new Vector2(transform.forward.x, transform.forward.z).magnitude) * Mathf.Rad2Deg;
            float deltaAngleVertical = Mathf.DeltaAngle(forwardAngleVertical, angleVertical);

            // Check if the target is within the cone of vision
            //bool isWithinVisionCone = angleHorizontal <= visionConeAngleHorizontal * 0.5f && angleVertical <= visionConeAngleVertical * 0.5f;
            bool isWithinVisionCone = Mathf.Abs(deltaAngleHorizontal) <= visionConeAngleHorizontal * 0.5f &&
                                  Mathf.Abs(deltaAngleVertical) <= visionConeAngleVertical * 0.5f;

            // Check if the target is within the maximum viewing distance
            bool isWithinDistance = distanceToTarget <= visionConeDistance;
   

            Color rayColor;

            if (isWithinVisionCone && isWithinDistance)
            {
                rayColor = Color.red; // Target inside the cone of vision and distance -> Raycast red
                Debug.Log("Target still within vision cone and distance: " + lockedTarget.name);
            }
            else
            {
                // The target goes out of the cone of vision or is too far away, resets the lock
                rayColor = Color.green; // Target outside the cone of vision or too far away -> Raycast green
                lockedTarget = null;
                Debug.Log("Target lost or out of range.");
                FindNewTarget();
            }

            Debug.DrawRay(transform.position, directionTarget * distanceToTarget, rayColor, 0.1f);
        }
        else
        {
            // find a new target if there isn't no target lock
            FindNewTarget();
        }

    }

    private void FindNewTarget()
    {
        // Find all enemies within the search radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, visionConeDistance);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                // direction to target
                Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;

                // Calculate the horizontal and vertical angle between the forward of the spacecraft and the direction towards the target
                float angleHorizontal = Vector3.Angle(transform.forward, directionToTarget);
                float angleVertical = Vector3.Angle(Vector3.ProjectOnPlane(directionToTarget, transform.right), transform.forward);

                // Check if the target is within the cone of vision
                bool isWithinVisionCone = angleHorizontal <= visionConeAngleHorizontal * 0.5f && angleVertical <= visionConeAngleVertical * 0.5f;

                if (isWithinVisionCone)
                {
                    // Find the closest target that is within the cone of vision
                    float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = collider.transform;
                    }
                }
            }
        }

        // Set target if found
        if (closestTarget != null)
        {
            lockedTarget = closestTarget;
            Debug.Log("New target acquired: " + lockedTarget.name);
        }
        else
        {
            Debug.Log("No target within vision cone.");
        }
    }   

    private void SpawnMissile() 
    {
        if (missilePrefab == null)
        {
            Debug.LogError("missilePrefab is not assigned!");
            return;
        }

        Missile missile = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation);
        //Missile missileScript = missile.GetComponent<Missile>();
        missile.SetTarget(lockedTarget);
    }

    private void ShootMissile()
    {
        if (playerController.GetShootMissile() && missileCooldownTimer <= 0f )
        {
            if (lockedTarget != null) 
            {
                SpawnMissile();
                missileCooldownTimer = missileCooldown; // Reset timer

            }
            else
            {
                Debug.Log("no target find.");
            }


        }
        else
        {
            missileCooldownTimer -= Time.deltaTime; 
            missileCooldownTimer = Mathf.Max(missileCooldownTimer, 0f); // the timer does not go below zero
        }
    }



    // DAMAGE LOGIC
    public void TakeDamage(float damage) 
    {
        // SHIELD LOGIC

        if (isImmune) // Check if player is currently immune
        {
            return;
        }

        if (shield != null && shield.IsShieldActive())
        {
            if (shield.AbsorbDamage(damage))
            {
                return;
            }
            if (!shield.IsShieldActive())
            {
                if (!isColliderDisabled) // Check if collider is already disabled
                {
                    DisablePlayerCollider();
                    reenableColliderTimer = reenableColliderTime; 
                    isImmune = true; 
                    immuneTimer = immuneTimeAfterShield; 
                }
            }
        }
        if (!isColliderDisabled)
        {
            // If the barrier is not active or not enough to absorb damage, reduce player health
            health -= damage;

            if (health <= 0f)
            {
                Destroy(gameObject);
                Debug.Log("Player has died.");
            }
        }

        // Update the timer to re-enable the collider if needed
        UpdateTimers();

    }   

    // SHIELD
    public void ActivateShield()
    {
        if (shield != null)
        {
            shield.Activate();
            DisablePlayerCollider(); // Disattiva il collider del giocatore quando lo scudo è attivo
            Debug.Log("Shield activated. Player's collider disabled.");
        }
    }

    public void DeactivateShield()
    {
        if (shield != null)
        {
            shield.Deactivate();
            EnablePlayerCollider(); // Riattiva il collider del giocatore quando lo scudo è disattivato
            Debug.Log("Shield deactivated. Player's collider enabled.");
        }
    }

    private void EnablePlayerCollider()
    {
        if (playerCollider != null)
        {
            playerCollider.enabled = true;
            isColliderDisabled = false; 
        }
    }

    private void DisablePlayerCollider()
    {
        if (playerCollider != null)
        {
            playerCollider.enabled = false;
            isColliderDisabled = true; 
        }
    }

    private void UpdateTimers()
    {
        if (isColliderDisabled)
        {
            reenableColliderTimer -= Time.deltaTime; // Decrement the timer
            if (reenableColliderTimer <= 0f)
            {
                EnablePlayerCollider(); // Re-enable the collider
            }
        }

        if (isImmune)
        {
            immuneTimer -= Time.deltaTime; // Decrement the immune timer
            if (immuneTimer <= 0f)
            {
                isImmune = false; // Reset the immune state
            }
        }
    }
    private void OnDestroy()
    {
        EnablePlayerCollider();
    }




    // PICKALE OBJ
    private void AddKey(int amount) 
    {
        keyCount += amount;

        Debug.Log("Coins collected: " + keyCount);
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
            
        if (pickable is Key key)
        {
            // add the coin value to player score
            AddKey(key.Value);
            
            pickable.PickUp(gameObject);
        }
        else if (pickable is Health Health)
        {
            // add the health value to player 
            AddHealth(Health.Value);
        }
    }

}


