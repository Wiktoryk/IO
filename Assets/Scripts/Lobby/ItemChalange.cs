using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemChalange : MonoBehaviour
{
    private int chalangeId = 0;
    private string chalagingPlayerId = "";
    private int minigameTypeId = -1;
    private float chalangingPlayerScore = 0;
    ChallengeData challengeData;

    public ChalangeController ChalangeControllerPrefab;
    public Text ChalangingPlayerNicknameText;

    public void setChalangeData(int ChalangeId, string ChalangingPlayerId, int MinigameTypeIndex, float ChalangingPlayerScore, string ChalangingPlayerNickname, ChallengeData chData)
    {
        chalangeId = ChalangeId;
        chalagingPlayerId = ChalangingPlayerId;
        minigameTypeId = MinigameTypeIndex;
        chalangingPlayerScore = ChalangingPlayerScore;
        challengeData = chData;

        ChalangingPlayerNicknameText.text = ChalangingPlayerNickname;
    }

    public void AcceptChalange()
    {
        ChalangeController chalangeController = Instantiate(ChalangeControllerPrefab);
        chalangeController.setChalangeData(chalangeId, chalagingPlayerId, chalangingPlayerScore, minigameTypeId);

        LobbyActionController.Instance().GoToMinigame(minigameTypeId);
    }

    public async void DeclineChalange()
    {
        await DataManager.Instance.CancelChallenge(challengeData);

        Destroy(gameObject);
    }
}
