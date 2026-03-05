using UnityEngine;
using UnityEngine.InputSystem; 
public class moveplayer : MonoBehaviour
{
    private float _rotationFrom;
    private Vector3 _moveFrom;

    public float gridStepSize = 10.3f;
    public float moveSpeed = 5.0f;
    public float rotationSpeed = 100f;

    public rewardManager rewardManager;
    
    [SerializeField] private MonoBehaviour _cameraController;
    public ICameraController CameraController => _cameraController as ICameraController;

    public bool inputEnabled = true; //V: allows to detect key input, turned off at the end of trials when transition screens/resets are called

    private Vector3 targetPosition;
    private Quaternion targetRotation;
    private bool isMoving = false;
    private bool isRotating = false;
    
    //V: variables to keep track of logging
    private bool rotationStartLogged = false;
    private bool movementStartLogged = false;
    
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
        if (keyboard == null) return;  // Safety check

        string keyPressed = null;
        Vector3 oldPosition = transform.position;

        if (keyboard.upArrowKey.wasPressedThisFrame) //V: up key is the only one allowing to move, the other ones are just controlling rotations
        {
            Vector3 potentialTarget = transform.position + (transform.forward * gridStepSize);
            if (WithinBounds(potentialTarget))
            {
                targetPosition = potentialTarget;
                isMoving = true;
            }
            CameraController.DisableMiniMap();
            keyPressed = "up";
        }
        else if (keyboard.downArrowKey.wasPressedThisFrame)
        {
            SetTarget(180f);
            CameraController.DisableMiniMap();
            keyPressed = "down";
        }
        else if (keyboard.leftArrowKey.wasPressedThisFrame)
        {
            SetTarget(-90f);
            CameraController.DisableMiniMap();
            keyPressed = "left";
        }
        else if (keyboard.rightArrowKey.wasPressedThisFrame)
        {
            SetTarget(90f);
            CameraController.DisableMiniMap();
            keyPressed = "right";
        }

        if (!string.IsNullOrEmpty(keyPressed))
            WebDataLogger.Instance.LogKeyPressEvent(keyPressed, oldPosition);
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
        if (!rotationStartLogged) //V: prevents from logging at each single frame
        {
            WebDataLogger.Instance.LogRotation("start", _rotationFrom, targetRotation.eulerAngles.y);
            rotationStartLogged = true;
        }

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime 
        );

        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.01f)
        {
            //V; ensure rotation is 90 degree multiple
            float y = Mathf.Round(targetRotation.eulerAngles.y / 90f) * 90f;
            transform.rotation = Quaternion.Euler(0, y, 0);
            isRotating = false;

            WebDataLogger.Instance.LogRotation("complete", _rotationFrom, transform.rotation.eulerAngles.y);
            rotationStartLogged = false;
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
        if (!movementStartLogged)
        {
            _moveFrom = transform.position;
            WebDataLogger.Instance.LogMovementEvent("start", _moveFrom, targetPosition);
            movementStartLogged = true;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime //V: Time.deltaTime = time since last frame; ensures moving time is constant despite ≠ computers may have ≠ updating speed
        );
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f) //V: if the distance between current and target position = 0.01, then snap to target 
        {
            transform.position = targetPosition;
            isMoving = false;

            WebDataLogger.Instance.LogMovementEvent("complete", _moveFrom, transform.position);
            movementStartLogged = false;

            rewardManager.RewardFound(transform.position);
        }
    }
}