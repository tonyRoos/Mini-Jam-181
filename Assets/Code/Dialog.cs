using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Dialog : MonoBehaviour
{
    [System.Serializable]
    public class DialogData
    {
        public Character character;
        public string speech;
        public bool leftToRight;
    }

    [SerializeField] private DialogData[] dialogs;
    [SerializeField] private GameObject dialogArea;
    [SerializeField] private RectTransform turnableArea;
    [SerializeField] private Image characterAvatar;
    [SerializeField] private TMP_Text characterNameUI;
    [SerializeField] private TMP_Text speechUi;
    [SerializeField] private AudioClip chatSound;

    private AudioSource audioSrc;



    private void Awake()
    {
        dialogArea.SetActive(false);
        audioSrc = gameObject.GetComponent<AudioSource>();
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

    public IEnumerator TypeText(string text, float delay)
    {
        speechUi.text = "";
        foreach (char letter in text)
        {
            speechUi.text += letter;
            audioSrc.Stop();
            audioSrc.pitch = Random.Range(0.6f, 1.4f);
            audioSrc.PlayOneShot(chatSound);
            yield return new WaitForSeconds(delay);
        }
        typingCoroutine = null;
    }

}
