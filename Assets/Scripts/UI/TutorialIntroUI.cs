using UnityEngine;
using System.Collections;

public class TutorialIntroUI : MonoBehaviour
{
    [SerializeField] private GameObject tutorialPanel;
    [SerializeField] private PlayerController2D player;
    [SerializeField] private PauseMenu pauseMenu;

    private void Start()
    {
        if (GameProgress.TutorialSeen)
        {
            tutorialPanel.SetActive(false);
            return;
        }

        ShowTutorial();
    }

    private void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        pauseMenu.PauseLocked = true;

        StartCoroutine(PauseNextFrame());
    }

    private IEnumerator PauseNextFrame()
    {
        yield return null;
        Time.timeScale = 0f;
    }

    public void SkipTutorial()
    {
        GameProgress.TutorialSeen = true;
        SaveSystem.SaveGame();

        tutorialPanel.SetActive(false);
        pauseMenu.PauseLocked = false;
        Time.timeScale = 1f;

        if (player != null)
            player.SetInputEnabled(true);
    }
}