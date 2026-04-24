using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;
    public string mainMenuSceneName = "MainMenu";

    private InputSystem_Actions controls;
    private bool isPaused = false;

    void Start()
    {
        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void Awake()
    {
        controls = new InputSystem_Actions();

        controls.Player.Pause.performed += ctx => TogglePause();
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
        pausePanel.SetActive(false);
        Time.timeScale = 1f;
        FindFirstObjectByType<PlayerRespawn>().Die();
    }

    public void OnMainMenuPressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    void OnDisable()
    {
        controls.Disable();
        Time.timeScale = 1f;
    }
}
