using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Collections;

public class GuiManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuArea, playModeUiArea;

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

    public void fadeOut(int nextScene)
    {
        fade.gameObject.SetActive(true);
        fade.rectTransform.localScale = Vector3.one;
        fadeMask.gameObject.SetActive(true);
        fadeMask.rectTransform.localScale = Vector3.one;

        StartCoroutine(ScaleImage(Vector3.one, Vector3.zero, 2f, nextScene));
    }

    IEnumerator ScaleImage(Vector3 from, Vector3 to, float duration, int nextScene)
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
        if (nextScene == 0) yield break;
        
        SceneManager.LoadScene(nextScene);
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

}
