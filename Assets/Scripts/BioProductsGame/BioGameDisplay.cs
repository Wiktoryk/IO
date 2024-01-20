using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class BioGameDisplay : MonoBehaviour
{
    GameObject startGameDisplay;
    [SerializeField]
    GameObject gameDisplay;
    [SerializeField]
    GameObject endGameDisplay;
    GameObject currentProductDisplay;
    [SerializeField]
    GameObject scoreDisplay;
    [SerializeField]
    GameObject timeDisplay;
    int previousScore;
    bool result;

    // Start is called before the first frame update
    void Start()
    {
        currentProductDisplay = GameObject.Find("CurrentProductDisplay");
        if(currentProductDisplay == null)
        {
            Debug.LogWarning("Unsuccessful assignment of CurrentProductDisplay");
        }
        //scoreDisplay = GameObject.Find("ScoreDisplay");
        startGameDisplay = GameObject.Find("StartGameDisplay");
        //gameDisplay = GameObject.Find("GameDisplay");
        //endGameDisplay = GameObject.Find("EndGameDisplay");
        //timeDisplay = GameObject.Find("TimeDisplay");
        previousScore = 0;
    }

    public void StartGame()
    {
        gameObject.GetComponent<BioProductsManager>().enabled = true;
        startGameDisplay.SetActive(false);
        gameDisplay.SetActive(true);
    }

    private void EndGame()
    {
        gameObject.GetComponent<BioProductsManager>().enabled = false;
        currentProductDisplay.GetComponent<UnityEngine.UI.Image>().sprite = null;
        gameDisplay.SetActive(false);
        endGameDisplay.SetActive(true);
Debug.Log("d");
        GameObject endText = GameObject.Find("FinishText");
        endText.GetComponent<Text>().text = "Koniec gry!           Uzyskany wynik:                   " + previousScore.ToString();
        
    }


    void Update()
    {
        

        if (Input.GetKeyUp(KeyCode.E))
        {
            Debug.Log("MiniGameScoreExit: " );
            MiniGameStatus.Instance.SetStatus("Gra o produktach ekologicznych", previousScore, result);
            SceneManager.LoadScene("LobbyScene");
        }
    }

    public void ReturnToLobby()
    {
        MiniGameStatus.Instance.SetStatus("Gra o produktach ekologicznych", previousScore, result);
        SceneManager.LoadScene("LobbyScene");
    }

    public void QuitToLobby()
    {
        MiniGameStatus.Instance.SetStatus("Gra o produktach ekologicznych", 0, false);
        SceneManager.LoadScene("LobbyScene");
    }
    

    public void UpdateDisplay(Product currentProduct, int score, float timeLeft)
    {

        if (currentProductDisplay.GetComponent<UnityEngine.UI.Image>().sprite == null || currentProduct.GetImage() != currentProductDisplay.GetComponent<UnityEngine.UI.Image>().sprite) 
        {
            UnityEngine.UI.Image img = currentProductDisplay.GetComponent<UnityEngine.UI.Image>();
            img.sprite = currentProduct.GetImage();
        }

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
    }
}
