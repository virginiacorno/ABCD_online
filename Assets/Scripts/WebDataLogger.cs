
using UnityEngine;
using System.Runtime.InteropServices;
using System;

/// Logs ABCD task data & sends it to JavaScript for Pavlovia storage
public class WebDataLogger : MonoBehaviour
{
    private static WebDataLogger _instance;
    public static WebDataLogger Instance => _instance;

    [DllImport("__Internal")]
    private static extern void SendDataToJS(string jsonData);

    [System.Serializable]
    public class KeyPressData
    {
        public string event_type = "key_press";
        public string participant;
        public string study_id;
        public string session_id;
        public string session = "001";
        public string date;
        public int round;
        public int rep;
        public double t_step_press_global;
        public double t_step_press_curr_run;
        public string key_pressed;
        public int key_index;
        public float curr_loc_x;
        public float curr_loc_z;
        public double t_global;
    }

    [System.Serializable]
    public class MovementData
    {
        public string event_type = "movement";
        public string participant;
        public string study_id;
        public string session_id;
        public string session = "001";
        public string date;
        public int round;
        public int rep;
        public bool movement_complete = true;
        public float curr_loc_x;
        public float curr_loc_y;
        public float curr_loc_z;
        public string phase;
        public float from_x;
        public float from_y;
        public float from_z;
        public float to_x;
        public float to_z;
        public double t_global;
        public double t_step_from_start_currrun;
        public double t_step_end_global;
        public double t_step_tglobal;
        public double length_step;
        public string direction;
        public float curr_rew_x;
        public float curr_rew_y;
        public float curr_rew_z;
        public string type;
        public string state;
        public bool found_reward;
        public int movement_index;
    }

    [System.Serializable]
    public class ScreenEventData
    {
        public string event_type = "screen_event";
        public string participant;
        public string study_id;
        public string session_id;
        public string session = "001";
        public string date;
        public string screen_name;
        public string phase; // "onset", "button_press", "offset"
        public double t_global;
    }

    [System.Serializable]
    public class CueData
    {
        public string event_type = "cue_displayed";
        public string participant;
        public string study_id;
        public string session_id;
        public string session = "001";
        public string date;
        public int round;
        public int rep;
        public double cue_time;
        public double cue_time_global;
    }

    [System.Serializable]
    public class RotationData
    {
        public string event_type = "rotation";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public string phase; // "start" or "complete"
        public float from_rotation;
        public float to_rotation;
        public double t_global;
    }

    [System.Serializable]
    public class CameraTransitionData
    {
        public string event_type = "camera_transition";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public string phase; // "start" or "complete"
        public float player_loc_x;
        public float player_loc_z;
        public double t_global;
    }

    [System.Serializable]
    public class ConfigurationStartData
    {
        public string event_type = "configuration_start";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public int config_index;
        public string config_name;
        public double t_global;
    }

    [System.Serializable]
    public class MemorizationData
    {
        public string event_type = "memorization";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public string phase; // "start", "rep_start", "reward_onset", "reward_offset", "complete"
        public string config_name;
        public int repetition_number;
        public float display_time;
        public float pause_time;
        public string reward_letter;
        public int reward_index;
        public double t_global;
    }

    [System.Serializable]
    public class BackwardWarningData
    {
        public string event_type = "backward_warning";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public string phase; // "onset" or "offset"
        public string config_name;
        public double t_global;
    }

    [System.Serializable]
    public class GamePhaseStartData
    {
        public string event_type = "game_phase_start";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public float start_loc_x;
        public float start_loc_y;
        public float start_loc_z;
        public string config_name;
        public int config_index;
        public double t_global;
    }

    [System.Serializable]
    public class RewardCheckData
    {
        public string event_type = "reward_check";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public float player_loc_x;
        public float player_loc_z;
        public float rew_loc_x;
        public float rew_loc_z;
        public string state;
        public string config_name;
        public float distance_to_reward;
        public bool found_reward;
        public int player_steps;
        public int shortest_path;
        public bool is_optimal;
        public double t_global;
    }

