using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalangesPanelController : MonoBehaviour
{
    public ItemChalange ItemChalangePrefab;
    public GameObject ChalangeListContent;


    void downloadChalangesData()
    {
        var result = ServerAPI.Instance.GetLoggedUserData();
        UserData user = (UserData)result.Item2;
        List<ChallengeData> challenges = user.ChallengeData;

        for (int i = 0; i < challenges.Count; i++)
        {
            addToChalangeList(i, challenges[i].UserID, challenges[i].MinigameID, 
                challenges[i].Score, ((UserData)ServerAPI.Instance.GetUserDataByID(challenges[i].UserID).Item2).Nickname, challenges[i]);
        }
    }

    void addToChalangeList(int ChalangeId, string ChalangingPlayerId, int MinigameTypeIndex, float ChalangingPlayerScore, string ChalangingPlayerNickname, ChallengeData challengeData)
    {
        ItemChalange itemChalange = Instantiate(ItemChalangePrefab);
        itemChalange.setChalangeData(ChalangeId, ChalangingPlayerId, MinigameTypeIndex, ChalangingPlayerScore, ChalangingPlayerNickname, challengeData);

        itemChalange.transform.parent = ChalangeListContent.transform;
    }

    public void show(bool v)
    {
        if (v)
        {
            downloadChalangesData();

            ItemChalange[] chalanges = ChalangeListContent.GetComponentsInChildren<ItemChalange>();

            foreach (ItemChalange ch in chalanges)
            {
                Destroy(ch.gameObject);
            }
        }

        gameObject.SetActive(v);
    }

    public void closePanel()
    {
        show(false);
    }
}
