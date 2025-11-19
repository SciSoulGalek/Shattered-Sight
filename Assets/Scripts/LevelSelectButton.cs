using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    public int levelIndex = 1;              // 1..10
    public string sceneName = "Level1";     // "Level1".."Level10"

    public Button button;
    public TextMeshProUGUI label;           // optional, for showing the number
    public GameObject lockIcon;             // optional

    void Awake()
    {
        if (!button)
            button = GetComponent<Button>();

        if (label)
            label.text = levelIndex.ToString();

        int highestUnlocked = GameProgress.HighestUnlocked;
        bool unlocked = levelIndex <= highestUnlocked;

        if (button)
            button.interactable = unlocked;

        if (lockIcon)
            lockIcon.SetActive(!unlocked);

        if (label)
            label.gameObject.SetActive(unlocked);
    }

    public void OnPressed()
    {
        Debug.Log($"[LevelSelectButton] Click on level {levelIndex}, sceneName={sceneName}");
        
        if (button && !button.interactable)
            return;

        SceneManager.LoadScene(sceneName);
    }
}
