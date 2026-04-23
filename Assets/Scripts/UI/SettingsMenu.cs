using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SettingsMenu : MonoBehaviour
{
    [Header("Scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("UI")]
    public TextMeshProUGUI progressText;

    private InputSystem_Actions controls;

    private void Awake()
    {
        controls = new InputSystem_Actions();

        controls.UI.Back.performed += ctx => OnBackPressed();
    }

    private void OnEnable()
    {
        controls.UI.Enable();
    }

    private void OnDisable()
    {
        controls.UI.Disable();
    }

    private void Start()
    {
        UpdateProgressText();
    }

    void UpdateProgressText()
    {
        int highest = GameProgress.HighestUnlocked;
        int next = GameProgress.NextLevel;
        bool hasProgress = GameProgress.HasProgress;

        if (progressText)
        {
            progressText.text =
                $"HighestUnlocked: {highest}\n" +
                $"NextLevel: {next}\n" +
                $"HasProgress: {hasProgress}";
        }
    }

    public void OnResetProgressPressed()
    {
        GameProgress.ResetProgress();
        UpdateProgressText();
    }

    public void OnBackPressed()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}