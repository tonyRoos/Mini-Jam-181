using UnityEngine;
using UnityEngine.SceneManagement;

/* Script responsável por reiniciar o jogo quando o jogador clica em "play again" */

public class EndGame : MonoBehaviour
{
    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}
