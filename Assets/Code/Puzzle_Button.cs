using UnityEngine;
using System.Linq;

public class Puzzle_Button : MonoBehaviour
{
    public enum Tags { NONE, RedDoor, GreenDoor, BlueDoor, YellowDoor  }

    [SerializeField] Tags Opens, Closes;
    private Transform[] OpensList;
    private Transform[] ClosesList;
    private Animator anim;

    private void Awake()
    {
        string tagText = Opens.ToString();
        OpensList = Opens != Tags.NONE ? GameObject.FindGameObjectsWithTag(tagText).Select(obj => obj.transform).ToArray() : null;
        tagText = Closes.ToString();
        ClosesList = Closes != Tags.NONE ? GameObject.FindGameObjectsWithTag(tagText).Select(obj => obj.transform).ToArray() : null;
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            anim.SetBool("isPressed", true);
            if (OpensList != null) {
                foreach (Transform t in OpensList)
                {
                    t.localEulerAngles = new Vector3(0, -80, 0);
                }
            }
            if (ClosesList != null)
            {
                foreach (Transform t in ClosesList)
                {
                    t.localEulerAngles = Vector3.zero;
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
