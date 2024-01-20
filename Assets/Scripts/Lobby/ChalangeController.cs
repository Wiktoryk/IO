using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalangeController : MonoBehaviour
{
    private static ChalangeController instance = null;

    public int chalangeId = 0;
    public string chalangingPlayerId = "";
    public float chalangingPlayerScore = 0;
    public int minigameTypeId = -1;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static ChalangeController Instance()
    {
        return instance;
    }

    public void setChalangeData(int ChalangeId, string ChalangingPlayerId, float ChalangingPlayerScore, int MinigameTypeId)
    {
        chalangeId = ChalangeId;
        chalangingPlayerId = ChalangingPlayerId;
        chalangingPlayerScore = ChalangingPlayerScore;
        minigameTypeId = MinigameTypeId;
    }

    public void CompleteChalange(int ChalangedPlayerScore)
    {


        Destroy(gameObject);
    }
}
