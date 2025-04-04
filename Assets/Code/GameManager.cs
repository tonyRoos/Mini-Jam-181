using UnityEngine;

/* Fiz essa classe para facilitar o acesso a variaveis universais ao jogo. Como fase atual, volume, configurações no geral.. */

public static class GameManager
{
    public static int currentLevel { get => PlayerPrefs.GetInt("currentLevel", 0); set => PlayerPrefs.SetInt("currentLevel", value); }

    
}
