using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    public TextMeshProUGUI progressText;

    void Start()
    {
        UpdateProgressText();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackPressed();
        }
    }

    void UpdateProgressText()
    {
        int highest = GameProgress.HighestUnlocked;
        int next = GameProgress.NextLevel;
        bool hasProgress = GameProgress.HasProgress;

        if (progressText)
        {
            progressText.text =
                $"HighestUnlocked: {highest}\n" +
                $"NextLevel: {next}\n" +
                $"HasProgress: {hasProgress}";
        }
    }

    public void OnResetProgressPressed()
    {
        GameProgress.ResetProgress();
        UpdateProgressText();
    }


    public void OnBackPressed()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
