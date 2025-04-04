using UnityEngine;

/* Permite o jogador obter o item criado */

public class GetItem : MonoBehaviour
{
    public Item item;
    [SerializeField] Transform itemSprite;
    public AudioClip getSound;

    private void Update()
    {
        if (!itemSprite) return;

        itemSprite.Rotate( 0, Time.deltaTime * 180, 0);
    }
}
