using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgress : MonoBehaviour
{
    [Header("Level Info")]
    public int thisLevelIndex = 1;          // 1,2,3...
    public string nextLevelSceneName = "Level2";  // or "" if last level

    public void CompleteLevel()
    {
        int highestUnlocked = PlayerPrefs.GetInt("HighestUnlocked", 1);

        // Unlock next level if there is one
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            int nextIndex = thisLevelIndex + 1;

            if (nextIndex > highestUnlocked)
                PlayerPrefs.SetInt("HighestUnlocked", nextIndex);

            PlayerPrefs.SetInt("NextLevel", nextIndex);
            PlayerPrefs.SetInt("HasProgress", 1);
        }
        else
        {
            // last level: mark finished, but don't go beyond
            PlayerPrefs.SetInt("NextLevel", thisLevelIndex);
            PlayerPrefs.SetInt("HasProgress", 1);
        }

        PlayerPrefs.Save();

        // Load next scene (or maybe show "You win" screen first)
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            SceneManager.LoadScene("MainMenu"); // back to menu after last level
        }
    }
}
