using UnityEngine;
using System.Linq;

public class Puzzle_Button : MonoBehaviour
{
    public enum Tags { RedDoor, GreenDoor, BlueDoor, YellowDoor  }

    [SerializeField] Tags Opens, Closes;
    private Transform[] OpensList;
    private Transform[] ClosesList;

    private void Awake()
    {
        string tagText = Opens.ToString();
        OpensList = GameObject.FindGameObjectsWithTag(tagText).Select(obj => obj.transform).ToArray();
        tagText = Closes.ToString();
        ClosesList = GameObject.FindGameObjectsWithTag(tagText).Select(obj => obj.transform).ToArray();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            foreach(Transform t in OpensList)
            {
                t.localEulerAngles = new Vector3( 0, -80, 0);
            }
            foreach (Transform t in ClosesList)
            {
                t.localEulerAngles = Vector3.zero;
            }
        }
    }

}
