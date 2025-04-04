using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

/* Responsável por mostrar os personagens, virados conforme o lado na tela e escreve o texto da fala enquanto roda um áudio. */

public class Dialog : MonoBehaviour
{
    [System.Serializable]
    public class DialogData
    {
        public DialogAvatar character;
        public string speech;
        public bool leftToRight;
    }

    [SerializeField] private Vector2 speechPitch = new Vector2( 1.2f , 1.8f);
    [SerializeField] private DialogData[] dialogs;
    [SerializeField] private GameObject dialogArea;
    [SerializeField] private RectTransform turnableArea;
    [SerializeField] private Image characterAvatar;
    [SerializeField] private TMP_Text characterNameUI;
    [SerializeField] private TMP_Text speechUi;
    [SerializeField] private AudioClip[] chatSounds;

    private AudioSource audioSrc;
    private PlayableDirector PlayableDirector;


    private void Awake()
    {
        dialogArea.SetActive(false);
        audioSrc = gameObject.GetComponent<AudioSource>();
        PlayableDirector = GameObject.FindFirstObjectByType<PlayableDirector>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && PlayableDirector != null) {
            PlayableDirector.time = PlayableDirector.duration;
            PlayableDirector.Evaluate();
        }
    }

    private Coroutine typingCoroutine = null;
    public void say(int sayIndex)
    {
        characterNameUI.text = dialogs[sayIndex].character.name;
        characterAvatar.sprite = dialogs[sayIndex].character.avatar;
        dialogArea.SetActive(true);
        speechUi.text = dialogs[sayIndex].speech;
        turnableArea.localScale = new Vector3(dialogs[sayIndex].leftToRight ? 1 : -1, 1, 1);
        characterNameUI.rectTransform.localScale = turnableArea.localScale;
        speechUi.rectTransform.localScale = turnableArea.localScale;

        if (typingCoroutine != null) { StopCoroutine(typingCoroutine); Debug.LogWarning("Dialogo " + sayIndex + " muito adiantado"); }

        typingCoroutine = StartCoroutine(TypeText(dialogs[sayIndex].speech, 0.05f));
    }

    public void endDialog()
    {
        dialogArea.SetActive(false);
    }

    private bool playedSoundLastLetter = false;
    public IEnumerator TypeText(string text, float delay)
    {
        speechUi.text = "";
        foreach (char letter in text)
        {
            speechUi.text += letter;
            if (!playedSoundLastLetter)
            {
                audioSrc.Stop();
                audioSrc.pitch = Random.Range(1.2f, 1.8f);
                audioSrc.PlayOneShot(chatSounds[Random.Range(0, chatSounds.Length - 1)]);
            }

            playedSoundLastLetter = !playedSoundLastLetter;
            yield return new WaitForSeconds(delay);
        }
        typingCoroutine = null;
    }

}
