using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameStatus : MonoBehaviour
{
    private static MiniGameStatus instance = null;
    public static MiniGameStatus Instance { get => instance; }


    private string miniGameName;
    private int score;
    private int minigameId = -1;
    private bool ended = false;
    private bool success = true;

    public string MiniGameName { get => miniGameName; } //set => miniGameName = value;
    public int Score { get => score; }
    public bool Success { get => success; }
    public int MinigameId { set => minigameId = value; get => minigameId; }

    /**
     * Reading Ended property automaticly resets its value after end of this operation
     */
    public bool Ended { 
        get {
            bool temp = ended;
            ended = false;
            return temp; 
        } 
    }

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    /**
     * Method used for setting state of MiniGameStatus. It also automaticly sets value of property Ended to true.
     */
    public void SetStatus(string miniGameName, int score, bool success = true)
    {
        this.miniGameName = miniGameName;
        this.score = score;
        this.success = success;
        ended = true;
    }

    public void OnDestroy()
    {
        if (this == instance)
        {
            instance = null;
        }
    }
}
