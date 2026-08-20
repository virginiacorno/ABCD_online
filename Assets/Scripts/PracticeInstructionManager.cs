using UnityEngine;

public class PracticeInstructionManager : MonoBehaviour
{
    public GameObject instructionPanel;
    public GameObject practicePanel;
    public moveplayer player;
    public PracticePhase practicePhase;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instructionPanel.SetActive(true);
        practicePanel.SetActive(false);
        WebDataLogger.Instance.LogScreenEvent("practice_phase_start", "onset");
        Time.timeScale = 0f;
        player.inputEnabled = false;
    }

    //V: advanced by UI button clicks only, not key presses
    public void OnInstructionButton()
    {
        instructionPanel.SetActive(false);
        practicePanel.SetActive(false);
        Time.timeScale = 1f; //V: free exploration is real movement, not a static panel
        practicePhase.StartPractice();
    }

    //V: called by PracticePhase once free exploration ends (timer or space bar)
    public void ShowPracticePanel()
    {
        practicePanel.SetActive(true);
        instructionPanel.SetActive(false);
        Time.timeScale = 0f;
    }

    public void OnPracticeButton()
    {
        instructionPanel.SetActive(false);
        practicePanel.SetActive(false);
        Time.timeScale = 1f;

        practicePhase.StartCoroutine(practicePhase.RunPracticeLoop());
    }

}
