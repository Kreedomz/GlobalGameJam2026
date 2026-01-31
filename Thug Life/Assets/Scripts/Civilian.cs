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
    [SerializeField] float wanderRange = 5.0f; // The range the AI will wander around based on their start location
    [SerializeField] float timeBeforeNextWander = 5.0f; // The wait time in second before AI starting to wander again
    float currentTimeWandering = 0.0f; // The elapsed time since the AI has been wandering for | Resets on new wander location generated

    NavMeshAgent agent;

    Vector3 startLocation = Vector3.zero; // The location where the AI will be wandering around
    Vector3 targetLocation = Vector3.zero; // The location where the AI is supposed to walking towards

    [Header("Robbing Settings")]
    [SerializeField] float timeBetweenRobbings = 20.0f; // The amount of time in seconds before the civilian can be robbed again
    float elapsedTimeSinceLastRob = 0.0f;
    public RobState robState { get; private set; } = RobState.None; // Determines whether or on the civilian can be robbed

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        startLocation = transform.position;

        // Check if the AI has an initial target location to move towards
        if (targetLocation == Vector3.zero)
        {
            targetLocation = GetNewWanderPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        HandleCivilianWandering();
        HandleRobbingState();
    }

    void HandleRobbingState()
    {
        if (robState == RobState.Robbed)
        {
            elapsedTimeSinceLastRob += Time.deltaTime;

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
            robState = RobState.Robbed;
        }
    }
}
