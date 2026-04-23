using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LevelSelectButton : MonoBehaviour
{
    public int levelIndex = 1;           
    public string sceneName = "Level1";   

    public Button button;
    public TextMeshProUGUI label; 
    public GameObject lockIcon;            

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
