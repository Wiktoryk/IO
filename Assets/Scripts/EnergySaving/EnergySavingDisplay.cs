using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnergySavingDisplay : MonoBehaviour
{
    GameObject startGameDisplay;
    [SerializeField]
    GameObject gameDisplay;
    [SerializeField]
    GameObject endGameDisplay;
    [SerializeField]
    GameObject scoreDisplay;
    [SerializeField]
    GameObject timeDisplay;
    int previousScore;

    // Start is called before the first frame update
    void Start()
    {
        //scoreDisplay = GameObject.Find("ScoreDisplay");
        startGameDisplay = GameObject.Find("StartGameDisplay");
        //gameDisplay = GameObject.Find("GameDisplay");
        //endGameDisplay = GameObject.Find("EndGameDisplay");
        //timeDisplay = GameObject.Find("TimeDisplay");
        previousScore = 0;
    }

    public void StartGame()
    {
        gameObject.GetComponent<Office>().enabled = true;
        startGameDisplay.SetActive(false);
        gameDisplay.SetActive(true);
    }

    private void EndGame()
    {

        gameObject.GetComponent<Office>().EndGame();
        gameObject.GetComponent<Office>().enabled = false;
        gameDisplay.SetActive(false);
        endGameDisplay.SetActive(true);
        Debug.Log("d");
        GameObject endText = GameObject.Find("FinishText");
        endText.GetComponent<Text>().text = "Congratulations!           Your final score:                   " + previousScore.ToString();

    }


    void Update()
    {
    }

    public void ReturnToLobby()
    {
        Debug.Log("MiniGameScoreExit: ");
        MiniGameStatus.Instance.SetStatus("MINI", previousScore);
        SceneManager.LoadScene("LobbyScene");
    }


    public void UpdateDisplay(int score, float timeLeft)
    {

        if (score != previousScore)
        {
            UnityEngine.UI.Text textField = scoreDisplay.GetComponent<UnityEngine.UI.Text>();
            textField.text = "Score: " + score.ToString();
            previousScore = score;
        }

        UnityEngine.UI.Text timeField = timeDisplay.GetComponent<UnityEngine.UI.Text>();
        int time = (int)timeLeft;
        timeField.text = "Time left: " + time.ToString();

        if (time <= 0)
        {
            EndGame();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("MiniGameScoreExit: ");
            MiniGameStatus.Instance.SetStatus("MINI", previousScore);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
