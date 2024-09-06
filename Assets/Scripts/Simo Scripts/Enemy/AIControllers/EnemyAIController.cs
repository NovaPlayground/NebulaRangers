using System.Collections;
using UnityEngine;

public class EnemyAIController : MonoBehaviour
{
    private enum State
    {
        Patrol,
        RunToPlayer,
        ShootToPlayer,
        EvadePlayer

    }

    //MOVEMENT
    [SerializeField] private float patrolSpeed = 10f;
    [SerializeField] private float runToPlayerSpeed = 10f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float acceleration = 5f; //determines the rate at which the spaceship changes its current speed towards the desired speed.
    [SerializeField] private float evadeSpeed = 15f; // Speed when evading the player
    private float followDistance;

    private Vector3 targetDirection;
    private Vector3 randomDirection;
    private Vector3 currentVelocity;


    // COOLDOWNS
    [SerializeField] private float changeDirectionCooldown = 2f;
    [SerializeField] private float changeShootDirectionCooldown = 5f;
    [SerializeField] private float evadeCheckInterval = 2f; // Interval to check the decision in EvadePlayer
    private float evadeCheckCooldown; // Cooldown for checking status in EvadePlayer
    private float movementShootDirectionCooldown;
    private float changeMovementDirCooldown; // cooldown to move


    // PLAYER 
    [SerializeField] private Transform player;
    [SerializeField] private Collider shieldCollider; // Player shield collider
    private bool hasPlayerEnteredColliderOnce = false; // check if the player has entered the collider at least once

    // SHOOT 
    [SerializeField] private MachinegunBullet machinegunBullet;
    [SerializeField] private float shootDelay = 1f;
    [SerializeField] private GameObject[] muzzles;
    private float shootCooldown = 0f;


    // STATE 
    private State currentState = State.Patrol;
    private bool isHit = false;
    private bool isFirstTimePlayerEnters = true;

    // ENEMY 
    [SerializeField] private SphereCollider enemyCollider; // Enemy's SphereCollider 
    [SerializeField] private float healthRegenTime = 5f; // to restore enemy health after a fight if he can evade
    [SerializeField] private Transform planet;
    private float healthRegenTimer = 0f;
    //private EnemyShoot enemy;
    //private EnemyShield enemyShield;
    private Rigidbody enemyRigidbody;

    // INTERFACE
    private IEnemy enemy;


    // CHECK OUT OF BOUND
    [SerializeField] private float maxDistanceFromPlanet;


    // SPAWN MANAGER
    private SpawnManager spawnManager;


    //public EnemyAIController(EnemyShoot enemy)
    //{
    //    this.enemy = enemy;
    //    enemyRigidbody = enemy.GetRigidbody();

    //}

    public EnemyAIController(IEnemy enemy)
    {
        this.enemy = enemy;
        enemyRigidbody = enemy.GetRigidbody();

    }

    //public EnemyAIController(MonoBehaviour enemy)
    //{
    //    if (enemy is EnemyShoot)
    //    {
    //        this.enemy = enemy as EnemyShoot;
    //        enemyRigidbody = this.enemy.GetRigidbody();
    //    }
    //    else if (enemy is EnemyShield)
    //    {
    //        var enemyShield = enemy as EnemyShield;
    //        enemyRigidbody = enemyShield.GetRigidbody();
    //    }
    //    else
    //    {
    //        Debug.LogError("Enemy type not supported.");
    //    }

    //}


