using UnityEngine;
using UnityEngine.InputSystem;
public class moveplayer : MonoBehaviour
{
    private float _rotationFrom;

    public float gridStepSize = 10.3f;
    public float moveSpeed = 3.68f;
    public float rotationSpeed = 100f;

    public rewardManager rewardManager;

    [SerializeField] private MonoBehaviour _cameraController;
    public ICameraController CameraController => _cameraController as ICameraController;

    public bool inputEnabled = true; //V: allows to detect key input, turned off at the end of trials when transition screens/resets are called

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool isMoving = false;
    private bool isRotating = false;

    // Timestamps and key captured at press, used when the action completes
    private double _tStepPressGlobal;
    private Vector3 _positionAtPress;
    private double _rotationPressGlobal;
    private string _keyPressed;

    //V: step count for feedback
    public int stepCount = 0;

    void Start()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (isRotating) //V: first check if we are rotating/supposed to be rotating
        {
            RotateToTarget();
        }
        else if (!isMoving)
        {
            CheckInput();
            rewardManager.RewardFound(transform.position);
        }
        else if (isMoving)
        {
            MoveToTarget();
        }
    }

    public void SetPosition(Vector3 newPosition) //V: function to position the player on the grid as specified by parameters above
    {
        transform.position = newPosition;
        targetPosition = newPosition;
        transform.rotation = Quaternion.identity; //V: reset to initial facing direction (forward along +Z)
        targetRotation = Quaternion.identity;
        isMoving = false;
        isRotating = false;
    }

    void CheckInput() //V: check keyboard input and set the rotation and movement targets accordingly
    {
        if (!inputEnabled) return; //V: early return if input is disabled

        Keyboard keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.upArrowKey.wasPressedThisFrame) //V: up key is the only one allowing to move
        {
            Vector3 potentialTarget = transform.position + (transform.forward * gridStepSize);
            if (WithinBounds(potentialTarget))
            {
                _tStepPressGlobal = WebDataLogger.Timestamp();
                _positionAtPress = transform.position;
                _keyPressed = "up";
                targetPosition = potentialTarget;
                isMoving = true;
                stepCount++;
            }
            CameraController.DisableMiniMap();
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            _rotationPressGlobal = WebDataLogger.Timestamp();
            _keyPressed = "down";
            SetTarget(180f);
            CameraController.DisableMiniMap();
        }
        else if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            _rotationPressGlobal = WebDataLogger.Timestamp();
            _keyPressed = "left";
            SetTarget(-90f);
            CameraController.DisableMiniMap();
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            _rotationPressGlobal = WebDataLogger.Timestamp();
            _keyPressed = "right";
            SetTarget(90f);
            CameraController.DisableMiniMap();
        }
    }

    void SetTarget(float relativeYRotation) //V: calculate rotation target relative to current position and set isRotating to true
    {
        _rotationFrom = transform.rotation.eulerAngles.y;
        float newYRotation = _rotationFrom + relativeYRotation;
        targetRotation = Quaternion.Euler(0, newYRotation, 0);
        isRotating = true;
    }

    void RotateToTarget()
    {
        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.01f)
        {
            float y = Mathf.Round(targetRotation.eulerAngles.y / 90f) * 90f;
            transform.rotation = Quaternion.Euler(0, y, 0);
            isRotating = false;

            Vector3 rewPos = rewardManager.GetCurrentRewardPosition();
            WebDataLogger.Instance.LogRotation(
                _rotationPressGlobal,
                _rotationPressGlobal - WebDataLogger.Instance.TrialStartTime,
                _rotationFrom, transform.rotation.eulerAngles.y,
                transform.position.x, transform.position.z,
                rewPos.x, rewPos.z,
                rewardManager.GetCurrentState(),
                rewardManager.config.IsBackw ? "backw" : "forw",
                rewardManager.repsCompleted,
                rewardManager.GetCurrentConfigName(),
                _keyPressed
            );
        }
    }

    bool WithinBounds(Vector3 position) //V: check that we are within grid boundaries
    {
        float leftBound = -5.3f;
        float rightBound = 15.3f;
        float upBound = 25.6f; //V: for upper bounds we use z coordinates
        float bottomBound = 5f;
        float tolerance = 0.1f;

        return position.x > leftBound - tolerance &&
        position.x < rightBound + tolerance &&
        position.z < upBound + tolerance &&
        position.z > bottomBound - tolerance;
    }

    void MoveToTarget()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            transform.position = targetPosition;
            isMoving = false;
            Debug.Log($"Step duration: {WebDataLogger.Timestamp() - _tStepPressGlobal:F3}s");

            Vector3 rewPos = rewardManager.GetCurrentRewardPosition();
            WebDataLogger.Instance.LogStep(
                _tStepPressGlobal,
                _tStepPressGlobal - WebDataLogger.Instance.TrialStartTime,
                _positionAtPress.x, _positionAtPress.z,
                transform.position.x, transform.position.z,
                WebDataLogger.Timestamp(),
                rewPos.x, rewPos.z,
                rewardManager.GetCurrentState(),
                rewardManager.config.IsBackw ? "backw" : "forw",
                rewardManager.repsCompleted,
                rewardManager.GetCurrentConfigName(),
                _keyPressed
            );

            rewardManager.RewardFound(transform.position);
        }
    }
}
