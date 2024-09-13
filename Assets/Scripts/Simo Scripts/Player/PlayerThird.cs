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

        if (input.magnitude > 0 )
        {
            if (!audioSourceMoving.isPlaying)
            {
                audioSourceMoving.Play();
            }
            
            audioSourceMoving.volume = Mathf.Lerp(audioSourceMoving.volume, 1.0f, Time.deltaTime * fadeDuration);
        }
        else if (input.magnitude == 0)
        {
            audioSourceMoving.volume = Mathf.Lerp(audioSourceMoving.volume, 0.0f, Time.deltaTime * fadeDuration);
            
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
    // RIMANE DA DEBUGGARE I RAYCAST, PERCHè LA CONDIZIONE IF NON VIENE MAI SODDISFATTA MA ENTRA SOLO NELL'ELSE

    private void UpdateMuzzles()

    {
        if (defaultMuzzlePrefab != null && muzzles.Length > 0)
        {
            // Rimuove i muzzles predefiniti
            foreach (var muzzle in muzzles)
            {
                if (muzzle != null)
                {
                    Destroy(muzzle);
                }
            }
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
            //Debug.LogError("missilePrefab is not assigned!");
            return;
        }

        Missile missile = Instantiate(missilePrefab, missileSpawnPoint.position, missileSpawnPoint.rotation);
        //Missile missileScript = missile.GetComponent<Missile>();
        missile.SetTarget(lockedTarget);
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
        // SHIELD
        if (shield.IsShieldActive())
        {
            shield.AbsorbDamage(damage); // Reindirizza i danni allo scudo
            Debug.Log("Danno assorbito dallo scudo.");          
            return; // Esci dal metodo se lo scudo prende il danno
        }

        health -= damage;
        Debug.Log("player health : " + health);
        if (health <= 0f)
        {
            Destroy(gameObject);
            Debug.Log("Player has died.");

            // Trigger the Game Over scene
            //FindObjectOfType<GameOverScene>().TriggerGameOver();
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
            Debug.Log("Level 1 Buff Applied: +25 Health"); //Save the increase health
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

    public float GetNormalaizedHealth()
    {
        return health / maxHealth;
    }

    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float currentHealth) 
    {
        health = currentHealth;
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