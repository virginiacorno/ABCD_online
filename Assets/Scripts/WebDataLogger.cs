using UnityEngine;
using System.Runtime.InteropServices;
using System;
using System.IO;

public class WebDataLogger : MonoBehaviour
{
    private static WebDataLogger _instance;
    public static WebDataLogger Instance
    {
        get
        {
            #if UNITY_EDITOR
            if (_instance == null)
                _instance = new GameObject("WebDataLogger [Debug]").AddComponent<WebDataLogger>();
            #endif
            return _instance;
        }
    }

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

        public int package_number;
    }

    [Header("Debug")]
    public bool saveToCSV = false;

    private string participantId;
    private string studyId;
    private string sessionId;
    private int packageNumber;
    public double TrialStartTime { get; set; }

    #if UNITY_EDITOR
    private string _csvPath;
    private bool _csvHeaderWritten = false;
    #endif

    void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        #if UNITY_EDITOR
        if (saveToCSV)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            _csvPath = Path.Combine(Application.dataPath, "..", $"debug_log_{timestamp}.csv");
        }
        #endif
    }

    void Start()
    {
        Debug.Log("[DATALOGGER] WebDataLogger started, bridge ready.");
    }

    public static double Timestamp() => (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;

    /// Called from JavaScript to initialise participant info
    /// e.g., unityInstance.SendMessage('WebDataLogger', 'SetParticipantInfo', 'PID|STUDY|SESSION');
    public void SetPackageNumber(int num) { packageNumber = num; }

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

    public void TriggerInactivityTimeout()
    {
        Debug.Log("ABCD_TIMEOUT");
    }

    private void Send(LogRow row)
    {
        row.participant_id = participantId;
        row.study_id = studyId;
        row.session_id = sessionId;
        row.package_number = packageNumber;
        row.date = DateTime.UtcNow.ToString("yyyy-MM-dd");
        row.t_global = Timestamp();

        string json = JsonUtility.ToJson(row);
        Debug.Log("[WEBGL_DATA] " + json);

        #if UNITY_EDITOR
        if (saveToCSV)
            AppendToCSV(row);
        #endif

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

    #if UNITY_EDITOR
    void AppendToCSV(LogRow row)
    {
        if (!_csvHeaderWritten)
        {
            string header = "event_type,participant_id,study_id,session_id,date,t_global," +
                            "screen_name,phase," +
                            "movement_type,t_step_press_global,t_step_press_curr_run,length_step," +
                            "curr_loc_x,curr_loc_z,to_loc_x,to_loc_z,t_step_end_global,key_pressed," +
                            "from_rotation,to_rotation," +
                            "curr_rew_x,curr_rew_z,state,type,trial,task," +
                            "distance,t_reward_start,reward_delay,reward_found," +
                            "player_steps,shortest_path,is_optimal,package_number";
            File.WriteAllText(_csvPath, header + "\n");
            _csvHeaderWritten = true;
            Debug.Log($"[DATALOGGER] CSV logging to: {_csvPath}");
        }

        string line = $"{row.event_type},{row.participant_id},{row.study_id},{row.session_id},{row.date},{row.t_global}," +
                      $"{row.screen_name},{row.phase}," +
                      $"{row.movement_type},{row.t_step_press_global},{row.t_step_press_curr_run},{row.length_step}," +
                      $"{row.curr_loc_x},{row.curr_loc_z},{row.to_loc_x},{row.to_loc_z},{row.t_step_end_global},{row.key_pressed}," +
                      $"{row.from_rotation},{row.to_rotation}," +
                      $"{row.curr_rew_x},{row.curr_rew_z},{row.state},{row.type},{row.trial},{row.task}," +
                      $"{row.distance},{row.t_reward_start},{row.reward_delay},{row.reward_found}," +
                      $"{row.player_steps},{row.shortest_path},{row.is_optimal},{row.package_number}";
        File.AppendAllText(_csvPath, line + "\n");
    }
    #endif
}