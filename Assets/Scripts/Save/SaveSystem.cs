using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string path = Application.persistentDataPath + "/save.json";

    public static void SaveGame()
    {
        SaveData data = new SaveData
        {
            highestUnlocked = GameProgress.HighestUnlocked,
            nextLevel = GameProgress.NextLevel,
            hasProgress = GameProgress.HasProgress,
            tutorialSeen = GameProgress.TutorialSeen
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static void LoadGame()
    {
        if (!File.Exists(path))
        {
            GameProgress.HasProgress = false;
            GameProgress.NextLevel = 1;
            GameProgress.HighestUnlocked = 1;
            GameProgress.TutorialSeen = false;
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        GameProgress.HighestUnlocked = data.highestUnlocked;
        GameProgress.NextLevel = data.nextLevel;
        GameProgress.HasProgress = data.hasProgress;
    }

    public static void DeleteSave()
    {
        if (File.Exists(path))
            File.Delete(path);
    }
}