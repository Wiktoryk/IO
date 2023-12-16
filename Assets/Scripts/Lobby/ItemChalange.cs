using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemChalange : MonoBehaviour
{
    private int chalangeId = 0;
    private int chalagingPlayerId = 0;
    private int minigameTypeId = -1;
    private int chalangingPlayerScore = 0;

    public ChalangeController ChalangeControllerPrefab;
    public Text ChalangingPlayerNicknameText;
    public LobbyActionController lobbyActionController;

    public void setChalangeData(int ChalangeId, int ChalangingPlayerId, int MinigameTypeIndex, int ChalangingPlayerScore, string ChalangingPlayerNickname)
    {
        chalangeId = ChalangeId;
        chalagingPlayerId = ChalangingPlayerId;
        minigameTypeId = MinigameTypeIndex;
        chalangingPlayerScore = ChalangingPlayerScore;

        ChalangingPlayerNicknameText.text = ChalangingPlayerNickname;
    }

    public void AcceptChalange()
    {
        ChalangeController chalangeController = Instantiate(ChalangeControllerPrefab);
        chalangeController.setChalangeData(chalangeId, chalagingPlayerId, chalangingPlayerScore);

        lobbyActionController.GoToMinigame(minigameTypeId);
    }

    public void DeclineChalange()
    {


        Destroy(gameObject);
    }
}
