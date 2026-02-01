using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;

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

    [Header("Audio")]
    [SerializeField] AudioClip Zip01;   // your mask sound
    [SerializeField] float maskSfxVolume = 1f;

    [SerializeField]
    AudioSource maskSFXSource;


    void Start()
    {
        playerReputation = maximumReputation / 2; // Set the player reputation to 50% of the max
        ToggleCivilianLights(false); // Hide the lights of the civilians at the beginning since player won't have mask on

        playerReputation = maximumReputation / 2;
        ToggleCivilianLights(false);
    }

    // Update is called once per frame
    void Update()
    {
        ToggleMask();
        HandleRobbing();
        HandleRobbingUI();
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

    void HandleRobbingUI()
    {
        // Grab all the civilians in the game
        Civilian[] civilians = FindObjectsByType<Civilian>(FindObjectsSortMode.None);
        HUDController HUDController = FindFirstObjectByType<HUDController>();

        if (HUDController != null)
        {
            int civilianInRange = 0;
            // Loop through all civilians in the game
            foreach (Civilian civilian in civilians)
            {
                if (civilian != null)
                {
                    float distanceToPlayer = Vector3.Distance(transform.position, civilian.transform.position);
                    if (distanceToPlayer <= robbingDistance)
                    {
                        civilianInRange++;
                    }
                }
            }

            if (civilianInRange > 0)
            {
                if (maskState == MaskState.On)
                {
                    // Enable the robbing UI
                    HUDController.ToggleRobUI(true);
                }
            }
            else
            {
                // Disable the robbing UI
                HUDController.ToggleRobUI(false);
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
                ToggleCivilianLights(true);
               
            }
            else
            {
                // Take off mask
                maskState = MaskState.Off;
                ToggleCivilianLights(false);
            }
            PlayMaskSound();
        }
    }

    void PlayMaskSound()
    {
        if (Zip01 == null && maskSFXSource == null) return;
        maskSFXSource.PlayOneShot(Zip01, maskSfxVolume);
    }


    void ToggleCivilianLights(bool toggle)
    {
        Civilian[] civilians = FindObjectsByType<Civilian>(FindObjectsSortMode.None);
        foreach (Civilian civilian in civilians)
        {
            Light visionLight = civilian.GetComponent<Light>();

            // If the vision light component is detected in the enemies
            if (visionLight != null)
            {
                // If you want to turn the lights on
                if (toggle)
                {
                    visionLight.enabled = true;
                }
                // Turn off the lights
                else
                {
                    visionLight.enabled = false;
                }
            }
        }
    }

    public void AddReputation()
    {
        playerReputation += reputationReward;

        // If player reached under 0 or 0 rep
        if (playerReputation >= maximumReputation)
        {
            EndScreenUI endScreenUI = FindFirstObjectByType<EndScreenUI>();
            if (endScreenUI != null)
            {
                endScreenUI.ShowWin(playerMoney);
            }
        }
    }

    public void AddMoney()
    { 
        int randomMoneyReward = Random.Range(Mathf.Abs(moneyRewardMin), Mathf.Abs(moneyRewardMax));
        playerMoney += randomMoneyReward;
    }
    public void LoseReputation()
    {
        playerReputation -= reputationReward;

        // If player reached under 0 or 0 rep
        if (playerReputation <= 0f)
        {
            EndScreenUI endScreenUI = FindFirstObjectByType<EndScreenUI>();
            if (endScreenUI != null)
            {
                endScreenUI.ShowLose(playerMoney);
            }
        }
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
