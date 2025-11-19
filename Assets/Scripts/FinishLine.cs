using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{
    [Header("Level Info")]
    public int thisLevelIndex = 1;            // 1..10
    public string nextLevelSceneName = "";    // e.g. "Level2", or "" if last level

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // Save progress
        GameProgress.CompleteLevel(thisLevelIndex);

        // Load next scene or main menu
        if (!string.IsNullOrEmpty(nextLevelSceneName))
        {
            SceneManager.LoadScene(nextLevelSceneName);
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    // If 3D:
    // private void OnTriggerEnter(Collider other) { ... same logic ... }
}
