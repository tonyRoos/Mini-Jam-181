using UnityEngine;

public class GetItem : MonoBehaviour
{
    public Item item;
    [SerializeField] Transform itemSprite;

    private void Update()
    {
        if (!itemSprite) return;

        itemSprite.Rotate( 0, Time.deltaTime * 180, 0);
    }
}
