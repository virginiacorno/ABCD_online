using UnityEngine;

public class PracticeInstructionManager : MonoBehaviour
{
    public GameObject instructionPanel;
    public GameObject movementPanel;
    public GameObject practicePanel;
    public moveplayer player;
    public PracticePhase practicePhase;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instructionPanel.SetActive(true);
        movementPanel.SetActive(false);
        practicePanel.SetActive(false);
        Time.timeScale = 0f;
        WebDataLogger.Instance.LogScreenEvent("instruction", "onset");
    }

    public void OnInstructionButton()
    {
        WebDataLogger.Instance.LogScreenEvent("instruction", "button_press");
        WebDataLogger.Instance.LogScreenEvent("instruction", "offset");
        movementPanel.SetActive(true);
        instructionPanel.SetActive(false);
        practicePanel.SetActive(false);
        Time.timeScale = 0f;
        WebDataLogger.Instance.LogScreenEvent("movement", "onset");
    }

    public void OnMovementButton()
    {
        WebDataLogger.Instance.LogScreenEvent("movement", "button_press");
        WebDataLogger.Instance.LogScreenEvent("movement", "offset");
        movementPanel.SetActive(false);
        instructionPanel.SetActive(false);
        practicePanel.SetActive(true);
        Time.timeScale = 0f;
        WebDataLogger.Instance.LogScreenEvent("practice", "onset");
    }

    public void OnPracticeButton()
    {
        WebDataLogger.Instance.LogScreenEvent("practice", "button_press");
        WebDataLogger.Instance.LogScreenEvent("practice", "offset");
        instructionPanel.SetActive(false);
        movementPanel.SetActive(false);
        practicePanel.SetActive(false);
        Time.timeScale = 1f;

        practicePhase.StartPractice();
    }

}
