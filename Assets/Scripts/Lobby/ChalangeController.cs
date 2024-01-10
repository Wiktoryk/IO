using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalangeController : MonoBehaviour
{
    private static ChalangeController instance = null;

    private int chalangeId = 0;
    private string chalangingPlayerId = "";
    private float chalangingPlayerScore = 0;


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

    public void setChalangeData(int ChalangeId, string ChalangingPlayerId, float ChalangingPlayerScore)
    {
        chalangeId = ChalangeId;
        chalangingPlayerId = ChalangingPlayerId;
        chalangingPlayerScore = ChalangingPlayerScore;
    }

    public void CompleteChalange(int ChalangedPlayerScore)
    {


        Destroy(gameObject);
    }
}
