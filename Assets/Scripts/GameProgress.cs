using UnityEngine;

public static class GameProgress
{
    private const string HasProgressKey = "HasProgress";
    private const string NextLevelKey = "NextLevel";
    private const string HighestUnlockedKey = "HighestUnlocked";

    public static int TotalLevels = 10;

    public static bool HasProgress => PlayerPrefs.GetInt(HasProgressKey, 0) == 1;
    public static int NextLevel => PlayerPrefs.GetInt(NextLevelKey, 1);
    public static int HighestUnlocked => PlayerPrefs.GetInt(HighestUnlockedKey, 1);

    public static void ResetProgress()
    {
        PlayerPrefs.SetInt(HasProgressKey, 0);
        PlayerPrefs.SetInt(NextLevelKey, 1);
        PlayerPrefs.SetInt(HighestUnlockedKey, 1);
        PlayerPrefs.Save();
    }

    public static void CompleteLevel(int currentLevel)
    {
        int next = Mathf.Clamp(currentLevel + 1, 1, TotalLevels);

        if (next > HighestUnlocked)
            PlayerPrefs.SetInt(HighestUnlockedKey, next);

        PlayerPrefs.SetInt(NextLevelKey, next);
        PlayerPrefs.SetInt(HasProgressKey, next > 1 ? 1 : 0);
        PlayerPrefs.Save();
    }
}
