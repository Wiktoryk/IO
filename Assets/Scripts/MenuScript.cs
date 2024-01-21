using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class MenuScript : MonoBehaviour
{
    public GameObject menu;
    public GameObject menuButton;
    [FormerlySerializedAs("timer")] public Timer customTimer;
    public Statistics statistics;
    // Start is called before the first frame update
    void Start()
    {
        menu.SetActive(false);
        menuButton.SetActive(true);
    }

    public void Resume()
    {
        menu.SetActive(false);
        menuButton.SetActive(true);
        customTimer.timerOn = true;
    }
    public void MenuOn()
    {
        menu.SetActive(true);
        menuButton.SetActive(false);
       customTimer.timerOn = false;
    }

    public void Repeat() {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
    }

    public void End() {
        //MiniGameStatus.Instance.SetStatus("Ponowne wykorzystanie", 0, false);
        SceneManager.LoadScene("LobbyScene");

    }


}
