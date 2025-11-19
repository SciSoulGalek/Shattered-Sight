using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public string mainMenuSceneName = "MainMenu";

    private bool isPaused = false;

    void Start()
    {
        // На всякий случай
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
    
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }
    
    public void OnResumePressed()
    {
        if (isPaused)
            TogglePause();
    }
    
    
    public void OnRestartPressed()
    {
        Time.timeScale = 1f;
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.name);
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDisable()
    {
        // На всякий случай, чтобы игра не зависла если объект/сцена перезагрузились
        Time.timeScale = 1f;
    }
}
