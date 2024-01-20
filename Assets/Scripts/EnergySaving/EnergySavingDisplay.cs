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
    bool result;
    float previousScale = 0;

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
        endText.GetComponent<Text>().text = "Koniec gry!           Twój wynik koñcowy:                   " + previousScore.ToString();

    }


    void Update()
    {
        //Changing cell sizes according to the device resolution GRID LAYOUT: 5x6
        GridLayoutGroup glg = GameObject.Find("GridLayout").GetComponent<GridLayoutGroup>();
        GameObject scrollView = GameObject.Find("Stations");
        GameObject viewport = GameObject.Find("StationsViewport");
        int x = Screen.currentResolution.width - 100;
        int y = Screen.currentResolution.height;
        int spacing = 55;

        //Vector2 newCellSize = new Vector2((x - 4 * spacing - 200) / 5, (y - 5 * spacing - 600) / 6);
        //glg.cellSize = newCellSize;
        float stationsX = Mathf.Abs(scrollView.GetComponent<RectTransform>().sizeDelta.x) ;
        float stationsY = Mathf.Abs(scrollView.GetComponent<RectTransform>().sizeDelta.y);

        float tmp = (float)x / stationsX;
        if ( tmp != previousScale && tmp != 1)
        {
            previousScale = (float)x / stationsX;
            Vector3 scale = new Vector3((float)x / stationsX, (float)x / stationsX, 1);

            scrollView.transform.localScale = scale;
            //Vector2 scale2 = new Vector2(glg.cellSize.x * (float)x / stationsX, glg.cellSize.y * (float)x / stationsX);
            //viewport.GetComponent<RectTransform>().sizeDelta = scale2;
            //glg.cellSize = scale2;
        }
        
    }

    public void ReturnToLobby()
    {
        Debug.Log("MiniGameScoreExit: ");
        MiniGameStatus.Instance.SetStatus("Gra o oszczêdzaniu energii", previousScore, result);
        SceneManager.LoadScene("LobbyScene");
    }

    public void QuitToLobby()
    {
        MiniGameStatus.Instance.SetStatus("Gra o oszczêdzaniu energii", 0, false);
        SceneManager.LoadScene("LobbyScene");
    }

    public void UpdateDisplay(int score, float timeLeft)
    {

        if (score != previousScore)
        {
            UnityEngine.UI.Text textField = scoreDisplay.GetComponent<UnityEngine.UI.Text>();
            textField.text = "Wynik: " + score.ToString();
            previousScore = score;
        }

        UnityEngine.UI.Text timeField = timeDisplay.GetComponent<UnityEngine.UI.Text>();
        int time = (int)timeLeft;
        timeField.text = "Czas: " + time.ToString();

        if (previousScore <= 0) 
        {
            previousScore = 0;
            result = false;
            EndGame();
        }
        if (time <= 0)
        {
            
            result = true;
            EndGame();
        }
        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("MiniGameScoreExit: ");
            MiniGameStatus.Instance.SetStatus("Gra o oszczêdzaniu energii", previousScore);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
