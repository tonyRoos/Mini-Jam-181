using UnityEngine;
using UnityEngine.SceneManagement;

/* Busca a fase atual no GameManager, e recarrega a fase de acordo. */

public class DeathScreen : MonoBehaviour
{
    public void KeepGoing()
    {
        SceneManager.LoadScene(GameManager.currentLevel);
    }
}
