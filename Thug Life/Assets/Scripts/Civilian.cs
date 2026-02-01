using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Civilian : MonoBehaviour
{
    public enum RobState
    {
        None,
        Robbed
    }

    [Header("Civilian Settings")]
    [SerializeField] float movementSpeed = 3.5f; // How fast the civilian moves
    [SerializeField] float wanderRange = 5.0f; // The range the AI will wander around based on their start location
    [SerializeField] float timeBeforeNextWander = 5.0f; // The wait time in second before AI starting to wander again
    float currentTimeWandering = 0.0f; // The elapsed time since the AI has been wandering for | Resets on new wander location generated

    NavMeshAgent agent = null;

    Vector3 startLocation = Vector3.zero; // The location where the AI will be wandering around
    Vector3 targetLocation = Vector3.zero; // The location where the AI is supposed to walking towards

    [Header("Robbing Settings")]
    [SerializeField] float timeBetweenRobbings = 20.0f; // The amount of time in seconds before the civilian can be robbed again
    float elapsedTimeSinceLastRob = 0.0f;
    [SerializeField] float timeSpottedToLoseRep = 5.0f; // The amount of time in seconds required for the civilian to spot the player and make him lose rep
    float timeWhileSpotted = 0.0f; // The amount in seconds that the player is being spotted by the current civilian
    public RobState robState { get; private set; } = RobState.None; // Determines whether or on the civilian can be robbed


    [Header("Vision Cone Settings")]
    [SerializeField] float visionAngle = 90.0f; // The amount of angles in degrees
    [SerializeField] float detectionRange = 10.0f; // How far the civilians can detect players from (used in the light component)
    Light visionLight = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        visionLight = GetComponent<Light>();
        startLocation = transform.position;

        // Check if the AI has an initial target location to move towards
        if (targetLocation == Vector3.zero)
        {
            targetLocation = GetNewWanderPosition();
        }

        // Set the nav mesh agent properties
        if (agent != null)
        {
            agent.speed = movementSpeed;
        }

        // Make sure the civilian has a Light component
        if (visionLight != null)
        {
            visionLight.range = detectionRange;
            visionLight.innerSpotAngle = visionAngle;
            visionLight.spotAngle = visionAngle;
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleCivilianWandering();
        HandleRobbingState();
        HandlePlayerDetection();
    }

    void HandlePlayerDetection()
    {
        // Check if the player is inside the civilians range and vision cone
        if (IsPlayerInsideVision() && IsPlayerInsideRange())
        {
            Player player = FindFirstObjectByType<Player>();
            if (player != null)
            {
                // If the civilian detects the player while they have their mask on
                if (player.maskState == Player.MaskState.On)
                {
                    timeWhileSpotted += Time.deltaTime;

                    // If the player is spotted for a long time, make them lose reputation
                    if (timeWhileSpotted >= timeSpottedToLoseRep)
                    {
                        // Show seen UI
                        HUDController HUDController = FindFirstObjectByType<HUDController>();
                        if (HUDController != null)
                        {
                            HUDController.FlashSeen();
                        }
                        // Lose player rep
                        player.LoseReputation();
                        timeWhileSpotted = 0.0f;
                    }
                }
                // If the player is inside the civilians range and distance but does not have their mask on
                else
                {
                    if (timeWhileSpotted > 0.0f)
                    {
                        // Remove the time spotted on the player when no mask is detected on the player
                        timeWhileSpotted -= Time.deltaTime;
                    }
                    else
                    {
                        timeWhileSpotted = 0.0f;
                    }
                }
            }
        }
    }

    void HandleRobbingState()
    {
        // If civilian is robbed
        if (robState == RobState.Robbed)
        {
            elapsedTimeSinceLastRob += Time.deltaTime;

            // Check if the robbery cooldown of the civilian is over
            if (elapsedTimeSinceLastRob >= timeBetweenRobbings)
            {
                robState = RobState.None;
                elapsedTimeSinceLastRob = 0.0f;
            }
        }
    }

    void HandleCivilianWandering()
    {
        if (agent == null) { return; } // If no NavMeshAgent component is detected

        // Check if the AI is able to move to their target location
        if (targetLocation != Vector3.zero)
        {
            // If the AI is close to their target location/destination
            float distanceToTarget = Vector3.Distance(transform.position, targetLocation);
            if (distanceToTarget <= 1.0f && currentTimeWandering >= timeBeforeNextWander)
            {
                targetLocation = GetNewWanderPosition();
                currentTimeWandering = 0.0f;
            }
            // AI is far, so move the AI to the target location
            else
            {
                agent.SetDestination(targetLocation);
                currentTimeWandering += Time.deltaTime;
            }
        }
    }

    Vector3 GetNewWanderPosition()
    {
        if (startLocation != Vector3.zero)
        {
            Vector3 randomCirclePosition = new Vector3(Random.insideUnitCircle.x * wanderRange, 0.0f, Random.insideUnitCircle.y * wanderRange);

            Vector3 newPositionVector = randomCirclePosition + startLocation;

            return newPositionVector;
        }
        return Vector3.zero;
    }

    public void RobCivilian()
    {
        if (robState == RobState.Robbed) { return; } // Ignore a robbed civilian

        // If a player is found, give a reward
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.AddReputation(); // Add reputation for a successfull civilian rob
            player.AddMoney();
            robState = RobState.Robbed;
        }
    }

    bool IsPlayerInsideVision()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            float angleToPlayer = Vector3.Angle(transform.forward, playerDirection);

            // If the player is within the civilians detection vision angle
            if (angleToPlayer <= visionAngle)
            {
                return true;
            }
        }

        return false;
    }

    bool IsPlayerInsideRange()
    {
        Player player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            // If the player is within the detection range of the civilian
            if (Vector3.Distance(player.transform.position, transform.position) <= detectionRange)
            {
                return true;
            }
        }
        return false;
    }
}
