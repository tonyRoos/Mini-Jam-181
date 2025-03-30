using UnityEngine;

[CreateAssetMenu(fileName = "new character", menuName = "Characters/New")]
public class Character : ScriptableObject
{
    public string charName;
    public Sprite avatar;
}