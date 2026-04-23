using UnityEngine;

public static class GameProgress
{
    public static int TotalLevels = 10;

    public static bool HasProgress { get; set; }
    public static int NextLevel { get; set; } = 1;
    public static int HighestUnlocked { get; set; } = 1;

    public static void ResetProgress()
    {
        HasProgress = false;
        NextLevel = 1;
        HighestUnlocked = 1;

        SaveSystem.SaveGame();
    }

    public static void CompleteLevel(int currentLevel)
    {
        int next = Mathf.Clamp(currentLevel + 1, 1, TotalLevels);

        if (next > HighestUnlocked)
            HighestUnlocked = next;

        NextLevel = next;
        HasProgress = next > 1;

        SaveSystem.SaveGame();
    }
}