    [System.Serializable]
    public class RewardEventData
    {
        public string event_type = "reward_event";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public string phase; // "onset" or "offset"
        public float rew_loc_x;
        public float rew_loc_z;
        public string reward_letter;
        public int reward_index;
        public int config_index;
        public string state;
        public double t_global;
    }

    [System.Serializable]
    public class RepetitionCompleteData
    {
        public string event_type = "repetition_complete";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public int config_index;
        public int reps_completed;
        public int total_reps;
        public double t_global;
    }

    [System.Serializable]
    public class TrialStartEventData
    {
        public string event_type = "trial_start";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public int config_index;
        public string config_name;
        public string trial_type;
        public string sequence;
        public int rep;
        public double t_global;
    }

    [System.Serializable]
    public class PackageAssignmentData
    {
        public string event_type = "package_assignment";
        public string participant; public string study_id; public string session_id;
        public string session = "001"; public string date;
        public int package_number;
        public string package_id;
        public double t_global;
    }

    // Participant info (set from JavaScript on startup)
    private string participantId;
    private string studyId;
    private string sessionId;
    
    // Trial tracking
    private double trialStartTime;
    private double experimentStartTime;

    void Start()
    {
        Debug.Log("[DATALOGGER] WebDataLogger started, bridge ready.");
    }
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
        experimentStartTime = GetUnixTimestamp();
    }

    /// Called from JavaScript to initialise participant info
    /// e.g., unityInstance.SendMessage('DataLogger', 'SetParticipantInfo', 'PID|STUDY|SESSION');
    public void SetParticipantInfo(string info)
    {
        string[] parts = info.Split('|');
        if (parts.Length >= 3)
        {
            participantId = parts[0];
            studyId = parts[1];
            sessionId = parts[2];
            Debug.Log($"DataLogger initialised: PID={participantId}, STUDY={studyId}, SESSION={sessionId}");
        }
    }

    public void LogScreenEvent(string screenName, string phase)
    {
        var data = new ScreenEventData
        {
            participant = participantId,
            study_id = studyId,
            session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            screen_name = screenName,
            phase = phase,
            t_global = GetUnixTimestamp()
        };

        SendToJavaScript(data);
    }

    public void LogCue(int round, int rep) //V: maybe more relevant for fMRI (tho player is blocked for a few seconds)
    {
        double now = GetUnixTimestamp();

        var data = new CueData
        {
            participant = participantId,
            study_id = studyId,
            session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            round = round,
            rep = rep,
            cue_time = now - trialStartTime,
            cue_time_global = now
        };

        SendToJavaScript(data);
    }

    public void LogRotation(string phase, float fromRotation, float toRotation)
    {
        SendToJavaScript(new RotationData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = phase, from_rotation = fromRotation, to_rotation = toRotation,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogCameraTransition(string phase, Vector3 playerPos) //V: maybe more relevant to fMRI
    {
        SendToJavaScript(new CameraTransitionData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = phase, player_loc_x = playerPos.x, player_loc_z = playerPos.z,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogConfigurationStart(int configIndex, string configName)
    {
        SendToJavaScript(new ConfigurationStartData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            config_index = configIndex, config_name = configName,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogMemorizationStart(string configName, int repetitions) //V: maybe more relevant to fMRI
    {
        SendToJavaScript(new MemorizationData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = "start", config_name = configName, repetition_number = repetitions,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogMemorizationRepetition(int repNum, float displayTime, float pauseTime) //V: maybe more relevant to fMRI
    {
        SendToJavaScript(new MemorizationData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = "rep_start", repetition_number = repNum,
            display_time = displayTime, pause_time = pauseTime,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogMemorizationReward(string rewardPhase, string rewardLetter, int rewardIndex, int repNum) //V: maybe more relevant to fMRI
    {
        SendToJavaScript(new MemorizationData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = rewardPhase, reward_letter = rewardLetter,
            reward_index = rewardIndex, repetition_number = repNum,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogMemorizationComplete() //V: maybe more relevant to fMRI
    {
        SendToJavaScript(new MemorizationData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = "complete", t_global = GetUnixTimestamp()
        });
    }

    public void LogBackwardWarning(string phase, string configName) //V: maybe more relevant to fMRI + no backw tasks in this pilot
    {
        SendToJavaScript(new BackwardWarningData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = phase, config_name = configName,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogGamePhaseStart(Vector3 startPos, string configName, int configIndex)
    {
        trialStartTime = GetUnixTimestamp();
        SendToJavaScript(new GamePhaseStartData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            start_loc_x = startPos.x, start_loc_y = startPos.y, start_loc_z = startPos.z,
            config_name = configName, config_index = configIndex,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogRewardCheck(Vector3 playerPos, Vector3 rewPos, string state, string configName, float distance, bool found, int playerSteps = 0, int shortestPath = 0, bool isOptimal = false)
    {
        SendToJavaScript(new RewardCheckData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            player_loc_x = playerPos.x, player_loc_z = playerPos.z,
            rew_loc_x = rewPos.x, rew_loc_z = rewPos.z,
            state = state, config_name = configName,
            distance_to_reward = distance, found_reward = found,
            player_steps = playerSteps, shortest_path = shortestPath, is_optimal = isOptimal,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogRewardEvent(string phase, Vector3 rewPos, string rewardLetter, int rewardIndex, int configIndex, string state)
    {
        SendToJavaScript(new RewardEventData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = phase, rew_loc_x = rewPos.x, rew_loc_z = rewPos.z,
            reward_letter = rewardLetter, reward_index = rewardIndex,
            config_index = configIndex, state = state,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogRepetitionComplete(int configIndex, int repsCompleted, int totalReps)
    {
        SendToJavaScript(new RepetitionCompleteData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            config_index = configIndex, reps_completed = repsCompleted, total_reps = totalReps,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogTrialStartEvent(int configIndex, string configName, string trialType, string sequence, int rep)
    {
        trialStartTime = GetUnixTimestamp();
        SendToJavaScript(new TrialStartEventData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            config_index = configIndex, config_name = configName,
            trial_type = trialType, sequence = sequence, rep = rep,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogPackageAssignment(int packageNumber, string packageId)
    {
        SendToJavaScript(new PackageAssignmentData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            package_number = packageNumber, package_id = packageId,
            t_global = GetUnixTimestamp()
        });
    }

    public void TriggerInactivityTimeout()
    {
        Debug.Log("ABCD_TIMEOUT");
    }


    public void LogKeyPressEvent(string key, Vector3 playerPos, int round, int rep) //V: maybe more needed for fMRI (movements and space bar presses are already recorderd)
    {
        SendToJavaScript(new KeyPressData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            key_pressed = key,
            t_global = GetUnixTimestamp(),
            curr_loc_x = playerPos.x,
            curr_loc_z = playerPos.z,
            round = round,
            rep = rep
        });
    }

    public void LogMovementEvent(string phase, Vector3 fromPos, Vector3 toPos)
    {
        SendToJavaScript(new MovementData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = phase,
            from_x = fromPos.x, from_z = fromPos.z,
            to_x = toPos.x, to_z = toPos.z,
            t_global = GetUnixTimestamp()
        });
    }

    private void SendToJavaScript(object data)
    {
        string json = JsonUtility.ToJson(data);

        Debug.Log("[WEBGL_DATA] " + json);
        
        Debug.Log("[WEBGL_CALL] about to call SendDataToJS: " + json);
        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            SendDataToJS(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send data to JS: {e.Message}");
        }
        #else
        Debug.Log($"[WEBGL DATA]: {json}");
        #endif
    }

    private double GetUnixTimestamp()
    {
        return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
    }
}
