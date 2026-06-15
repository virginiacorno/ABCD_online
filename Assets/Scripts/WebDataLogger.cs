using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class WebDataLogger : MonoBehaviour
{
    private static WebDataLogger _instance;
    public static WebDataLogger Instance => _instance;

    [DllImport("__Internal")]
    private static extern void SendDataToJS(string jsonData);

    [System.Serializable]
    private class LogRow
    {
        // Always populated — matches fMRI column names
        public string event_type;
        public string participant_id;
        public string study_id;
        public string session_id;
        public string date;
        public double t_global;

        // Screen
        public string screen_name;
        public string phase;

        // Movement / step — all names match fMRI
        public string movement_type;
        public double t_step_press_global;
        public double t_step_press_curr_run;
        public float length_step;
        public float curr_loc_x;
        public float curr_loc_z;
        public float to_loc_x;
        public float to_loc_z;
        public double t_step_end_global;
        public string key_pressed; // online addition

        // Rotation — matches fMRI
        public float from_rotation;
        public float to_rotation;

        // Reward position and trial context — matches fMRI
        public float curr_rew_x;
        public float curr_rew_z;
        public string state;
        public string type;
        public int trial;
        public string task;

        // Reward check — matches fMRI; online adds player_steps/shortest_path/is_optimal
        public float distance;
        public double t_reward_start;
        public float reward_delay;
        public bool reward_found;
        public int player_steps;
        public int shortest_path;
        public bool is_optimal;

        // Online-only events below
        public string reward_letter;
        public int reward_index;
        public int config_index;
        public string config_name;
        public int repetition_number;
        public float display_time;
        public float pause_time;
        public float start_loc_x;
        public float start_loc_y;
        public float start_loc_z;
        public double cue_time;
        public int rep;
        public string trial_type;
        public string sequence;
        public int reps_completed;
        public int total_reps;
        public int package_number;
        public string package_id;
    }

    private string participantId;
    private string studyId;
    private string sessionId;
    public double TrialStartTime { get; private set; }

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        Debug.Log("[DATALOGGER] WebDataLogger started, bridge ready.");
    }

    public static double Timestamp() => (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

    /// Called from JavaScript to initialise participant info
    /// e.g., unityInstance.SendMessage('WebDataLogger', 'SetParticipantInfo', 'PID|STUDY|SESSION');
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
        Send(new LogRow { event_type = "screen", screen_name = screenName, phase = phase });
    }

    // Matches fMRI LogStep — called once when movement completes, carrying both press and end times
    public void LogStep(double tStepPressGlobal, double tStepPressCurrRun,
        float currLocX, float currLocZ, float toLocX, float toLocZ, double tStepEndGlobal,
        float currRewX, float currRewZ, string state, string type, int trial, string task,
        string keyPressed = "up")
    {
        Send(new LogRow
        {
            event_type = "movement", movement_type = "step",
            t_step_press_global = tStepPressGlobal, t_step_press_curr_run = tStepPressCurrRun,
            curr_loc_x = currLocX, curr_loc_z = currLocZ,
            to_loc_x = toLocX, to_loc_z = toLocZ,
            t_step_end_global = tStepEndGlobal,
            curr_rew_x = currRewX, curr_rew_z = currRewZ,
            state = state, type = type, trial = trial, task = task,
            key_pressed = keyPressed
        });
    }

    // Matches fMRI LogRotation — called once when rotation completes
    public void LogRotation(double rotPressGlobal, double rotPressCurrRun,
        float fromRotation, float toRotation,
        float currLocX, float currLocZ, float currRewX, float currRewZ,
        string state, string type, int trial, string task, string keyPressed = "")
    {
        Send(new LogRow
        {
            event_type = "movement", movement_type = "rotation",
            t_step_press_global = rotPressGlobal, t_step_press_curr_run = rotPressCurrRun,
            from_rotation = fromRotation, to_rotation = toRotation,
            curr_loc_x = currLocX, curr_loc_z = currLocZ,
            curr_rew_x = currRewX, curr_rew_z = currRewZ,
            state = state, type = type, trial = trial, task = task,
            key_pressed = keyPressed
        });
    }

    // Matches fMRI LogRewardCheck signature
    public void LogRewardCheck(float currLocX, float currLocZ, float currRewX, float currRewZ,
        float distance, string state, string type, int trial, string task,
        bool rewardFound, double tRewardStart = 0, float rewardDelay = 0f,
        int playerSteps = 0, int shortestPath = 0, bool isOptimal = false)
    {
        Send(new LogRow
        {
            event_type = "reward_check",
            curr_loc_x = currLocX, curr_loc_z = currLocZ,
            curr_rew_x = currRewX, curr_rew_z = currRewZ,
            distance = distance, state = state, type = type, trial = trial, task = task,
            reward_found = rewardFound, t_reward_start = tRewardStart, reward_delay = rewardDelay,
            player_steps = playerSteps, shortest_path = shortestPath, is_optimal = isOptimal
        });
    }

    public void LogRewardEvent(string phase, Vector3 rewPos, string rewardLetter, int rewardIndex, int configIndex, string state)
    {
        Send(new LogRow
        {
            event_type = "reward_event", phase = phase,
            curr_rew_x = rewPos.x, curr_rew_z = rewPos.z,
            reward_letter = rewardLetter, reward_index = rewardIndex,
            config_index = configIndex, state = state
        });
    }

    public void LogCue(int round, int rep)
    {
        double now = Timestamp();
        Send(new LogRow { event_type = "cue_displayed", trial = round, rep = rep, cue_time = now - TrialStartTime });
    }

    public void LogConfigurationStart(int configIndex, string configName)
    {
        Send(new LogRow { event_type = "configuration_start", config_index = configIndex, config_name = configName });
    }

    public void LogMemorizationStart(string configName, int repetitions)
    {
        Send(new LogRow { event_type = "memorization", phase = "start", config_name = configName, repetition_number = repetitions });
    }

    public void LogMemorizationRepetition(int repNum, float displayTime, float pauseTime)
    {
        Send(new LogRow { event_type = "memorization", phase = "rep_start", repetition_number = repNum, display_time = displayTime, pause_time = pauseTime });
    }

    public void LogMemorizationReward(string rewardPhase, string rewardLetter, int rewardIndex, int repNum)
    {
        Send(new LogRow { event_type = "memorization", phase = rewardPhase, reward_letter = rewardLetter, reward_index = rewardIndex, repetition_number = repNum });
    }

    public void LogMemorizationComplete()
    {
        Send(new LogRow { event_type = "memorization", phase = "complete" });
    }

    public void LogBackwardWarning(string phase, string configName)
    {
        Send(new LogRow { event_type = "backward_warning", phase = phase, config_name = configName });
    }

    public void LogGamePhaseStart(Vector3 startPos, string configName, int configIndex)
    {
        TrialStartTime = Timestamp();
        Send(new LogRow
        {
            event_type = "game_phase_start",
            start_loc_x = startPos.x, start_loc_y = startPos.y, start_loc_z = startPos.z,
            config_name = configName, config_index = configIndex
        });
    }

    public void LogRepetitionComplete(int configIndex, int repsCompleted, int totalReps)
    {
        Send(new LogRow { event_type = "repetition_complete", config_index = configIndex, reps_completed = repsCompleted, total_reps = totalReps });
    }

    public void LogTrialStartEvent(int configIndex, string configName, string trialType, string sequence, int rep)
    {
        TrialStartTime = Timestamp();
        Send(new LogRow { event_type = "trial_start", config_index = configIndex, config_name = configName, trial_type = trialType, sequence = sequence, rep = rep });
    }

    public void LogPackageAssignment(int packageNumber, string packageId)
    {
        Send(new LogRow { event_type = "package_assignment", package_number = packageNumber, package_id = packageId });
    }

    public void TriggerInactivityTimeout()
    {
        Debug.Log("ABCD_TIMEOUT");
    }

    private void Send(LogRow row)
    {
        row.participant_id = participantId;
        row.study_id = studyId;
        row.session_id = sessionId;
        row.date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        row.t_global = Timestamp();

        string json = JsonUtility.ToJson(row);
        Debug.Log("[WEBGL_DATA] " + json);

        #if UNITY_WEBGL && !UNITY_EDITOR
        try
        {
            SendDataToJS(json);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to send data to JS: {e.Message}");
        }
        #endif
    }
}