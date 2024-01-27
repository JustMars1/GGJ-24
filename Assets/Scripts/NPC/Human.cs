using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using System.Collections.Generic;

public class Human : Eatable
{
    // Enumeration to represent different states of the human
    private enum HumanState
    {
        Idle,
        RunningAway,
        FallingOver,
        Cooldown,
        RunningToHouse
    }

    // References to components
    private NavMeshAgent navMeshAgent;

    // Parameters for human behavior
    public float detectionRadius = 10f;
    public float normalWanderSpeed = 2f;
    public float runAwaySpeed = 6f;
    public float wanderRadius = 20f;
    public float wanderTimer = 2f;
    public float runAwayDistance = 20f;
    public float fearZigzagFactor = 8f;
    public float fallChance = 0.0003f;
    public float fallDuration = 3.0f;
    public float cooldownDuration = 5.0f;
    public float runAwayDuration = 8f;  
    public float runToHouseChance = 0.005f; 
    public float runToHouseDistance = 1f;


    // Variables for internal use
    private float lastDestinationTime;
    public float timeBetweenDestinations = 1.5f;

    public float stuckTimerThreshold = 2f;
    private float stuckTimer;
    private float timer;
    private Transform playerTransform;
    private bool isFalling;
    private float cooldownTimer;
    private float currentRunAwayTime;  // Track how long they have been running away
    private HumanState currentState = HumanState.Idle;


    // Add a field to store the chosen house position
    private Vector3 chosenHousePosition;

    // Reference to house GameObjects
    private GameObject[] houses;

    void Start()
    {
        // Initialize references to components
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = 0.1f;
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

        // Find houses by tag
        houses = GameObject.FindGameObjectsWithTag("House");

        // Set the initial speed to normal wandering speed
        navMeshAgent.speed = normalWanderSpeed;
    }

    void Update()
    {
        // Update timers
        timer += Time.deltaTime;
        stuckTimer += Time.deltaTime;
        cooldownTimer -= Time.deltaTime;

        // State machine to handle different human behaviors based on the current state
        switch (currentState)
        {
            case HumanState.Idle:
                UpdateIdleState();
                break;
            case HumanState.RunningAway:
                UpdateRunningAwayState();
                break;
            case HumanState.FallingOver:
                // Falling over state is handled separately
                break;
            case HumanState.Cooldown:
                UpdateCooldownState();
                break;
            case HumanState.RunningToHouse:
                UpdateRunningToHouseState();
                break;
        }
    }

