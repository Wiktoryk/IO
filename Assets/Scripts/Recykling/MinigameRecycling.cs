using UnityEngine;
using UnityEngine.SceneManagement;

public class MinigameRecycling : MonoBehaviour, IMinigame
{
    public (bool, int) GetResult()
    {
        return RecyklingGameScript.result;
    }

    public void PlayMinigame()
    {
        SceneManager.LoadScene("Recykling");
    }
}
