using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameStatus : MonoBehaviour
{
    private static MiniGameStatus instance = null;
    public static MiniGameStatus Instance { get => instance; }


    private string miniGameName;
    private int score;
    private bool ended = false;

    public string MiniGameName { get => miniGameName; } //set => miniGameName = value;
    public int Score { get => score; }

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

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Method used for setting state of MiniGameStatus. It also automaticly sets value of property Ended to true.
     */
    public void SetStatus(string miniGameName, int score)
    {
        this.miniGameName = miniGameName;
        this.score = score;
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
