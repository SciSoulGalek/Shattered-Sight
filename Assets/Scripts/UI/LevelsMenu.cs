using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelsMenu : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";

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

    public void OnBackPressed()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}