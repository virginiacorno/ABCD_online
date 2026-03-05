using UnityEngine;
using System.Collections.Generic;
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
    public class TrialStartData
    {
        public string event_type = "trial_start";
        public string participant;
        public string study_id;
        public string session_id;
        public string session = "001";
        public string date;
        public int round;
        public int rep;
        public double start_ABCD_screen;
        public string trial_type;
        public string sequence;
        public float start_loc_x;
        public float start_loc_y;
        public float start_loc_z;
    }

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
    public class RewardData
    {
        public string event_type = "reward";
        public string participant;
        public string study_id;
        public string session_id;
        public string session = "001";
        public string date;
        public int round;
        public int rep;
        public float rew_loc_x;
        public float rew_loc_y;
        public float rew_loc_z;
        public double t_reward_start;
        public double t_reward_start_global;
        public string reward_letter;
        public int reward_index;
        public string state;
        public int moves_to_find;
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

    // Participant info (set from JavaScript on startup)
    private string participantId;
    private string studyId;
    private string sessionId;
    
    // Trial tracking
    private double trialStartTime;
    private double experimentStartTime;
    private float taskStartTime;

    void Start()
    {
        // typed smoke test — this will serialize properly with JsonUtility
        var test = new RewardData {
            participant = participantId ?? "UNKNOWN",
            study_id = studyId ?? "",
            session_id = sessionId ?? "",
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            round = 0,
            rep = 0,
            rew_loc_x = 0f,
            rew_loc_y = 0f,
            rew_loc_z = 0f,
            t_reward_start = GetUnixTimestamp(),
            t_reward_start_global = GetUnixTimestamp(),
            reward_letter = "X",
            reward_index = -1,
            state = "test",
            moves_to_find = 0
        };
        SendToJavaScript(test);   // should yield a full JSON object in the iframe
        Debug.Log("[DATALOGGER TEST] Sent typed startup smoke_test");
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

    /// Matches DataLogger.InitializeWithInfo so game scripts can call either logger
    public void InitializeWithInfo(string participant, string study, string session)
    {
        participantId = participant;
        studyId = study;
        sessionId = session;
        taskStartTime = Time.realtimeSinceStartup;
        Debug.Log($"WebDataLogger initialised: PID={participantId}, STUDY={studyId}, SESSION={sessionId}");
    }

    /// Matches DataLogger.GetCurrentRunTime
    public float GetCurrentRunTime()
    {
        return Time.realtimeSinceStartup - taskStartTime;
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

    public void LogTrialStart(int round, int rep, string trialType, string sequence, Vector3 startPos)
    {
        trialStartTime = GetUnixTimestamp();
        
        var data = new TrialStartData
        {
            participant = participantId,
            study_id = studyId,
            session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            round = round,
            rep = rep,
            start_ABCD_screen = trialStartTime,
            trial_type = trialType,
            sequence = sequence,
            start_loc_x = startPos.x,
            start_loc_y = startPos.y,
            start_loc_z = startPos.z
        };

        SendToJavaScript(data);
    }

    public void LogKeyPress(int round, int rep, string key, int keyIndex)
    {
        double now = GetUnixTimestamp();
        
        var data = new KeyPressData
        {
            participant = participantId,
            study_id = studyId,
            session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            round = round,
            rep = rep,
            t_step_press_global = now,
            t_step_press_curr_run = now - trialStartTime,
            key_pressed = key,
            key_index = keyIndex
        };

        SendToJavaScript(data);
    }

    public void LogMovement(int round, int rep, Vector3 fromPos, Vector3 toPos, string direction,
                           Vector3 targetPos, string trialType, string state, bool foundReward, int movementIndex)
    {
        double now = GetUnixTimestamp();
        double stepStart = now - 0.1;
        
        var data = new MovementData
        {
            participant = participantId,
            study_id = studyId,
            session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            round = round,
            rep = rep,
            curr_loc_x = toPos.x,
            curr_loc_y = toPos.y,
            curr_loc_z = toPos.z,
            from_x = fromPos.x,
            from_y = fromPos.y,
            from_z = fromPos.z,
            t_step_from_start_currrun = stepStart - trialStartTime,
            t_step_end_global = now,
            t_step_tglobal = stepStart,
            length_step = now - stepStart,
            direction = direction,
            curr_rew_x = targetPos.x,
            curr_rew_y = targetPos.y,
            curr_rew_z = targetPos.z,
            type = trialType,
            state = state,
            found_reward = foundReward,
            movement_index = movementIndex
        };

        SendToJavaScript(data);
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

    public void LogCue(int round, int rep)
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

    public void LogReward(int round, int rep, Vector3 rewardPos, string rewardLetter,
                         int rewardIndex, string state, int movesToFind)
    {
        double now = GetUnixTimestamp();
        
        var data = new RewardData
        {
            participant = participantId,
            study_id = studyId,
            session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            round = round,
            rep = rep,
            rew_loc_x = rewardPos.x,
            rew_loc_y = rewardPos.y,
            rew_loc_z = rewardPos.z,
            t_reward_start = now - trialStartTime,
            t_reward_start_global = now,
            reward_letter = rewardLetter,
            reward_index = rewardIndex,
            state = state,
            moves_to_find = movesToFind
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

    public void LogCameraTransition(string phase, Vector3 playerPos)
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

    public void LogMemorizationStart(string configName, int repetitions)
    {
        SendToJavaScript(new MemorizationData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = "start", config_name = configName, repetition_number = repetitions,
            t_global = GetUnixTimestamp()
        });
    }

    public void LogMemorizationRepetition(int repNum, float displayTime, float pauseTime)
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

    public void LogMemorizationReward(string rewardPhase, string rewardLetter, int rewardIndex, int repNum)
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

    public void LogMemorizationComplete()
    {
        SendToJavaScript(new MemorizationData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            phase = "complete", t_global = GetUnixTimestamp()
        });
    }

    public void LogBackwardWarning(string phase, string configName)
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

    public void LogRewardCheck(Vector3 playerPos, Vector3 rewPos, string state, string configName, float distance, bool found)
    {
        SendToJavaScript(new RewardCheckData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            player_loc_x = playerPos.x, player_loc_z = playerPos.z,
            rew_loc_x = rewPos.x, rew_loc_z = rewPos.z,
            state = state, config_name = configName,
            distance_to_reward = distance, found_reward = found,
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

    public void LogKeyPressEvent(string key, Vector3 playerPos)
    {
        SendToJavaScript(new KeyPressData
        {
            participant = participantId, study_id = studyId, session_id = sessionId,
            date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            key_pressed = key,
            t_global = GetUnixTimestamp()
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

    /// Same interface as DataLogger.LogEvent — so game scripts can call either logger
    public void LogEvent(Dictionary<string, object> data)
    {
        if (!data.ContainsKey("participant")) data["participant"] = participantId;
        if (!data.ContainsKey("study_id")) data["study_id"] = studyId;
        if (!data.ContainsKey("session_id")) data["session_id"] = sessionId;
        if (!data.ContainsKey("session")) data["session"] = "001";
        if (!data.ContainsKey("date")) data["date"] = DateTime.UtcNow.ToString("yyyy-MM-dd");
        if (!data.ContainsKey("t_global")) data["t_global"] = GetUnixTimestamp();

        // Build JSON manually since JsonUtility doesn't support Dictionary
        var entries = new List<string>();
        foreach (var kvp in data)
        {
            string value = kvp.Value != null ? kvp.Value.ToString() : "";
            // Numbers and booleans without quotes, strings with quotes
            if (kvp.Value is int || kvp.Value is float || kvp.Value is double)
                entries.Add($"\"{kvp.Key}\":{value}");
            else if (kvp.Value is bool b)
                entries.Add($"\"{kvp.Key}\":{(b ? "true" : "false")}");
            else
                entries.Add($"\"{kvp.Key}\":\"{value.Replace("\"", "\\\"")}\"");
        }
        string json = "{" + string.Join(",", entries) + "}";

        Debug.Log("[WEBGL_DATA] " + json);
        #if UNITY_WEBGL && !UNITY_EDITOR
        try { SendDataToJS(json); }
        catch (Exception e) { Debug.LogError($"Failed to send data to JS: {e.Message}"); }
        #else
        Debug.Log($"[WEBGL DATA]: {json}");
        #endif
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
