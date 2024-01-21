using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    public void ReturnToLobby()
    {
        SceneManager.LoadScene(0);
    }
    private void EndGame()
    {
        gameObject.GetComponent<BioProductsManager>().enabled = false;
        currentProductDisplay.GetComponent<UnityEngine.UI.Image>().sprite = null;
        gameDisplay.SetActive(false);
        endGameDisplay.SetActive(true);

        GameObject endText = GameObject.Find("FinishText");
        endText.GetComponent<Text>().text = "Congratulations!           Your final score:                   " + previousScore.ToString();
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
            textField.text = "Score: " + score.ToString();
            previousScore = score;
        }

        UnityEngine.UI.Text timeField = timeDisplay.GetComponent<UnityEngine.UI.Text>();
        int time = (int)timeLeft;
        timeField.text = "Time left: " + time.ToString();

        if(time <= 0)
        {
            EndGame();
        }
    }
}
