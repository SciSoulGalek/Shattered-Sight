using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgress : MonoBehaviour
{
    [Header("Level Info")]
    public int thisLevelIndex = 1;      
    public string nextLevelSceneName = "Level2";

    public void CompleteLevel()
    {
        GameProgress.CompleteLevel(thisLevelIndex);
        if (!string.IsNullOrEmpty(nextLevelSceneName))
            SceneManager.LoadScene(nextLevelSceneName);
        else
            SceneManager.LoadScene("MainMenu");
    }

}
