using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecyklingGameScript : MonoBehaviour
{
    public Timer gameTimer;
    void Start()
    {
        gameTimer.timeLeft = 90;
        gameTimer.timerOn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameTimer.timeLeft > 0)
        {

        }
        else
        {
            Debug.Log("Time has run out!");
            gameTimer.timerOn = false;
            gameTimer.timeLeft = 0;
        }
    }
}
