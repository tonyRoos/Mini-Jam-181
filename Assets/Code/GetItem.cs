using UnityEngine;

public class GetItem : MonoBehaviour
{
    public Item item;

    private void Update()
    {
        transform.Rotate( 0, Time.deltaTime * 180, 0);
    }
}
