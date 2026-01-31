using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum MaskState
    {
        Off,
        On
    }

    [Header("Robbing Settings")]
    [SerializeField] float robbingDistance = 2.5f; // How far the player can be from a civilian to rob them
    [SerializeField] KeyCode robbingKey = KeyCode.E; // The key to press to rob civilians
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Mask Settings")]
    [SerializeField] KeyCode maskKey = KeyCode.G; // The key players will use to put on and take off their mask
    public MaskState maskState { get; private set; } = MaskState.Off;

    [Header("Player Reward Settings")]
    public int playerReputation { get; private set; } = 0;
    [SerializeField] int reputationReward = 150; // The amount to gain when successfully robbing a civilian and how much to lose if you get spotted
    [SerializeField] int maximumReputation = 1000; // The maximum amount of reputation a player can have
    public int playerMoney { get; private set; } = 0;
    [Range(1.0f, 50.0f)]
    [SerializeField] int moneyRewardMin = 50; // The minimum amount of money the player can get from robbing a civilian
    [Range(51.0f, 150.0f)]
    [SerializeField] int moneyRewardMax = 150; // The maximum amount of money the player can get from robbing a civilian

    void Start()
    {
        playerReputation = maximumReputation / 2; // Set the player reputation to 50% of the max
    }

    // Update is called once per frame
    void Update()
    {
        ToggleMask();
        HandleRobbing();
    }

    void HandleRobbing()
    {
        // Grab all the civilians in the game
        Civilian[] civilians = FindObjectsByType<Civilian>(FindObjectsSortMode.None);

        // Check if player is pressing the robbing key and they have their mask on
        if (Input.GetKeyDown(robbingKey) && maskState == MaskState.On)
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

    void ToggleMask()
    {
        if (Input.GetKeyDown(maskKey))
        {
            // If mask is off
            if (maskState == MaskState.Off) 
            {
                // Put on
                maskState = MaskState.On;
                print("Player put on mask");
            }
            else
            {
                // Take off mask
                maskState = MaskState.Off;
                print("Player removed mask");
            }

        }
    }

    public void AddReputation()
    {
        playerReputation += reputationReward;
    }

    public void AddMoney()
    { 
        int randomMoneyReward = Random.Range(Mathf.Abs(moneyRewardMin), Mathf.Abs(moneyRewardMax));
        playerMoney += randomMoneyReward;
    }
    public void LoseReputation()
    {
        playerReputation -= reputationReward;
    }

    public void AddReputation(int reputationToAdd)
    {
        playerReputation += reputationToAdd;
    }
    
    public void LoseReputation(int reputationToNegate)
    {
        playerReputation -= reputationToNegate;
    }

    public int GetPlayerReputation()
    {
        return playerReputation;
    }

    public int GetPlayerMaxReputation()
    {
        return maximumReputation;
    }
}