    // Start is called before the first frame update
    void Start()
    {

        // Ref to planet
        if (planet == null)
        {
            planet = GameObject.FindWithTag("Planet").transform;        
        }
        //enemy = GetComponent<EnemyShoot>();
        enemy = GetComponent<IEnemy>();

        enemyRigidbody = GetComponent<Rigidbody>();
        

        if (enemyCollider != null)
        {
            followDistance = enemyCollider.radius;
        }

        changeMovementDirCooldown = changeDirectionCooldown;
        movementShootDirectionCooldown = changeShootDirectionCooldown;
        evadeCheckCooldown = evadeCheckInterval;

        ChangeDirection();

        // SPAWN
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    // UPDATES 
    void Update()
    {
  
        switch (currentState)
        {
            case State.Patrol:
                PatrolUpdate();
                break;
            case State.RunToPlayer:
                RunToPlayer();
                break;
            case State.ShootToPlayer:
                ShootUpdate();
                break;
            case State.EvadePlayer:
                EvadePlayer();
                break;

        }

        
    }

    private void PatrolUpdate()
    {
        changeMovementDirCooldown -= Time.deltaTime;

        if (changeMovementDirCooldown <= 0f)
        {
            ChangeDirection();
            changeMovementDirCooldown = changeDirectionCooldown;
        }

        Patrol();


        // CHECK DISTANCE FROM PLANET
        float distanceToPlanet = Vector3.Distance(transform.position, planet.position);

        if (distanceToPlanet > maxDistanceFromPlanet) 
        {
            
            DestroyAndRespawn();
        }


        // RESTORE HEALTH
        healthRegenTimer += Time.deltaTime;

        if (healthRegenTimer >= 5f)
        {           
            enemy.SetHealth(enemy.GetMaxHealth());
            healthRegenTimer = 0f;
        }

    }



    private void ShootUpdate()
    {
        

        if (player == null)
        {
            currentState = State.Patrol;
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        if (distanceToPlayer > followDistance)
        {
            currentState = State.RunToPlayer;
            return;
        }

        // Random movement around the player
        movementShootDirectionCooldown -= Time.deltaTime;

        if (movementShootDirectionCooldown <= 0f)
        {
            ChangeDirectionMovementToShoot();
            movementShootDirectionCooldown = changeShootDirectionCooldown;
        }

        Vector3 desiredVelocity = targetDirection * runToPlayerSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        Vector3 targetPosition = enemyRigidbody.position + currentVelocity * Time.fixedDeltaTime;
        enemyRigidbody.MovePosition(targetPosition);

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);

        enemyRigidbody.transform.rotation = Quaternion.Slerp(enemyRigidbody.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        //viewing angle of the enemy, you can decide how large to make it. if the player is inside the corner he is shot otherwise not
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer < 10f)
        {
            shootCooldown -= Time.deltaTime;
            if (shootCooldown <= 0f)
            {
                ShootToPlayer();
                shootCooldown = shootDelay; // Reset cooldown to shoot
            }
        }

       
    }


    // STATES 
    private void ChangeDirection()
    {
        // Generates a random direction in a unit spherical space and normalizes the vector
        do
        {
            //randomDirection = Random.insideUnitSphere.normalized;
            targetDirection = Random.insideUnitSphere.normalized;

        } while (targetDirection == Vector3.zero);

        
    }

    private void ChangeDirectionMovementToShoot()
    {

        targetDirection = Random.insideUnitSphere.normalized;
    }

    private void Patrol()
    {

        //Rotation to new direction    
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        enemyRigidbody.transform.rotation = Quaternion.Slerp(enemyRigidbody.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Acceleration towards the target direction 
        Vector3 desiredVelocity = targetDirection * patrolSpeed;

        //currentVelocity is used to calculate the target position, allowing the spacecraft to gradually accelerate and decelerate, creating a more natural motion.
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        Vector3 targetPosition = enemyRigidbody.position + currentVelocity * patrolSpeed * Time.fixedDeltaTime;

        enemyRigidbody.MovePosition(targetPosition);

    }


    private void RunToPlayer()
    {
        if (player == null)
        {
            currentState = State.Patrol;
            isHit = false;
            return;
        }      

        movementShootDirectionCooldown -= Time.deltaTime;

        if (movementShootDirectionCooldown <= 0f)
        {
            ChangeDirectionMovementToShoot();
            movementShootDirectionCooldown = changeShootDirectionCooldown;
        }

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        enemyRigidbody.transform.rotation = Quaternion.Slerp(enemyRigidbody.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        //check the distance between the enemy and the player, if he is far away,
        //he gets closer, or if he is already close, shoot him
        if (distanceToPlayer > followDistance)
        {
            Vector3 desiredVelocity = directionToPlayer * runToPlayerSpeed;
            currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
        }
        else
        {
            Vector3 desiredVelocity = targetDirection * runToPlayerSpeed;
            currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);
            currentState = State.ShootToPlayer;
        }

        Vector3 targetPosition = enemyRigidbody.position + currentVelocity * Time.fixedDeltaTime;
        enemyRigidbody.MovePosition(targetPosition);


    }


    private void ShootToPlayer()
    {
       

        foreach (var muzzle in muzzles)
        {
           Instantiate(machinegunBullet, muzzle.transform.position, muzzle.transform.rotation);
        }
    }

    private void EvadePlayer()
    {
        if (player == null)
        {
            currentState = State.Patrol;
            hasPlayerEnteredColliderOnce = false; // Resets when the enemy loses reference to the player
            isHit = false;
            return;
        }

        // if the enemy decides to escape, and manages to move far enough away from the player, he returns to patrol,
        // however, if we return with the player within its range of action, it will approach the player but, seeing that it is him, it will run away 

        Vector3 directionAwayFromPlayer = (transform.position - player.position).normalized;

        // calculate the rotation to rotate the forward in the opposite direction so as not to look at the player 
        Quaternion targetRotation = Quaternion.LookRotation(directionAwayFromPlayer);
        enemyRigidbody.transform.rotation = Quaternion.Slerp(enemyRigidbody.transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);

        Vector3 desiredVelocity = directionAwayFromPlayer * evadeSpeed;
        currentVelocity = Vector3.Lerp(currentVelocity, desiredVelocity, acceleration * Time.fixedDeltaTime);

        Vector3 targetPosition = enemyRigidbody.position + currentVelocity * Time.fixedDeltaTime;
        enemyRigidbody.MovePosition(targetPosition);

        //check health // logic to decide if run or fight
        evadeCheckCooldown -= Time.deltaTime;
       
        if (evadeCheckCooldown <= 0f)
        {
            evadeCheckCooldown = evadeCheckInterval;


            // Check if player is within distance and enemy health is still low
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            float randomChoice = Random.value;
            Debug.Log($" random number : { randomChoice }");

            if (distanceToPlayer > followDistance * 2f) 
            {
                currentState = State.Patrol;               
                return;
            }
            else if (distanceToPlayer > followDistance * 0.5 && enemy.GetHealth() <= enemy.GetMaxHealth() * 0.25f) 
            {
                currentState = State.RunToPlayer;
            }
            else
            {
                currentState = State.EvadePlayer;
            }

        }

    }

    private void DestroyAndRespawn() 
    {
        // Notifica lo SpawnManager della distruzione del nemico
        spawnManager.OnEnemyDestroyed(gameObject);

        //Disattiva l'oggetto (invece di distruggerlo completamente)
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other == shieldCollider)
        {
            // Ensure that we are referencing the player's transform even if the shield enters first
            player = other.gameObject.transform.root; // Get the player's root object, which is the player itself
            currentState = State.RunToPlayer;

            //float randomChoice = Random.value;

            if (enemy.GetHealth() <= enemy.GetMaxHealth() * 0.5f)
            {
                currentState = State.EvadePlayer;
                healthRegenTimer = 0f;
            }

        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other == shieldCollider)
        {          
            currentState = State.Patrol;
            isHit = false;
            healthRegenTimer = 0f; 

        }

        

        
    }

}
