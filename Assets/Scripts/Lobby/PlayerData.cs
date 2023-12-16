using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData //: MonoBehaviour
{
    private static PlayerData instance = null;
    private int[] minigamesHighScores = {0, 0, 0, 0, 0, 0, 0, 0};
    private int playerID = 0;
    private string nickname = "Nickname";
    private int level = 0;

    public enum MinigameType:int { MG_A = 0, MG_B, MG_C, MG_D, MG_E, MG_F, MG_G, MG_H };

    public int getPlayerID() 
    {
        return playerID;
    }

    public void setPlayerID(int PlayerId)
    {
        playerID = PlayerId;
    }

    public string getNickname() 
    {
        return nickname;
    }

    public int getLevel()
    {
        return level;
    }
    private PlayerData() { }

    /*private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }*/

    public static PlayerData GetInstance()
    {
        if (instance == null)
        {
            instance = new PlayerData(); //Instance(PlayerData);
        }

        return instance;
    }

    public bool DownloadPlayerData()
    {


        return true;
    }

    public bool UploadPlayerData()
    {


        return true;
    }

    public int getMinigameHighScore(MinigameType type)
    {
        return minigamesHighScores[(int)type];
    }

    public void setMinigameHighScore(MinigameType type, int newHighScore)
    {
        minigamesHighScores[(int)type] = newHighScore;
    }
}
