using UnityEngine;

public static class GameManager
{
    //public bool cutscenePlaying;
    public static int currentLevel { get => PlayerPrefs.GetInt("currentLevel", 0); set => PlayerPrefs.SetInt("currentLevel", value); }

    
}
