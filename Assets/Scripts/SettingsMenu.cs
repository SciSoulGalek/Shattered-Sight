using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";
    public Button resetProgressButton;

    public void OnResetProgressPressed()
    {
        // Clear only our keys (safer than PlayerPrefs.DeleteAll)
        PlayerPrefs.DeleteKey("NextLevel");
        PlayerPrefs.DeleteKey("HighestUnlocked");
        PlayerPrefs.DeleteKey("HasProgress");
        PlayerPrefs.Save();

        Debug.Log("Progress reset.");

        // Optional: show a simple popup or text "Progress reset"
        // Or just go back to main menu:
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void OnBackPressed()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
