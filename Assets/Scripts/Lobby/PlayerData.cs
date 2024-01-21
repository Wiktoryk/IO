using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerData //: MonoBehaviour
{
    private static PlayerData instance = null;
    private float[] minigamesHighScores = { 0, 0, 0, 0, 0, 0, 0, 0 };
    private string playerID = "";
    private string nickname = "Nickname";
    private int level = 0;

    public enum MinigameType : int { RECYCLING = 0, WATER_SAVING, ECO_PRODS, ENERGY_SAVING, REUSING, PUBLIC_TRANSP, WASTES, PAPPER_SAVING };
    //{ RECYCLING = 0, WATER_SAVING, ECO_PRODS, ENERGY_SAVING, REUSING, PUBLIC_TRANSP, WASTES, PAPPER_SAVING };

public string getPlayerID()
    {
        return playerID;
    }

    public void setPlayerID(string PlayerId)
    {
        playerID = PlayerId;
    }

    public string getNickname()
    {
        return nickname;
    }

    public void setNickname(string newNickname)
    {
        nickname = newNickname;
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
        var result = DataManager.Instance.GetLoggedUser();
        UserData udata = (UserData)result.Item2;

        playerID = udata.ID;
        nickname = udata.Nickname;
        List<float> hs = udata.Highscores;

        for (int i = 0; i < hs.Count; i++)
        {
            minigamesHighScores[i] = hs[i];
        }

        return true;
    }

    public async Task<bool> UploadPlayerData()
    {
        var result = DataManager.Instance.GetLoggedUser();
        UserData udata = (UserData)result.Item2;

        for (int i = 0; i < udata.Highscores.Count; i++)
        {
            udata.Highscores[i] = minigamesHighScores[i];
        }

        await DataManager.Instance.UpdateUser(udata);

        return true;
    }

    public float getMinigameHighScore(MinigameType type)
    {
        return minigamesHighScores[(int)type];
    }

    public void setMinigameHighScore(MinigameType type, float newHighScore)
    {
        minigamesHighScores[(int)type] = newHighScore;
        UploadPlayerData();
    }
}
