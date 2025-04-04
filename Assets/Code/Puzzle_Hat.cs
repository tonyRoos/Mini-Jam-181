using UnityEngine;

/* Script que faz a transi��o do personagem entre dois chap�us */

public class Puzzle_Hat : MonoBehaviour
{
    [SerializeField] private Puzzle_Hat connection;

    private AudioSource audioSrc;
    [SerializeField] private AudioClip transportSound;

    private void Awake()
    {
        audioSrc = GetComponent<AudioSource>(); 
    }

    public void playTransportSound()
    {
        audioSrc.PlayOneShot(transportSound);
    }

    public void transport(Transform obj)
    {
        obj.transform.position = connection.transform.position;
        connection.playTransportSound();
    }
}
