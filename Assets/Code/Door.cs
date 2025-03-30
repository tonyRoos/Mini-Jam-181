using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField] private int nextScene = -1;
    [SerializeField] private Item key = null;
    [SerializeField] private Animator doorAnim;

    private Player player;

    private void Awake()
    {
        doorAnim = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            player = collision.gameObject.GetComponent<Player>();
            if (key != null)
            {
                Inventory inventory = collision.gameObject.GetComponent<Player>().getInventory();
                if(inventory != null && inventory.items.Contains(key))
                {
                    key = null;
                    navigateToNextScene();
                } else
                {
                    tryOpenDoor(false);
                }
            } else
            {
                navigateToNextScene();
            }
        }
    }

    private void tryOpenDoor(bool succed)
    {
        if(succed)
        {
            doorAnim.SetTrigger("SuccedOpen");
        } else
        {
            doorAnim.SetTrigger("FailOpen");
        }
    }

    private void navigateToNextScene()
    {
        if (nextScene > -1)
        {
            doorAnim.SetTrigger("SuccedOpen");
            player.guiManager.fadeOut(nextScene);
        }
    }
}