    void UpdateIdleState()
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
            // Transition to RunningAway state
            currentState = HumanState.RunningAway;
            navMeshAgent.speed = runAwaySpeed;
            stuckTimer = 0;
            currentRunAwayTime = 0;  // Reset the run-away time
        }
    }

    void UpdateRunningAwayState()
    {
        // Increment the time the human has been running away
        currentRunAwayTime += Time.deltaTime;

        // Check if the NPC is stuck and reset its destination
        if (stuckTimer >= stuckTimerThreshold && navMeshAgent.velocity.magnitude < 0.1f)
        {
            // Generate a random position within the navmesh bounds
            Vector3 randomPos = RandomNavSphere(transform.position, wanderRadius, -1);
            randomPos = ClampToNavmeshBounds(randomPos, wanderRadius, -1);
            navMeshAgent.SetDestination(randomPos);
            stuckTimer = 0;
        }

        // Check if the human should start running to the house
        if (Random.value < runToHouseChance)
        {
            // Transition to RunningToHouse state
            currentState = HumanState.RunningToHouse;
            Debug.Log("Decided to run to the house!");
            return; // Skip the rest of the RunningAway logic
        }

        // Continue with the general running away behavior
        // Run away general direction
        Vector3 runAwayDir = transform.position - playerTransform.position;

        // Zigzag
        Vector3 zigzagOffset = new Vector3(
            Random.Range(-2f, 2f) * fearZigzagFactor,
            0,
            Random.Range(-2f, 2f) * fearZigzagFactor
        );
        Vector3 runAwayPos = transform.position + runAwayDir.normalized * runAwayDistance + zigzagOffset;
        runAwayPos = ClampToNavmeshBounds(runAwayPos, wanderRadius, -1);

        // Set the destination only if enough time has passed since the last one
        if (Time.time - lastDestinationTime > timeBetweenDestinations)
        {
            navMeshAgent.SetDestination(runAwayPos);
            lastDestinationTime = Time.time;
        }

        // Check if the run-away duration is reached
        if (currentRunAwayTime >= runAwayDuration)
        {
            // Transition back to idle state
            currentState = HumanState.Idle;
            navMeshAgent.speed = normalWanderSpeed;
            timer = wanderTimer; // Reset the timer for the next idle destination
        }

        // Occasionally fall over
        if (!isFalling && Random.value < fallChance)
        {
            currentState = HumanState.FallingOver;
            animator.SetTrigger("FallOver");
            StartCoroutine(FallOver());
        }
    }

    void UpdateCooldownState()
    {
        // Check if cooldown duration is over
        if (cooldownTimer <= 0)
        {
            // Transition back to idle state
            currentState = HumanState.Idle;
            navMeshAgent.speed = normalWanderSpeed;
        }
    }

    void UpdateRunningToHouseState()
    {
        // Check if the chosen house position is still valid
        if (!IsHousePositionValid(chosenHousePosition, runToHouseDistance))
        {
            // The chosen house is too far or doesn't exist anymore, find the next closest house
            Vector3 closestHousePos = GetClosestHousePosition();

            // Debug information
            Debug.Log("Running to the closest house!");
            Debug.DrawLine(transform.position, closestHousePos, Color.yellow, 5f);

            // Update the chosen house position
            chosenHousePosition = closestHousePos;
        }

        // Set the destination only if enough time has passed since the last one
        if (Time.time - lastDestinationTime > timeBetweenDestinations)
        {
            navMeshAgent.SetDestination(chosenHousePosition);
            lastDestinationTime = Time.time;
        }

        // Check if the run-to-house duration is reached
        if (Vector3.Distance(transform.position, chosenHousePosition) <= runToHouseDistance)
        {
            // Transition back to idle state
            currentState = HumanState.Idle;
            navMeshAgent.speed = normalWanderSpeed;
            timer = wanderTimer; // Reset the timer for the next idle destination
        }

        // Occasionally fall over
        if (!isFalling && Random.value < fallChance)
        {
            currentState = HumanState.FallingOver;
            animator.SetTrigger("FallOver");
            StartCoroutine(FallOver());
        }
    }

    IEnumerator FallOver()
    {
        // Handle falling over animation
        isFalling = true;
        navMeshAgent.enabled = false;
        yield return new WaitForSeconds(fallDuration);

        // Reset state and animation parameters
        isFalling = false;
        navMeshAgent.enabled = true;
        currentState = HumanState.Cooldown;
        cooldownTimer = cooldownDuration;

        // Set a new destination to resume wandering
        Vector3 randomPos = RandomNavSphere(transform.position, wanderRadius, -1);
        randomPos = ClampToNavmeshBounds(randomPos, wanderRadius, -1);
        navMeshAgent.SetDestination(randomPos);
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

    // Function to get the position of the closest house
    Vector3 GetClosestHousePosition()
    {
        Vector3 closestHousePos = Vector3.zero;
        float closestDistance = float.MaxValue;

        // Filter out destroyed houses from the list
        List<GameObject> validHouses = new List<GameObject>();
        foreach (GameObject house in houses)
        {
            if (house != null)
            {
                validHouses.Add(house);
            }
        }

        foreach (GameObject house in validHouses)
        {
            float distance = Vector3.Distance(transform.position, house.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestHousePos = house.transform.position;
            }
        }

        return closestHousePos;
    }
    // Function to check if the house position is still valid
    bool IsHousePositionValid(Vector3 housePosition, float distanceThreshold)
    {
        if (Vector3.Distance(transform.position, housePosition) > distanceThreshold)
        {
            // The house is too far or doesn't exist anymore
            return false;
        }

        // Check if there's any obstacle between the human and the house
        NavMeshPath path = new NavMeshPath();
        navMeshAgent.CalculatePath(housePosition, path);
        if (path.status != NavMeshPathStatus.PathComplete)
        {
            // The path is not complete, meaning there's an obstacle
            return false;
        }

        // The house position is considered valid
        return true;
    }
}