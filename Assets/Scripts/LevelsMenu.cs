using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";

    void Update()
    {
        // ESC key → same as Back button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnBackPressed();
        }
    }

    public void OnBackPressed()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
