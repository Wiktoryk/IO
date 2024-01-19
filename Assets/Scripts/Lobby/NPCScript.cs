using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCScript : MonoBehaviour
{
    public string statementsFileName = string.Empty;

    [SerializeField, TextArea]
    private string[] statements;

    static private readonly string npcsStatementsFolderPath = "Lobby/NPCStatements/";

    [System.Serializable]
    public class MiniGame
    {
        public string miniGameName;
        public string miniGameSceneName;

        public MiniGame(string miniGameName, string miniGameSceneName)
        {
            this.miniGameName = miniGameName;
            this.miniGameSceneName = miniGameSceneName;
        }

    }
    public MiniGame[] miniGames;

    // Start is called before the first frame update
    void Start()
    {
        //if (!string.IsNullOrEmpty(statementsFileName))
        //{
        //    TextAsset textFile = Resources.Load<TextAsset>(npcsStatementsFolderPath + statementsFileName);
        //    statements = textFile.text.Split('|', System.StringSplitOptions.RemoveEmptyEntries).Select(p => p.Trim()).ToArray();
        //    Resources.UnloadAsset(textFile);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string GetStatement() 
    { 
        return statements[Random.Range(0, statements.Length)]; 
    }

    public string[] GetMiniGamesNames()
    {
        string[] miniGamesNames = new string[miniGames.Length];

        for (int i = 0; i < miniGames.Length; i++)
        {
            if (!string.IsNullOrEmpty(miniGames[i].miniGameSceneName))
            {
                miniGamesNames[i] = miniGames[i].miniGameName;
            }
        }

        return miniGamesNames;
    }

    public void LoadMiniGame(string miniGameName)
    {
        foreach (MiniGame miniGame in miniGames) 
        { 
            if (miniGame.miniGameName == miniGameName)
            {
                SceneManager.LoadScene(miniGame.miniGameSceneName);     
                break;
            }
        }
    }
}
