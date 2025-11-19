using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    public int levelIndex = 1;        // 1,2,3...
    public string sceneName;          // "Level1", "Level2", etc.
    public Button button;
    public Text label;
    public GameObject lockIcon;       // optional

    void Awake()
    {
        if (!button) button = GetComponent<Button>();
        if (label) label.text = levelIndex.ToString();

        int highestUnlocked = PlayerPrefs.GetInt("HighestUnlocked", 1);
        bool unlocked = levelIndex <= highestUnlocked;

        button.interactable = unlocked;

        if (lockIcon)
            lockIcon.SetActive(!unlocked);
    }

    public void OnPressed()
    {
        int nextIndex = levelIndex;
        PlayerPrefs.SetInt("NextLevel", nextIndex);
        PlayerPrefs.SetInt("HasProgress", nextIndex > 1 ? 1 : 0);
        PlayerPrefs.Save();

        SceneManager.LoadScene(sceneName);
    }
}
