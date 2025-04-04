using UnityEngine;

/* Eu criei essa classe somente pra poder ativar um trigger na timeline e desabilitar o fantasma, coloquei o nome de TempBehaviour (Temporary behaviour), por falta de ideias mesmo. */

public class TempBehaviour : MonoBehaviour
{
    public void disableThisObject()
    {
        gameObject.SetActive(false);
    }
}
