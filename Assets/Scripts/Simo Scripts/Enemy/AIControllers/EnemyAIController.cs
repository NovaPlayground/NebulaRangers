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

        if (enemyCollider != null)
        {
            followDistance = enemyCollider.radius;
        }

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
                hasPlayerEnteredColliderOnce = false; //Reset when player is too far away
                return;
            }
            else if (distanceToPlayer > followDistance * 0.5 && hasPlayerEnteredColliderOnce && randomChoice > 0.5f) 
            {
                currentState = State.RunToPlayer;
            }
            else
            {
                currentState = State.EvadePlayer;
            }

        }

    }

 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other == shieldCollider)
        {          
            // Ensure that we are referencing the player's transform even if the shield enters first
            player = other.gameObject.transform.root; // Get the player's root object, which is the player itself
            currentState = State.RunToPlayer;

            float randomChoice = Random.value;

            if (enemy.GetHealth() <= enemy.GetMaxHealth() * 0.5f)  //  MAYBE IT NEEDS TO BE FIXED  // crea un timer che allo scadere del tempo se il nemico � riuscito a scappare rigenera la vita del 100%. se il nemico scende sotto il 25% deve andare in runtoplayer 
            {
                currentState = State.EvadePlayer;  
                hasPlayerEnteredColliderOnce = true; 
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") || other == shieldCollider)
        {          
            currentState = State.Patrol;
            isHit = false;
                    
        }

        

        
    }

}
