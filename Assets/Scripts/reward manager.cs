using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem; 

public class rewardManager : MonoBehaviour
{

    [System.Serializable]
    public class GridPosition
    {
        public float x;  // Unity X (left/right)
        public float y;  // Unity Y (height)
        public float z;  // Unity Z (forward/back)
        
        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }
    
    [System.Serializable]
    public class RewardConfiguration
    {
        public string configName;
        public List<GridPosition> rewardPositions;

        //V: determine seqeunce length for the ABCD to ABC variant
        public int SequenceLength => configName.StartsWith("ABC") && !configName.StartsWith("ABCD") ? 3 : 4; //V: if the name starts with ABC and not ABCD, then length = 3, otherwise = 4
        public bool IsABCType => SequenceLength == 3;

        //V: determine if it's a forward or backward trial
        public bool IsBackw => configName.StartsWith("backw") == true;
    }
    
    [System.Serializable]
    public class ConfigurationData
    {
        public List<RewardConfiguration> configurations;
        public int trialsPerConfig;
    }
    
    [Header("Configuration File")]
    public TextAsset configurationFile;
    
    [Header("Reward Prefab")]
    public GameObject rewardPrefab;

    [Header("UI References")]
    public TaskInstructionManagerBase instructionManager;
    
    private ConfigurationData configData;
    private GameObject[] currentRewardObjects; //V: array containing sequence of rewards
    private int currentConfigIdx = 0;
    private int nextRewardIdx = 0;
    private int repsCompleted = 0;
    private int lastShownRewardIdx = -1;
    public GameObject cueObject;
    public moveplayer player;
    public bool isPractice = false;
    private bool returnToA = false; 
    
    void Awake() //V: Awake() takes precedence over any Start() in any of the scripts, so we make sure all rewards are hidden before starting 
    {
        LoadConfigurationsFromFile();
        
        if (configData != null && configData.configurations.Count > 0)
        {
            LoadConfiguration(0);
            HideCue();
            Debug.Log("Awake complete - rewards created and hidden");
        }
        else
        {
            Debug.LogError("No configurations loaded!");
        }
    }

    void Start()
    {
        if (configData != null && configData.configurations.Count > 0)
        {
            Debug.Log($"Starting {configData.configurations[currentConfigIdx].configName}");
            Debug.Log($"Total configurations loaded: {configData.configurations.Count}");
        }
    }

    void LoadConfigurationsFromFile()
    {
        if (configurationFile == null)
        {
            Debug.LogError("Configuration file not assigned!");
            return;
        }
        
        try
        {
            configData = JsonUtility.FromJson<ConfigurationData>(configurationFile.text);
            Debug.Log($"Loaded {configData.configurations.Count} configurations from file");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load configuration file: {e.Message}");
        }
    }

    //V: need repetition to ensure some delay between end of previous trial and loading new configurations
    public void LoadConfiguration()
    {
        LoadConfiguration(currentConfigIdx);
    }

    public void LoadConfiguration(int index)
    {
        if (index >= 0 && index < configData.configurations.Count)
        {
            currentConfigIdx = index;
            nextRewardIdx = GetStartIndex();
            lastShownRewardIdx = -1;
            returnToA = false;

            // Destroy old rewards
            if (currentRewardObjects != null)
            {
                foreach (GameObject reward in currentRewardObjects)
                {
                    if (reward != null)
                        Destroy(reward);
                }
            }
            
            List<GridPosition> positions = configData.configurations[index].rewardPositions;
            
            // Create new rewards at specified positions
            currentRewardObjects = new GameObject[positions.Count];
            for (int i = 0; i < positions.Count; i++)
            {
                Vector3 worldPos = positions[i].ToVector3();
                currentRewardObjects[i] = Instantiate(rewardPrefab, worldPos, Quaternion.identity);
                currentRewardObjects[i].name = $"Reward_{(char)('A' + i)}_{configData.configurations[index].configName}";
                //currentRewardObjects[i].GetComponent<Renderer>().enabled = false;
                currentRewardObjects[i].SetActive(false);

                Debug.Log($"Reward {(char)('A' + i)} at world position: {worldPos}");
            }
            
            // Reposition player to the start position for this config
            player.SetPosition(GetStartPosition());

            Debug.Log($"Loaded {configData.configurations[index].configName}");
        }
    }

    int GetStartIndex()
    {
        var config = configData.configurations[currentConfigIdx];
        return config.IsBackw ? config.SequenceLength - 1 : 0; //V: if the current config is a backward trial, return number corresponding to last reward index, otherwise return 0
    }
    
    public int GetTotalConfigurations()
    {
        return configData.configurations.Count;
    }
    
    public string GetCurrentConfigName()
    {
        return configData.configurations[currentConfigIdx].configName;
    }

    public int GetCurrentConfigIndex()
    {
        return currentConfigIdx;
    }
    
