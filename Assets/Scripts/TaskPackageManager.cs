using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class GridPosition
{
    public float x, y, z;
    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[System.Serializable]
public class TaskConfig
{
    public string configName;
    public string taskType;
    public int taskPart;
    public List<GridPosition> rewardPositions;
    public bool IsBackw => taskType != null && taskType.StartsWith("backw");
}

[System.Serializable]
public class PackageData
{
    public List<TaskConfig> tasks;
    public int trialsPerTask;
    public float gridStepSize;
}

[DefaultExecutionOrder(-10)] //V: ensures the Awake() in this script runs before any other Awake()

public class TaskPackageManager : MonoBehaviour
{
    public static TaskPackageManager Instance { get; private set; }
    public int AssignedPackageNumber { get; private set; }
    public PackageData Data { get; private set; }

    private List<TaskConfig> _part1Tasks;
    private List<TaskConfig> _part2Tasks;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        AssignAndLoadPackage();
    }

    void Start()
    {
        if (WebDataLogger.Instance != null)
            WebDataLogger.Instance.SetPackageNumber(AssignedPackageNumber);
    }

    void AssignAndLoadPackage()
    {
        AssignedPackageNumber = Random.Range(1, 4);
        string resourcePath = $"Tasks/p{AssignedPackageNumber:D2}";
        TextAsset packageFile = Resources.Load<TextAsset>(resourcePath);

        if (packageFile == null)
        {
            Debug.LogError($"[PackageManager] Could not load: Resources/{resourcePath}");
            return;
        }

        Data = JsonUtility.FromJson<PackageData>(packageFile.text);
        Debug.Log($"[PackageManager] Assigned package {AssignedPackageNumber}, loaded {Data.tasks.Count} tasks");
        GenerateTaskOrders();
    }

    void GenerateTaskOrders()
    {
        _part1Tasks = Data.tasks
            .Where(t => t.taskPart == 1)
            .OrderBy(_ => Random.value)
            .ToList();

        _part2Tasks = Data.tasks
            .Where(t => t.taskPart == 2)
            .OrderBy(_ => Random.value)
            .ToList();

        Debug.Log($"[PackageManager] Part 1 order: {string.Join(", ", _part1Tasks.Select(t => t.configName))}");
        Debug.Log($"[PackageManager] Part 2 order: {string.Join(", ", _part2Tasks.Select(t => t.configName))}");
    }

    public List<TaskConfig> GetPart1Tasks() => _part1Tasks;
    public List<TaskConfig> GetPart2Tasks() => _part2Tasks;
    public string GetPackageId() => $"p{AssignedPackageNumber:D2}";
}
