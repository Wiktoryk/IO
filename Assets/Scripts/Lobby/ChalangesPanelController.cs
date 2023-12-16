using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChalangesPanelController : MonoBehaviour
{
    public ItemChalange ItemChalangePrefab;
    public GameObject ChalangeListContent;


    void downloadChalangesData()
    {

    }

    void addToChalangeList(int ChalangeId, int ChalangingPlayerId, int MinigameTypeIndex, int ChalangingPlayerScore, string ChalangingPlayerNickname)
    {
        ItemChalange itemChalange = Instantiate(ItemChalangePrefab);
        itemChalange.setChalangeData(ChalangeId, ChalangingPlayerId, MinigameTypeIndex, ChalangingPlayerScore, ChalangingPlayerNickname);

        itemChalange.transform.parent = ChalangeListContent.transform;
    }

    public void show(bool v)
    {
        if (v)
        {
            downloadChalangesData();
        }

        gameObject.SetActive(v);
    }

    public void closePanel()
    {
        show(false);
    }
}
