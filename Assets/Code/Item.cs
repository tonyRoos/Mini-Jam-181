using UnityEngine;

// � o conteudo, as informa��es dos itens (cria o asset com os dados do item)

[CreateAssetMenu(fileName = "new item", menuName = "Items/New")]
public class Item : ScriptableObject
{
    public Sprite icon;
    public string itemName;
}