    public bool RewardFound(Vector3 playerPosition)
    {
        if (isPractice) return false;

        Debug.Log($"Player position: {playerPosition}");
        Debug.Log($"nextRewardIdx: {nextRewardIdx}");
        int rewardsToCollect = configData.configurations[currentConfigIdx].SequenceLength;
        
        if (nextRewardIdx >= rewardsToCollect || nextRewardIdx < 0) //V: < 0 in case we are in backward trials
        {
            return false;
        }
        
        GameObject currReward = currentRewardObjects[nextRewardIdx];
        float distance = Vector3.Distance(playerPosition, currReward.transform.position);

        Debug.Log($"Reward {nextRewardIdx} position: {currReward.transform.position}, Distance: {distance}");

        //V: check for space bar presses
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && keyboard.spaceKey.wasPressedThisFrame)
        {
            bool atRewardLocation = (distance < 0.01f);
            
            // log all space bar presses
            WebDataLogger.Instance.LogRewardCheck(
                playerPosition,
                currReward.transform.position,
                ((char)('A' + nextRewardIdx)).ToString(),
                configData.configurations[currentConfigIdx].configName,
                distance,
                atRewardLocation
            );
            
            // Only process reward if at correct location
            if (atRewardLocation)
            {
                if (returnToA) //V: if this was set true before, turn it off, show the reward and then increase reps completed
                {
                    returnToA = false;
                    ShowReward(0);
                    lastShownRewardIdx = 0;
                    player.inputEnabled = false;
                    repsCompleted++;

                    WebDataLogger.Instance.LogRepetitionComplete(currentConfigIdx, repsCompleted, configData.trialsPerConfig);

                    Invoke("CompleteTrial", 0.5f);
                    return true;
                }

                Debug.Log("spacebar was pressed at reward location");
                var config = configData.configurations[currentConfigIdx];
                int rewardCount = config.SequenceLength;
                Debug.Log($"Reward {nextRewardIdx + 1}/{rewardCount} found!");
                
                ShowReward(nextRewardIdx);
                lastShownRewardIdx = nextRewardIdx;
                
                nextRewardIdx += config.IsBackw ? -1 : 1; //V: if it's a backward trial, subtract 1 (otherwise add 1)

                if (configData.configurations[currentConfigIdx].IsABCType)
                {
                    if (repsCompleted != 0 && nextRewardIdx == 1)
                    {
                        StartCoroutine(ShowCue());
                    }
                }            

                if (repsCompleted == configData.trialsPerConfig - 1) //V: code need to return to reward A in the last repetition of a configuration in order to increase the reps completed
                {
                    if (nextRewardIdx >= rewardsToCollect || nextRewardIdx < 0)
                    {
                        Debug.Log("return to A");
                        returnToA = true;
                        nextRewardIdx = 0;
                    }
                } else if (nextRewardIdx >= rewardsToCollect || nextRewardIdx < 0)
                {
                    player.inputEnabled = false;
                    repsCompleted++;

                    WebDataLogger.Instance.LogRepetitionComplete(currentConfigIdx, repsCompleted, configData.trialsPerConfig);

                    Invoke("CompleteTrial", 0.5f);
                }
                
                return true;  
            }
            else
            {
                Debug.Log($"Space pressed but not at reward. Distance: {distance}");
                return false;
            }
        }
        
        // Handle hiding rewards when player moves away
        if (lastShownRewardIdx >= 0)
        {
            GameObject lastReward = currentRewardObjects[lastShownRewardIdx];
            float distanceToLast = Vector3.Distance(playerPosition, lastReward.transform.position);

            if (distanceToLast > 0.05f)
            {
                HideReward(lastShownRewardIdx);
                lastShownRewardIdx = -1;
            }
        }
        
