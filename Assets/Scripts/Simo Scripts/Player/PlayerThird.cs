using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerThird : MonoBehaviour, IDamageable, IPlayer
{
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
    [SerializeField] private MachinegunBullet machinegunBullet;
    [SerializeField] private float shootDelay;
    [SerializeField] private GameObject defaultMuzzlePrefab; // Default muzzle
    [SerializeField] private GameObject doubleBarrelMuzzlePrefab; // Muzzle double barrel
    [SerializeField] private Vector3 muzzleOffset = new Vector3(1.0f, 0.5f, 1.0f); //  muzzles Offset
    [SerializeField] private Vector3 doubleBarrelOffset = new Vector3(0, 0, 0); // double barrel Offset  
    private GameObject[] muzzles;

    private float shootCooldown;

    // MISSILE 
    [SerializeField] private Missile2 missilePrefab;
    [SerializeField] private Transform missileSpawnPoint;
    //[SerializeField] private float lockOnRange = 50f; //Snap radius
    [SerializeField] private float missileCooldown = 6f;

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
    [SerializeField] private float shieldHealth = 50f;
    [SerializeField] private float shieldDelay = 1.5f;
   
    private float shieldDelayTimer;
    private bool isColliderDisabled = false;


    // KEY
    private int keyCount = 0;

    // EVENTS BUFF
    public UnityEvent OnKeyLevel1;
    public UnityEvent OnKeyLevel2;
    public UnityEvent OnKeyLevel3;
    public UnityEvent OnKeyLevel4;

    // OUT OF BOUND 
    [SerializeField] private float maxDistanceFromPlanet = 650f;
    [SerializeField] private float timeOutsideBound = 7f;

    private float timeOutsideBoundTimer = 0f;
    private bool isWarningActive = false; // Status of the warning message

    // PLANET
    [SerializeField] private Transform planet;


    // UI
    [SerializeField] private TextMeshProUGUI warningMessage;  // Reference to the warning message in the UI

    // AUDIO
    [SerializeField] private AudioSource audioSourceShooting; 
    [SerializeField] private AudioSource audioSourceMoving;
    private float fadeDuration = 1.0f;

    // VFX
    [SerializeField] private GameObject explosionPrefab;

    

    private void Start()
    {

        rbThird = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerControllerThird>();
        playerCollider = GetComponent<BoxCollider>();
        shootCooldown = 0.0f;
        
        // MUZZEL
        if (defaultMuzzlePrefab != null)
        {
            muzzles = new GameObject[] { Instantiate(defaultMuzzlePrefab, transform.position + muzzleOffset, Quaternion.identity, transform) };
        }

        // SHIELD
        shield.Initialize(shieldHealth);        

        // APPLY BUFF
        SetupBuffs();
        ApplyBuffs();


        // UI
        if (warningMessage != null)
        {
            warningMessage.gameObject.SetActive(false); // set the Text not active
        }

        // PLANET 
        if (planet == null)
        {
            planet = GameObject.FindWithTag("Planet").transform;
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
        CheckDistanceFromPlanet();
        

        if (!shield.IsShieldActive()) 
        {

            shieldDelayTimer -= Time.deltaTime;

            if (shieldDelayTimer <= 0f) 
            {
               EnablePlayerCollider();
            }
        }
    }


    //  OLD MOVEMENT LOGIC
    //private void Rotate()
    //{

    //    Vector2 mousePos = playerController.GetMouseLook();

    //    // Calculate the normalized distance of the mouse from the center of the screen
    //    Vector2 mouseDistance = new Vector2(mousePos.x - Screen.width * 0.5f, mousePos.y - Screen.height * 0.5f) / Screen.height;

    //    // Calculate rotation angles for pitch (X-axis) and yaw (Y-axis)
    //    float pitch = -mouseDistance.y * lookRateSpeed * Time.fixedDeltaTime;
    //    float yaw = mouseDistance.x * lookRateSpeed * Time.fixedDeltaTime;

    //    // Calculate pitch and yaw rotations as Quaternions
    //    Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);
    //    Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);

    //    // Combine current rotations with the new pitch and yaw rotations
    //    Quaternion targetRotation = transform.rotation * yawRotation * pitchRotation;

    //    // Apply the new rotation to the object using a smooth interpolation
    //    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rollSpeed * Time.fixedDeltaTime);

    //    // If you want to add roll rotation, apply a Quaternion for roll here
    //    float newRollInput = playerController.GetRoll();
    //    Quaternion rollRotation = Quaternion.Euler(0f, 0f, newRollInput * rollSpeed * Time.fixedDeltaTime);


    //    transform.rotation *= rollRotation;

    //}

    //private Quaternion Roll()
    //{

    //    float rollInput = playerController.GetRoll();

    //    // Calculate the amount of roll rotation based on input and speed
    //    rotationAngle += rollInput * rollSpeed * Time.fixedDeltaTime;

    //    // Create a Quaternion representing the new rotation with roll applied
    //    Quaternion newRotation = Quaternion.Euler(0f, 0f, rotationAngle);

    //    // Interpolate between the current rotation and the new rotation over time
    //    newRotation = Quaternion.Lerp(transform.rotation, newRotation, Time.fixedDeltaTime);

    //    return newRotation;

    //}

    //private void Move()
    //{
    //    Vector2 input = playerController.GetMovement();

    //    Vector3 moveDirection = new Vector3(input.x, 0f, input.y);

    //    moveDirection.Normalize();

    //    // Calculate the movement vector based on the normalized direction and current orientation of the object
    //    Vector3 movement = moveDirection.x * transform.right + moveDirection.z * transform.forward;

    //    // Calculate the target velocity based on the movement direction and forward speed
    //    Vector3 targetVelocity = moveDirection * forwardSpeed * Time.fixedDeltaTime;

    //    // Extract forward (Y-axis) and strafe (X-axis) inputs from the movement input
    //    float inputForward = input.y;
    //    float inputStrafe = input.x;

    //    // Smoothly interpolate forward speed based on input and acceleration
    //    activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, inputForward * forwardSpeed, forwardAcceleration * Time.fixedDeltaTime);

    //    // Smoothly interpolate strafe speed based on input and acceleration
    //    activeStrafeSpeed = Mathf.Lerp(activeStrafeSpeed, inputStrafe * strafeSpeed, strafeAcceleration * Time.fixedDeltaTime);

    //    // Smoothly decrease hover speed assuming it doesn't affect movement
    //    activeHoverSpeed = Mathf.Lerp(activeHoverSpeed, 0f, hoverAcceleration * Time.deltaTime);

    //    // Move the Rigidbody rbThird to its new position based on the calculated movement and forward speed
    //    rbThird.MovePosition(rbThird.position + movement * forwardSpeed * Time.fixedDeltaTime);

    //    if (input.magnitude > 0 )
    //    {
    //        if (!audioSourceMoving.isPlaying)
    //        {
    //            audioSourceMoving.Play();
    //        }

    //        audioSourceMoving.volume = Mathf.Lerp(audioSourceMoving.volume, 1.0f, Time.deltaTime * fadeDuration);
    //    }
    //    else if (input.magnitude == 0)
    //    {
    //        audioSourceMoving.volume = Mathf.Lerp(audioSourceMoving.volume, 0.0f, Time.deltaTime * fadeDuration);

    //        if (audioSourceMoving.volume <= 0.1f && audioSourceMoving.isPlaying)
    //        {
    //            audioSourceMoving.Stop();
    //        }
    //    }
    //}

    private void Rotate()
    {
        Vector2 mousePos = playerController.GetMouseLook();

        // Get the distance of the mouse position from the center of the screen
        Vector2 mouseDistance = new Vector2(mousePos.x - Screen.width * 0.5f, mousePos.y - Screen.height * 0.5f) / Screen.height;

        // Calculate pitch (rotation around the X axis) and yaw (rotation around the Y axis) based on mouse movement
        float pitch = -mouseDistance.y * lookRateSpeed * Time.fixedDeltaTime;
        float yaw = mouseDistance.x * lookRateSpeed * Time.fixedDeltaTime;

        // Create rotation quaternions for pitch and yaw
        Quaternion pitchRotation = Quaternion.AngleAxis(pitch, Vector3.right);
        Quaternion yawRotation = Quaternion.AngleAxis(yaw, Vector3.up);

        // Combine the current rotation with pitch and yaw adjustments
        Quaternion targetRotation = transform.rotation * yawRotation * pitchRotation;

        // Smoothly interpolate to the target rotation
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rollSpeed * Time.fixedDeltaTime);

        // Handle roll input to rotate around the Z axis
        float rollInput = playerController.GetRoll(); 
        Quaternion rollRotation = Quaternion.Euler(0f, 0f, -rollInput * rollSpeed * Time.fixedDeltaTime);

        // Apply roll rotation
        transform.rotation *= rollRotation;
    }

    private void Move()
    {
        Vector2 input = playerController.GetMovement(); 

        // Use only the forward/backward axis of the input
        float inputForward = input.y;

        // Smoothly interpolate forward speed based on input
        activeForwardSpeed = Mathf.Lerp(activeForwardSpeed, inputForward * forwardSpeed, forwardAcceleration * Time.fixedDeltaTime);

        // Move the Rigidbody 
        Vector3 movement = transform.forward * activeForwardSpeed * Time.fixedDeltaTime;
        rbThird.MovePosition(rbThird.position + movement);

        // Manage movement sound effects
        if (Mathf.Abs(inputForward) > 0.1f) // Check if the player is moving forward/backward
        {
            if (!audioSourceMoving.isPlaying)
            {
                audioSourceMoving.Play();
            }

            // increase the volume while moving
            float targetVolume =  audioSourceMoving.volume = Mathf.Lerp(audioSourceMoving.volume, 0.2f, Time.deltaTime * fadeDuration);
            audioSourceMoving.volume = Mathf.Clamp(targetVolume, 0.0f, 0.2f); // Clamp to max 0.3
        }
        else
        {
            //decrease the volume when not moving
            float targetVolume = Mathf.Lerp(audioSourceMoving.volume, 0.0f, Time.deltaTime * fadeDuration);
            audioSourceMoving.volume = Mathf.Clamp(targetVolume, 0.0f, 0.2f); // Ensure volume stays valid

            // Stop the audio when the volume is very low
            if (audioSourceMoving.volume <= 0.1f && audioSourceMoving.isPlaying)
            {
                audioSourceMoving.Stop();
            }
        }
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
                    audioSourceShooting.Play();
                    Instantiate(machinegunBullet, muzzle.transform.position, transform.rotation);                    
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
    private void UpdateMuzzles()

    {
        if (defaultMuzzlePrefab != null && muzzles.Length > 0)
        {
            // Removes muzzles
            foreach (var muzzle in muzzles)
            {
                if (muzzle != null)
                {
                    Destroy(muzzle);
                }
            }

            muzzles = new GameObject[0];  // clear the list
        }

        if (doubleBarrelMuzzlePrefab != null)
        {
           
            GameObject doubleBarrelMuzzle = Instantiate(doubleBarrelMuzzlePrefab, transform.position + muzzleOffset + doubleBarrelOffset, Quaternion.identity, transform); 

            //Suppose doubleBarrelMuzzle Prefab has two firing points
            Transform[] muzzlePoints = doubleBarrelMuzzlePrefab.GetComponentsInChildren<Transform>();

            // only have the relevant firing points
            if (muzzlePoints.Length >= 2)
            {
                muzzles = new GameObject[2];
                muzzles[0] = Instantiate(doubleBarrelMuzzlePrefab, muzzlePoints[1].position, Quaternion.identity, transform);
                muzzles[1] = Instantiate(doubleBarrelMuzzlePrefab, muzzlePoints[2].position, Quaternion.identity, transform);
            }



        }
        
    }

    
    private void TargetWithinCone()
    {
        if (lockedTarget != null)
        {
            // Calculate the direction toward the target
            Vector3 directionToTarget = (lockedTarget.position - transform.position).normalized;
            float distanceToTarget = Vector3.Distance(transform.position, lockedTarget.position);

            // Horizontal and vertical angle calculation using Vector3.Angle
            float angleHorizontal = Vector3.Angle(transform.forward, directionToTarget);
            float angleVertical = Vector3.Angle(Vector3.ProjectOnPlane(directionToTarget, transform.right), transform.forward);

            // Check if the target is within the cone of vision
            bool isWithinVisionCone = angleHorizontal <= visionConeAngleHorizontal * 0.5f && angleVertical <= visionConeAngleVertical * 0.5f;
            bool isWithinDistance = distanceToTarget <= visionConeDistance;

            if (isWithinVisionCone && isWithinDistance)
            {
                Debug.Log("Target is still within vision cone and distance: " + lockedTarget.name);
            }
            else
            {
                Debug.Log("Target lost or out of range.");
                lockedTarget.GetComponent<IEnemy>().SetCanvasActive(false);
                lockedTarget = null;  // Reset the lock if out of vision or distance
                FindNewTarget();  // Find a new target
            }

            // Visualize the raycast for debugging purposes
            Debug.DrawRay(transform.position, directionToTarget * distanceToTarget, isWithinVisionCone ? Color.red : Color.green, 0.1f);
        }
        else
        {
            FindNewTarget();
        }
    }

    private void FindNewTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, visionConeDistance);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))
            {
                // Calculate direction and angles
                Vector3 directionToTarget = (collider.transform.position - transform.position).normalized;
                float angleHorizontal = Vector3.Angle(transform.forward, directionToTarget);
                float angleVertical = Vector3.Angle(Vector3.ProjectOnPlane(directionToTarget, transform.right), transform.forward);

                // Check if target is within the vision cone
                bool isWithinVisionCone = angleHorizontal <= visionConeAngleHorizontal * 0.5f && angleVertical <= visionConeAngleVertical * 0.5f;

                if (isWithinVisionCone)
                {
                    float distanceToTarget = Vector3.Distance(transform.position, collider.transform.position);

                    // Track the closest enemy
                    if (distanceToTarget < closestDistance)
                    {
                        closestDistance = distanceToTarget;
                        closestTarget = collider.transform;
                    }
                }
            }
        }

        if (closestTarget != null)
        {
            lockedTarget = closestTarget;
            lockedTarget.GetComponent<IEnemy>().SetCanvasActive(true);

            //Debug.Log("New target acquired: " + lockedTarget.name);
        }
        else
        {
            //Debug.Log("No target within vision cone.");
        }
    }
 

    private void SpawnMissile()
    {
        if (missilePrefab == null)
        {
            
            return;
        }

        // Instantiate the missile and set the target
        Missile2 missileInstance = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation).GetComponent<Missile2>();

        // Set the target to the locked target
        if (lockedTarget != null)
        {
            missileInstance.SetTarget(lockedTarget);
        }

    }

    private void ShootMissile()
    {
        if (playerController.GetShootMissile() && missileCooldownTimer <= 0f)
        {
            if (lockedTarget != null)
            {
                SpawnMissile();
                missileCooldownTimer = missileCooldown; // Reset timer

            }
            else
            {
                //Debug.Log("no target find.");
            }


        }
        else
        {
            missileCooldownTimer -= Time.fixedDeltaTime;
            missileCooldownTimer = Mathf.Max(missileCooldownTimer, 0f); // the timer does not go below zero
        }
    }

    
    

    // DAMAGE LOGIC
    public void TakeDamage(float damage)
    {
        // SHIELD
        if (shield.IsShieldActive())
        {
            shield.AbsorbDamage(damage); // Redirects damage to the shield
            Debug.Log("Damage absorbed.");          
            return; // // Exit  if shield takes damage
        }

        health -= damage;
        Debug.Log("player health : " + health);
        if (health <= 0f)
        {
            Destroy(gameObject);
            Debug.Log("Player has died.");

            // Instantiate the explosion effect at the enemy's position and rotation
            if (explosionPrefab != null)
            {
                Instantiate(explosionPrefab, transform.position, transform.rotation);
            }

            // Trigger the Game Over scene
            SceneManager.LoadScene("EndGameMenu");
        }   

  
    }

    private void EnableShield() 
    {
        
        shield.Activate();
        DisablePlayerCollider(); // Disables the player's collider when the shield is active
        shieldDelayTimer = shieldDelay;
        Debug.Log("Shield activated. Player's collider disabled.");
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



    // PICKALE OBJ
    private void AddKey()
    {
        keyCount++;
        PlayerPrefs.SetInt("KeyCount", keyCount);
        ApplyBuffs();

        Debug.Log("key collected: " + keyCount);
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
            // add the key to player 
            AddKey();
            pickable.PickUp(gameObject);
        }
        else if (pickable is Health Health)
        {
            // add the health value to player 
            AddHealth(Health.Value);
        }
    }



    // LEVEL BUFF LOGIC
    public void SetupBuffs()
    {
        OnKeyLevel1.AddListener(() => {
            health += 25f;
            Debug.Log("Level 1 Buff Applied: +25 Health"); 
        });

        OnKeyLevel2.AddListener(() => {
            
            EnableShield();
            Debug.Log("Level 2 Buff Applied: Shield activated");
        });

        OnKeyLevel3.AddListener(() => {
            UpdateMuzzles();
            Debug.Log("Level 3 Buff Applied: Double Barrel Activate");
        });

        OnKeyLevel4.AddListener(() => {
            machinegunBullet.SetDamage(30f);
            Debug.Log("Level 4 buff applied: damage increase");
        });
    }

    private void ApplyBuffs()
    {
        switch (keyCount)
        {

            case 1:

                OnKeyLevel1?.Invoke();
                break;

            case 2:

                OnKeyLevel2?.Invoke();
                break;

            case 3:

                OnKeyLevel3?.Invoke();
                break;

            case 4:

                OnKeyLevel4?.Invoke();
                break;


        }
    }



    // OUT OF BUOND CHECK
    private void CheckDistanceFromPlanet() 
    {
        float distanceFromPlanet = Vector3.Distance(transform.position, planet.transform.position);

        if (distanceFromPlanet > maxDistanceFromPlanet)
        {
            if (!isWarningActive)
            {
                WarningMessage();
            }

            timeOutsideBoundTimer += Time.deltaTime;

            if (timeOutsideBoundTimer >= timeOutsideBound)
            {
                // Destroy the player and reload the level               
                ResetLevel();
            }
        }
        else
        {
            if (isWarningActive) 
            {
                DeactivateWarning();
               
            }

            timeOutsideBoundTimer = 0f;
        }
    }
    
    private void WarningMessage() 
    {
        //Method to show the message "Return to the planet"
        if (warningMessage != null)
        {
            warningMessage.gameObject.SetActive(true);
            warningMessage.text = "GO BACK TO THE PLANET!!";
        }

        isWarningActive = true;
        
    }

    private void DeactivateWarning()
    {
        if (warningMessage != null)
        {
            warningMessage.gameObject.SetActive(false);
        }

        isWarningActive = false;
    }
    
    private void ResetLevel()
    {
        // Method to reset the level
        Destroy(gameObject);

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); 
    }


    // UI
    public float GetMissileCooldown() 
    {
        return missileCooldownTimer;
    }

    public float GetNormalaizedHealth()
    {
        return health / maxHealth;
    }


    // HEALTH
    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float currentHealth) 
    {
        health = currentHealth;
    }


    // KEY
    public int GetKeyCount()
    {
        return keyCount;
    }

    public void SetKeyCount(int currentKeyCount)
    {
        keyCount = currentKeyCount;
    }


    

}