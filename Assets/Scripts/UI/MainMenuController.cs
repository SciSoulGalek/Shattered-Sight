using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string firstLevelSceneName = "Level1";
    public string levelsSceneName = "Levels";
    public string settingsSceneName = "Settings";

    [Header("UI")]
    public Button playButton;
    public TextMeshProUGUI playButtonLabel;

    bool hasProgress;
    int nextLevelIndex;

    void Start()
    {
        playButtonLabel.text = GameProgress.HasProgress && GameProgress.NextLevel > 1
            ? "Continue"
            : "New Game";
    }
    
    public void OnPlayPressed()
    {
        if (GameProgress.HasProgress && GameProgress.NextLevel > 1)
        {
            SceneManager.LoadScene("Level" + GameProgress.NextLevel);
        }
        else
        {
            GameProgress.ResetProgress();
            SceneManager.LoadScene("Level1");
        }
    }


    public void OnLevelsPressed()
    {
        SceneManager.LoadScene(levelsSceneName);
    }

    public void OnSettingsPressed()
    {
        SceneManager.LoadScene(settingsSceneName);
    }

    public void OnExitPressed()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
