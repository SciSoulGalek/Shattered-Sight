using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";

    public void OnBackPressed()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
