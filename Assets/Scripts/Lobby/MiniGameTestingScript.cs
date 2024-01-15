using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NPCScript;
using UnityEngine.SceneManagement;

public class MiniGameTestingScript : MonoBehaviour
{
    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            score++;
            Debug.Log("MiniGameScore: " +  score);
        }

        if (Input.GetKeyUp(KeyCode.KeypadEnter))
        {
            Debug.Log("MiniGameScoreExit: " + score);
            MiniGameStatus.Instance.SetStatus("MINI", score, true);
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
