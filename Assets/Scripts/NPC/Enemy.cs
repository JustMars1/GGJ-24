using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class Enemy : Eatable
{
    // Enumeration to represent different states of the human
    private enum EnemyState
    {
        Wonder,
        FollowingPlayer,
        AttackPlayer,
        Cooldown
    }

    // References to components
    private NavMeshAgent navMeshAgent;

    // Parameters for Enemy behavior
    public float detectionRadius = 10f;
    public float normalWanderSpeed = 2f;
    public float followPlayerSpeed = 6f;
    public float wanderRadius = 20f;
    public float wanderTimer = 2f;
    public float cooldownDuration = 5.0f;
    public float attackRange = 1.5f;
    public int normalEnemyDamage = 30;


    // Variables for internal use
    private float lastDestinationTime;
    public float timeBetweenDestinations = 1.5f;

    public float stuckTimerThreshold = 2f;
    private float stuckTimer;
    private float timer;
    private Transform playerTransform;
    private float cooldownTimer;
    private EnemyState currentState = EnemyState.Wonder;

    private Vector3 lastKnownPlayerPosition = Vector3.zero;

    protected override void Start()
    {
        base.Start();
        // Initialize references to components
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = 1f;
        // Find the player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
        }
        else
        {
            Debug.LogError("Player not found!");
        }

        // Set the initial speed to normal wandering speed
        navMeshAgent.speed = normalWanderSpeed;
    }

    public void Update()
    {
        // Update timers
        timer += Time.deltaTime;
        stuckTimer += Time.deltaTime;
        cooldownTimer -= Time.deltaTime;

        // State machine to handle different human behaviors based on the current state
        switch (currentState)
        {
            case EnemyState.Wonder:
                UpdateWonderState();
                break;
            case EnemyState.FollowingPlayer:
                UpdateFollowingPlayerState();
                break;
            case EnemyState.AttackPlayer: 
                UpdateAttackPlayerState();
                break;
            case EnemyState.Cooldown:
                UpdateCooldownState();
                break;
        }
        // Update animator parameters based on the current state
        UpdateAnimatorParameters();
    }

    private void UpdateAnimatorParameters()
    {
        // Update animator parameters based on the current state
        animator.SetBool("IsWalking", currentState == EnemyState.Wonder);
        animator.SetBool("IsRunning", currentState == EnemyState.FollowingPlayer);
    }

    public void UpdateWonderState()
    {
        // Check if it's time to pick a new destination for wandering
        if (timer >= wanderTimer)
        {
            // Generate a random position within the navmesh bounds
            Vector3 newPos = RandomNavSphere(transform.position, wanderRadius, -1);
            newPos = ClampToNavmeshBounds(newPos, wanderRadius, -1);
            navMeshAgent.SetDestination(newPos);

            // Reset the timer
            timer = 0;
        }

        // Check if the player is nearby
        if (playerTransform != null && Vector3.Distance(transform.position, playerTransform.position) < detectionRadius)
        {
            // Transition to FollowingPlayer state
            currentState = EnemyState.FollowingPlayer;
            navMeshAgent.speed = followPlayerSpeed;
            stuckTimer = 0;
        }
    }

    public void UpdateFollowingPlayerState()
    {
        //If player is in sight, it will follow the player
        //If out of sight it follows player to the last known position. If player is not in sight at the last known position, it will go back to idle state
        // Check if the player is in line of sight
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (playerTransform.position - transform.position).normalized, out hit, detectionRadius))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // Player is in line of sight, follow the player
                navMeshAgent.SetDestination(playerTransform.position);

                // Update the last known position
                lastKnownPlayerPosition = playerTransform.position;

                // Draw the ray for visualization
                Debug.DrawRay(transform.position, (lastKnownPlayerPosition - transform.position).normalized * detectionRadius, Color.red);
            }
            else
            {
                // Player is not in line of sight, go to the last known position
                navMeshAgent.SetDestination(lastKnownPlayerPosition);

                // Draw the ray to visualize the movement
                Debug.DrawRay(transform.position, (lastKnownPlayerPosition - transform.position).normalized * detectionRadius, Color.yellow);
            }
        }

        // Check if the enemy has reached the last known player position
        float distanceToLastKnownPosition = Vector3.Distance(transform.position, lastKnownPlayerPosition);
        if (distanceToLastKnownPosition < navMeshAgent.stoppingDistance)
        {
            Debug.Log($"Distance to last known position: {distanceToLastKnownPosition}");

            // Check if the player is still within the detection radius
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
            if (distanceToPlayer > detectionRadius)
            {
                // Player is not in line of sight at the last known position

                // Increment the timer
                stuckTimer += Time.deltaTime;

                if (stuckTimer > 5.0f) // If the enemy has been stuck for more than 5 seconds
                {
                    // Go back to Wonder state
                    currentState = EnemyState.Wonder;
                    navMeshAgent.speed = normalWanderSpeed;
                    timer = wanderTimer; // Reset the timer for the next idle destination
                    lastKnownPlayerPosition = Vector3.zero; // Reset the last known position
                    stuckTimer = 0; // Reset the stuck timer
                }
            }
            else
            {
                // Player is still within the detection radius, reset the stuck timer
                stuckTimer = 0;
            }
        }
    }
    public void UpdateAttackPlayerState()
    {
        //Start animation attack player if player is in attack range
        //Animation has animation event that calls DealDamage() function

        // Check if the player is still within attack range
        if (Vector3.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            // Player is in attack range, initiate attack animation
            // The animation should have an event calling the DealDamage() function
            animator.SetTrigger("MeleeAttack");
        }
        else
        {
            // Player is out of attack range, transition back to FollowingPlayer state
            currentState = EnemyState.FollowingPlayer;
            navMeshAgent.SetDestination(playerTransform.position);
            navMeshAgent.speed = followPlayerSpeed;
        }
    }

    public void UpdateCooldownState()
    {
        // Check if cooldown duration is over
        if (cooldownTimer <= 0)
        {
            // Transition back to idle state
            currentState = EnemyState.Wonder;
            navMeshAgent.speed = normalWanderSpeed;
        }
    }

    // Function to generate a random position within a sphere
    Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;
        randDirection += origin;

        NavMeshHit navHit;
        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }

    // Function to clamp position to navmesh bounds
    Vector3 ClampToNavmeshBounds(Vector3 position, float dist, int layermask)
    {
        NavMeshHit navHit;
        if (NavMesh.SamplePosition(position, out navHit, dist, layermask))
        {
            return navHit.position;
        }
        return position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    public void DealDamage()
    {
        //Deal damage to player
        GameManager.manager.ReduceScore(normalEnemyDamage);
    }
}