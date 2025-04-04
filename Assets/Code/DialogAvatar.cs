using UnityEngine;

/* Essa Classe permite criar assets contendo o sprite e nome do personagem para ser reutilizado e facilitar o preenchimento dos dialogos no inspetor */

[CreateAssetMenu(fileName = "new dialog avatar", menuName = "Game Assets/Dialog/New Dialog Avatar")]
public class DialogAvatar : ScriptableObject
{
    public string charName;
    public Sprite avatar;
}