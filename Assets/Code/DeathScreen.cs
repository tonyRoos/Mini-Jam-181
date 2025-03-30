using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    public void KeepGoing()
    {
        SceneManager.LoadScene(GameManager.currentLevel);
    }
}
