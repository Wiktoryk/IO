using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    public int score = 0;
    // Start is called before the first frame update
    public int getScore()
    {
        return score;
    }
    public void UpdateScore(int newScore)
    {
        score = newScore;
    }
}
