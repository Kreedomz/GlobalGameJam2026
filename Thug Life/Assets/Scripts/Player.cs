using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Robbing Settings")]
    [SerializeField] float robbingDistance = 2.5f; // How far the player can be from a civilian to rob them
    [SerializeField] KeyCode robbingKey = KeyCode.E; // The key to press to rob civilians
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Reward Settings")]
    int playerReputation = 0;
    [SerializeField] int reputationGainPerRob = 100;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleRobbing();
    }

    void HandleRobbing()
    {
        // Grab all the civilians in the game
        Civilian[] civilians = FindObjectsByType<Civilian>(FindObjectsSortMode.None);

        // Check if player is pressing the robbing key
        if (Input.GetKeyDown(robbingKey))
        {
            // Loop through all civilians in the game
            foreach (Civilian civilian in civilians)
            {
                float distanceToCivilian = Vector3.Distance(transform.position, civilian.transform.position);

                // If the current civilian is within the rob distance of the player
                if (distanceToCivilian <= robbingDistance)
                {
                    civilian.RobCivilian(); // Try to rob the civilian if they are "robbable"
                    break; // Make sure we only rob one civilian at a time and ignore continuing the for loop
                }
            }
        }
    }

    public void AddReputation()
    {
        playerReputation += reputationGainPerRob;
    }

    public void AddReputation(int reputationToAdd)
    {
        playerReputation += reputationToAdd;
    }
    
    public void NegateReputation(int reputationToNegate)
    {
        playerReputation -= reputationToNegate;
    }

}
