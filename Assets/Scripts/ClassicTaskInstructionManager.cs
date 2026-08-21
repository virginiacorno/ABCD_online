using UnityEngine;

public class ClassicTaskInstructionManager : TaskInstructionManagerBase
{
    public CameraManager cameraManager;
    public GameObject endPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endPanel.SetActive(false);
        ShowInstruction(); //V: call this as it is bc working mechanism already accurate in other script
    }

    //V: update the behaviour of buttons
    public override void OnInstructionButton()
    {
        WebDataLogger.Instance.LogScreenEvent("instruction", "button_press");
        WebDataLogger.Instance.LogScreenEvent("instruction", "offset");
        movementPanel.SetActive(false);
        instructionPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        Time.timeScale = 1f;

        ShowMovementInstruction();
    }

    public override void OnMovementButton()
    {
        WebDataLogger.Instance.LogScreenEvent("movement", "button_press");
        WebDataLogger.Instance.LogScreenEvent("movement", "offset");
        movementPanel.SetActive(false);
        instructionPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        Time.timeScale = 1f;
        cameraManager.StartNewConfiguration(0);
    }

    public override void OnFeedbackButton()
    {
        WebDataLogger.Instance.LogScreenEvent("feedback", "button_press");
        WebDataLogger.Instance.LogScreenEvent("feedback", "offset");
        movementPanel.SetActive(false);
        instructionPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        Time.timeScale = 1f;
        NewSequenceInstructions();
    }

    public override void OnContinueButton()
    {
        WebDataLogger.Instance.LogScreenEvent("new_sequence", "button_press");
        WebDataLogger.Instance.LogScreenEvent("new_sequence", "offset");
        instructionPanel.SetActive(false);
        movementPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        Time.timeScale = 1f;
        rewardManager.StartNextConfiguration();
    }

    public override void EndScreen()
    {
        movementPanel.SetActive(false);
        instructionPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        endPanel.SetActive(true);
        Time.timeScale = 0f;
        WebDataLogger.Instance.LogScreenEvent("end", "onset");
    }

    //V: call loading of the next scene when people press the button
    public void OnEndButton()
    {
        WebDataLogger.Instance.LogScreenEvent("end", "button_press");
        WebDataLogger.Instance.LogScreenEvent("end", "offset");
        Time.timeScale = 1f;
        SceneSequenceManager.Instance.GoToCueTask();
    }

}