        return false;
    }    

    public void CompleteTrial() //V: check if we have completed all repetitions of the current trial and switch to next configuration if appropriate
    {
        // Cue is hidden in ResetTrial() or StartNextConfigForFreeNav() after delay

        if (repsCompleted >= configData.trialsPerConfig)  
        {
            if (currentConfigIdx < configData.configurations.Count - 1)
            {
                Debug.Log($"{configData.configurations[currentConfigIdx].configName} complete!");
                currentConfigIdx++;
                repsCompleted = 0;

                CameraManager camManager = FindFirstObjectByType<CameraManager>();
                FreeNavigationCamera freeNavCam = FindFirstObjectByType<FreeNavigationCamera>();

                if (camManager != null && camManager.enabled)
                {
                    instructionManager.ShowFeedback(); 
                } 
                else if (freeNavCam != null && freeNavCam.enabled)
                {
                    LoadConfiguration(currentConfigIdx);
                    Debug.Log("Calling new sequence");
                    instructionManager.ShowFeedback();
                }
  
            }
            else
            {
                Debug.Log("All configurations completed!");
                instructionManager.EndScreen();
            }
        }
        else
        {
            Debug.Log($"Moving on to repetition {repsCompleted + 1}/3");
            Invoke("ResetTrial", 0.5f);
        }
    }


    public void StartNextConfiguration()
    {
        FindFirstObjectByType<CameraManager>().StartNewConfiguration(currentConfigIdx);
    }

    public void StartNextConfigForFreeNav()
    {
        // Reset for the new configuration
        HideAllRewards();
        nextRewardIdx = GetStartIndex();
        lastShownRewardIdx = -1;

        player.CameraController.SetupGameplayCameras();

        if (configData.configurations[currentConfigIdx].IsABCType)
            StartCoroutine(ShowCue());
        else
            player.inputEnabled = true;

        WebDataLogger.Instance.LogTrialStartEvent(
            currentConfigIdx,
            GetCurrentConfigName(),
            configData.configurations[currentConfigIdx].IsABCType ? "ABC" : "ABCD",
            configData.configurations[currentConfigIdx].IsABCType ? "A-B-C" : "A-B-C-D",
            repsCompleted
        );
        
        Debug.Log($"Starting {configData.configurations[currentConfigIdx].configName}");
    }
    
    void ResetTrial()
    {
        HideAllRewards();
        HideCue();
        nextRewardIdx = GetStartIndex();
        lastShownRewardIdx = -1;
        returnToA = false;

        player.inputEnabled = true;

        Debug.Log($"Starting trial {repsCompleted + 1}/{configData.trialsPerConfig} of Config {currentConfigIdx}");
    }

    public void ShowReward(int index)
    {
        Debug.Log($"ShowReward called with index: {index}");
        
        if (index >= 0 && index < currentRewardObjects.Length && currentRewardObjects[index] != null)
        {
            Debug.Log($"Showing reward at index {index}, name: {currentRewardObjects[index].name}");
            //Debug.Log($"Renderer before: {currentRewardObjects[index].GetComponent<Renderer>().enabled}");

            WebDataLogger.Instance.LogRewardEvent(
                "onset",
                currentRewardObjects[index].transform.position,
                ((char)('A' + index)).ToString(),
                index,
                currentConfigIdx,
                ((char)('A' + index)).ToString()
            );
            
            //currentRewardObjects[index].GetComponent<Renderer>().enabled = true;
            currentRewardObjects[index].SetActive(true);
            Vector3 dir = -player.transform.forward;
            dir.y = 0;
            currentRewardObjects[index].transform.rotation = Quaternion.LookRotation(dir);
            
            //Debug.Log($"Renderer after: {currentRewardObjects[index].GetComponent<Renderer>().enabled}");
        }
        else
        {
            Debug.LogError($"Cannot show reward at index {index}!");
        }
    }

    public void HideReward(int index)
    {
        if (index >= 0 && index < currentRewardObjects.Length && currentRewardObjects[index] != null)
        {
            WebDataLogger.Instance.LogRewardEvent(
                "offset",
                currentRewardObjects[index].transform.position,
                ((char)('A' + index)).ToString(),
                index,
                currentConfigIdx,
                ((char)('A' + index)).ToString()
            );

            //currentRewardObjects[index].GetComponent<Renderer>().enabled = false;
            currentRewardObjects[index].SetActive(false);
        }
    }
    
    void HideAllRewards()
    {
        if (currentRewardObjects != null)
        {
            foreach (GameObject reward in currentRewardObjects)
            {
                if (reward != null)
                {
                    //reward.GetComponent<Renderer>().enabled = false;
                    reward.SetActive(false);
                }
            }
        }
    }

    public void HideCue()
    {
        if (cueObject != null)
        {
            cueObject.SetActive(false);
        }
    }

    IEnumerator ShowCue()
    {
        //V: block the player
        player.inputEnabled = false;

        //V: show cue and log it 
        if (cueObject != null)
        {
            cueObject.SetActive(true);
            WebDataLogger.Instance.LogCue(currentConfigIdx, repsCompleted);
        }
        //V: block the player for 2 seconds so sure we see the cue
        yield return new WaitForSeconds(2f);

        //V: hide cue and re-enable movement
        HideCue();
        player.inputEnabled = true;

    }

    public Vector3 GetStartPosition()
    {
        var config = configData.configurations[currentConfigIdx];
        int lastRewardIdx = config.IsBackw ? 0 : config.SequenceLength - 1; //V: A (index 0) if backwards trial, C (index 2) if ABC and D (index 3) if ABCD
        return config.rewardPositions[lastRewardIdx].ToVector3();
    }

    public int GetCurrentRewardCount()
    {
        return configData.configurations[currentConfigIdx].rewardPositions.Count;
    }

    public Vector3 GetRewardWorldPosition(int idx)
    {
        if (currentRewardObjects != null && idx >= 0 && idx < currentRewardObjects.Length && currentRewardObjects[idx] != null)
            return currentRewardObjects[idx].transform.position;
        return Vector3.zero;
    }
}