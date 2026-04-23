using UnityEngine;

public class GameLoader : MonoBehaviour
{
    void Awake()
    {
        SaveSystem.LoadGame();
    }
}