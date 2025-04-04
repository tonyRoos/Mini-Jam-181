using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Essa classe alterna sprites em uma image na UI, pra poder mostrar um video na tela, depois carrega a próxima cena, se tiver. */
public class FrameByFrameVideo : MonoBehaviour
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
            /* garante que a próxima cena seja carregada caso não existam frames, destino para os frames, audio ou o componente de rodar o audio. */
            Debug.LogError("Missing components or frames in SpriteVideoPlayer.");
            loadNextScene();
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

        loadNextScene();
    }

    private void loadNextScene()
    {
        if (SceneManager.sceneCount > SceneManager.GetActiveScene().buildIndex + 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
