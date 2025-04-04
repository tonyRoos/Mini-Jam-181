using UnityEngine;
using System.Linq;

/* Permite abrir e fechar portas através das tags dos objetos, assim, as portas não precisam ser adicionadas a uma lista, reduzindo o trabalho de level design. */

public class Puzzle_Button : MonoBehaviour
{
    public enum Tags { NONE, RedDoor, GreenDoor, BlueDoor, YellowDoor  }

    [SerializeField] Tags Opens, Closes;
    [SerializeField] AudioClip buttonPressSound;
    private Transform[] OpensList;
    private Transform[] ClosesList;
    private Animator anim;
    private AudioSource audioSrc;

    private void Awake()
    {
        string tagText = Opens.ToString();
        OpensList = Opens != Tags.NONE ? GameObject.FindGameObjectsWithTag(tagText).Select(obj => obj.transform).ToArray() : null;
        tagText = Closes.ToString();
        ClosesList = Closes != Tags.NONE ? GameObject.FindGameObjectsWithTag(tagText).Select(obj => obj.transform).ToArray() : null;
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            audioSrc.PlayOneShot(buttonPressSound);
            anim.SetBool("isPressed", true);
            if (OpensList != null) {
                foreach (Transform t in OpensList)
                {
                    t.localEulerAngles = new Vector3(0, -80, 0);
                    t.GetComponentInChildren<BoxCollider>().enabled = false;
                }
            }
            if (ClosesList != null)
            {
                foreach (Transform t in ClosesList)
                {
                    t.localEulerAngles = Vector3.zero;
                    t.GetComponentInChildren<BoxCollider>().enabled = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            anim.SetBool("isPressed", false);
        }
    }

}
