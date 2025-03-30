using UnityEngine;

[CreateAssetMenu(fileName = "new item", menuName = "Items/New")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string itemName;
}
