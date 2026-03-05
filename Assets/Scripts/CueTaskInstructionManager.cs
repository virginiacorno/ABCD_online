using UnityEngine;

public class CueTaskInstructionManager : TaskInstructionManagerBase
{
    public FreeNavigationCamera freeNavCamera;
    public GameObject cuePanel;
    public GameObject endPanel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ShowCuePanel();
    }

    public void ShowCuePanel()
    {
        player.inputEnabled = false;
        cuePanel.SetActive(true);
        instructionPanel.SetActive(false);
        movementPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        endPanel.SetActive(false);
        WebDataLogger.Instance.LogScreenEvent("cue_panel", "onset");

        //V: disable the minimap so we can see the cue
        freeNavCamera.DisableMiniMap();
        //V: make cue in the scene visible
        rewardManager.cueObject.SetActive(true);
    }

    public void OnCueButton()
    {
        WebDataLogger.Instance.LogScreenEvent("cue_panel", "button_press");
        WebDataLogger.Instance.LogScreenEvent("cue_panel", "offset");
        rewardManager.HideCue();
        cuePanel.SetActive(false);
        instructionPanel.SetActive(false);
        movementPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        endPanel.SetActive(false);

        ShowInstruction();
    }

    public override void OnInstructionButton()
    {
        WebDataLogger.Instance.LogScreenEvent("instruction", "button_press");
        WebDataLogger.Instance.LogScreenEvent("instruction", "offset");
        instructionPanel.SetActive(false);
        movementPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        cuePanel.SetActive(false);
        endPanel.SetActive(false);
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
        cuePanel.SetActive(false);
        endPanel.SetActive(false);
        Time.timeScale = 1f;

        freeNavCamera.StartNewConfiguration(0);
    }

    public override void OnFeedbackButton()
    {
        WebDataLogger.Instance.LogScreenEvent("feedback", "button_press");
        WebDataLogger.Instance.LogScreenEvent("feedback", "offset");
        movementPanel.SetActive(false);
        instructionPanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        cuePanel.SetActive(false);
        endPanel.SetActive(false);
        Time.timeScale = 1f;

        NewSequenceInstructions();
    }

    public override void OnContinueButton()
    {
        WebDataLogger.Instance.LogScreenEvent("new_sequence", "button_press");
        WebDataLogger.Instance.LogScreenEvent("new_sequence", "offset");
        instructionPanel.SetActive(false);
        movementPanel.SetActive(false);
        cuePanel.SetActive(false);
        feedbackPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        endPanel.SetActive(false);
        Time.timeScale = 1f;

        freeNavCamera._startGameAfterTransition = true;
        freeNavCamera.StartCameraTransition();
    }

    public override void EndScreen()
    {
        Debug.Log("ABCD_DONE");
        instructionPanel.SetActive(false);
        movementPanel.SetActive(false);
        newSeqPanel.SetActive(false);
        cuePanel.SetActive(false);
        endPanel.SetActive(true);
    }
}
