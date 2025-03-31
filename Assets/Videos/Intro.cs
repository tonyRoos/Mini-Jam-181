using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
    [SerializeField] private float frameRate;
    [SerializeField] private Image targetImage;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private Sprite[] frames;


    private void Start()
    {
        if (frames.Length > 0 && targetImage != null && audioSource != null && audioClip != null)
        {
            StartCoroutine(PlayVideo());
        }
        else
        {
            Debug.LogError("Missing components or frames in SpriteVideoPlayer.");
            SceneManager.LoadScene(2);
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) { SceneManager.LoadScene(2); }
    }

    public void populateFrames(Sprite[] frames)
    {
        this.frames = frames;
    }

    private IEnumerator PlayVideo()
    {
        float frameTime = 1f / frameRate;
        audioSource.clip = audioClip;
        audioSource.Play();

        for (int i = 0; i < frames.Length; i++)
        {
            targetImage.sprite = frames[i];
            yield return new WaitForSeconds(frameTime);
        }
        SceneManager.LoadScene(2);
    }
}
