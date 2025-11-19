using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // change to TMPro if using TextMeshProUGUI

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Names")]
    public string firstLevelSceneName = "Level1";
    public string levelsSceneName = "Levels";
    public string settingsSceneName = "Settings";

    [Header("UI")]
    public Button playButton;
    public Text playButtonLabel; // or TextMeshProUGUI

    bool hasProgress;
    int nextLevelIndex;

    void Start()
    {
        // Load saved progress
        nextLevelIndex = PlayerPrefs.GetInt("NextLevel", 1);
        hasProgress = PlayerPrefs.GetInt("HasProgress", 0) == 1 && nextLevelIndex > 1;

        // Set label
        if (hasProgress)
            playButtonLabel.text = "Continue";
        else
            playButtonLabel.text = "New Game";
    }

    public void OnPlayPressed()
    {
        if (hasProgress)
        {
            // Continue from next level
            string sceneToLoad = "Level" + nextLevelIndex;
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            // Start fresh
            PlayerPrefs.SetInt("NextLevel", 1);
            PlayerPrefs.SetInt("HighestUnlocked", 1);
            PlayerPrefs.SetInt("HasProgress", 0);
            PlayerPrefs.Save();

            SceneManager.LoadScene(firstLevelSceneName);
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
