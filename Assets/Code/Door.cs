using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/* Esse C�digo de porta se refere �nica e exclusivamente as portas de passagem de fase */

public class Door : MonoBehaviour
{
    [SerializeField] private bool isFinalScene = false;
    [SerializeField] private int nextScene = -1;
    [SerializeField] private Item key = null;
    [SerializeField] private Animator doorAnim;
    [SerializeField] private AudioClip lockedSound, enterSound;

    private AudioSource audioSrc;
    private Player player;

    private void Awake()
    {
        doorAnim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>(); 
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
                    tryOpenDoor(true);
                    if (isFinalScene) { player.guiManager.fadeOut(0, GuiManager.FadeType.END_JAM_EDITION); return; }
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
            audioSrc.PlayOneShot(enterSound);
            doorAnim.SetTrigger("SuccedOpen");
        } else
        {
            audioSrc.pitch = Random.Range(0.75f, 1.25f);
            audioSrc.PlayOneShot(lockedSound);
            doorAnim.SetTrigger("FailOpen");
        }
    }

    private void navigateToNextScene()
    {
        if (nextScene > -1)
        {
            player.guiManager.fadeOut(nextScene, GuiManager.FadeType.KEY);
            player.gameMode = GameMode.CUT_SCENE;
        }
    }
}
