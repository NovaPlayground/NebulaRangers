using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;
using static UnityEngine.UI.GridLayoutGroup;

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
    [SerializeField] private float followDistance = 10f;
    [SerializeField] private float evadeSpeed = 15f; // Speed when evading the player

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
    [SerializeField] private Collider shieldCollider; // Collider dello shield del player

    // SHOOT 
    [SerializeField] private DebugNormalBullet debugNormalBullet;
    [SerializeField] private float shootDelay = 1f;
    [SerializeField] private GameObject[] muzzles;
    private float shootCooldown = 0f;


    // STATE 
    private State currentState = State.Patrol;
    private bool isHit = false;

    // ENEMY 
    private EnemyShoot enemy;
    private Rigidbody enemyRigidbody;
    


    public EnemyAIController(EnemyShoot enemy)
    {
        this.enemy = enemy;
        enemyRigidbody = enemy.GetRigidbody();
          
    }


    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponent<EnemyShoot>();

        enemyRigidbody = GetComponent<Rigidbody>();


        changeMovementDirCooldown = changeDirectionCooldown;
        movementShootDirectionCooldown = changeShootDirectionCooldown;
        evadeCheckCooldown = evadeCheckInterval;
        ChangeDirection();
    }

    // UPDATES 
    void Update()
    {

        // Check health and update isHit
        if (enemy.GetHealth() <= enemy.GetMaxHealth() * 0.5f && !isHit)
        {
            isHit = true;
            currentState = State.EvadePlayer;
        }

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

        ////check health // logic to decide if run or fight
        //if (enemy.GetHealth() <= enemy.GetMaxHealth() * 0.5f )//&&/* enemy.IsHit()*/)
        //{
        //    isHit = true;

        //    if (isHit)
        //    {
        //        currentState = State.EvadePlayer;
        //    }

            
        //}
    }



    // STATES 
    private void ChangeDirection()
    {
        // Genera una direzione casuale in uno spazio sferico unitario e normalizza il vettore
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
        //if (player == null)
        //{
        //    currentState = State.RunToPlayer;
        //    return;
        //}

        foreach (var muzzle in muzzles)
        {
           Instantiate(debugNormalBullet, muzzle.transform.position, muzzle.transform.rotation);
        }
    }

    private void EvadePlayer() 
    {


        if (player == null)
        {
            currentState = State.Patrol;
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

        //if (evadeCheckCooldown <= 0f)
        //{
        //    evadeCheckCooldown = evadeCheckInterval;

        //    // Additional conditions to decide whether to continue evading or switch to attacking
        //    if (player != null && Vector3.Distance(transform.position, player.position) < followDistance && enemy.GetHealth() <= enemy.GetMaxHealth() * 0.5f)
        //    {
        //        // If player is within range and enemy health is still below 50%, continue evading
        //        currentState = State.EvadePlayer;
        //    }
        //    else if (Vector3.Distance(transform.position, player.position) >= followDistance * 2f)
        //    {
        //        // If player is far enough, return to Patrol state
        //        currentState = State.Patrol;
        //        isHit = false; // Reset isHit after deciding to return to patrol
        //    }
        //    else if (currentState != State.EvadePlayer)
        //    {
        //        // Only switch to attacking if not already in EvadePlayer state
        //        currentState = State.RunToPlayer;
        //        isHit = false; // Reset isHit after deciding to attack
        //    }
        //    //else
        //    //{
        //    //    // Switch to attacking
        //    //    currentState = State.RunToPlayer;
        //    //    isHit = false;
        //    //}
        //}
        if (evadeCheckCooldown <= 0f)
        {
            evadeCheckCooldown = evadeCheckInterval;

            // Check if player is within distance and enemy health is still low
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer < followDistance * 0.5f && enemy.GetHealth() <= enemy.GetMaxHealth() * 0.5f)
            {
                // Continue evading if player is close and health is low
                currentState = State.EvadePlayer;
            }
            else
            {
                // Randomly decide to continue evading or switch to attacking
                float randomValue = Random.value;
                if (randomValue > 0.5f)
                {
                    currentState = State.EvadePlayer;
                }
                else
                {
                    currentState = State.RunToPlayer;
                    isHit = false; // Reset isHit after deciding to attack
                }
            }
        }

        //Check if the enemy is far enough away from the player to not immediately return
        float distanceToKeepEvadePlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToKeepEvadePlayer < followDistance * 0.5f ) // if the player is close, keep evading
        {
            currentState = State.EvadePlayer;
        }
        // QUESTO SI BLOCCA PERCHè PERDE LA TRASFORM DEL PLAYER
        //else if (distanceToPlayer > followDistance * 2f) // if the player is far, return in patrol state
        //{
        //    currentState = State.Patrol;
        //    isHit = false;
        //}

    }



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other == shieldCollider)
        {
            //player = other.gameObject.transform;
            // Ensure that we are referencing the player's transform even if the shield enters first
            player = other.gameObject.transform.root; // Get the player's root object, which is the player itself
            currentState = State.RunToPlayer;
        }


       

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other == shieldCollider)
        {
            player = null;
            currentState = State.Patrol;
            isHit = false;
        }

        //isHit = false;
    }

}
