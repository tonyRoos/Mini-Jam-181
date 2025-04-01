using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;
using static GuiManager;

public class GuiManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuArea, playModeUiArea, deathScreen, endGameScreen;

    [SerializeField] private Gradient staminaGradient;
    [SerializeField] private Image staminaBar;

    [SerializeField] private GameObject HorizontalList, ItemUiPrefab;
    [SerializeField] private List<Image> items;

    [SerializeField] private Image fade, fadeMask;
    [SerializeField] private GameObject cutsceneMode;

    private void Awake()
    {
        pauseMenuArea.SetActive(false);
        playModeUiArea.SetActive(true);
        cutsceneMode.SetActive(false);
        deathScreen.SetActive(false);
        endGameScreen.SetActive(false);
    }

    public void updateStaminaBar(float status)
    {
        staminaBar.fillAmount = status;
        staminaBar.color = staminaGradient.Evaluate(status);
    }

    public void addItem(Item newItem)
    {
        if (!items.Any(img => img.sprite == newItem.icon))
        {
            GameObject newItemUiObj = Instantiate(ItemUiPrefab, HorizontalList.transform);
            newItemUiObj.name = newItem.itemName;
            newItemUiObj.GetComponent<Image>().sprite = newItem.icon;
        }
    }

    public enum FadeType
    {
        KEY, DEATH, END_JAM_EDITION 
    }

    [SerializeField] Sprite keyMask, deathMask, endMask;
    public void fadeOut(int nextScene, FadeType fadeType)
    {
        switch(fadeType)
        {
            case FadeType.KEY:
                fadeMask.sprite = keyMask;
                break;
            case FadeType.DEATH:
                fadeMask.sprite = deathMask;
                break;
            case FadeType.END_JAM_EDITION:
                fadeMask.sprite = endMask;
                break;
        }

        fade.gameObject.SetActive(true);
        fade.rectTransform.localScale = Vector3.one;
        fadeMask.gameObject.SetActive(true);
        fadeMask.rectTransform.localScale = Vector3.one;

        StartCoroutine(ScaleImage(Vector3.one, Vector3.zero, 2f, nextScene, fadeType));
    }

    IEnumerator ScaleImage(Vector3 from, Vector3 to, float duration, int nextScene, FadeType fadeType)
    {
        float elapsedTime = 0f;
        RectTransform rectTransform = fadeMask.rectTransform;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            rectTransform.localScale = Vector3.Lerp(from, to, t);
            yield return null;
        }

        rectTransform.localScale = to;

        switch (fadeType)
        {
            case FadeType.KEY:
                if (nextScene == 0) yield break;

                SceneManager.LoadScene(nextScene);
                GameManager.currentLevel = nextScene;
                break;
            case FadeType.DEATH:
                setDeathScreen(true);
                break;
            case FadeType.END_JAM_EDITION:
                setEndGameScreen(true);
                break;
        }


        
    }

    public void setPauseMenu(bool active)
    {
        pauseMenuArea.SetActive(active);
        playModeUiArea.SetActive(!active);
    }

    public void setCutsceneMode(bool active)
    {
        cutsceneMode.SetActive(active);
    }

    [SerializeField] AudioClip deathSceneMusic;
    public void setDeathScreen(bool active)
    {
        deathScreen.SetActive(active);
        Camera.main.GetComponent<AudioSource>().clip = deathSceneMusic;
        Camera.main.GetComponent<AudioSource>().Play();
    }

    [SerializeField] AudioClip endSceneMusic;
    public void setEndGameScreen(bool active)
    {
        endGameScreen.SetActive(active);
        Camera.main.GetComponent<AudioSource>().clip = endSceneMusic;
        Camera.main.GetComponent<AudioSource>().Play();
    }

}
