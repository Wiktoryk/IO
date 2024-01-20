using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SendChallengePanelController : MonoBehaviour
{

    [SerializeField]
    private GameObject content;

    [SerializeField]
    private GameObject listItemPrefab;

    [SerializeField]
    private ScrollListChoosableItem selectedItem;

    [SerializeField]
    private Button chooseButton;

    // Start is called before the first frame update
    void Start()
    {
        chooseButton.enabled = false;
    }

    private void GenerateFriendList()
    {
        Object[] friendList = new Object[3];// = GetFriendList();

        foreach (Object obj in friendList)
        {
            GameObject friend = Instantiate(listItemPrefab, content.transform);
            friend.GetComponent<ScrollListChoosableItem>().Init("Player1", "0", this);
        }

    }

    private void Clear()
    {
        selectedItem = null;
        
        for (int i = 0; i < content.transform.childCount; i++)
        {
            DestroyImmediate(content.transform.GetChild(i).gameObject);
        }
    }

    public void Cancel()
    {
        Clear();
        gameObject.SetActive(false);
    }

    public void ChooseFriend()
    {
        ChallengeData challengeData = new ChallengeData();
        challengeData.MinigameID = 0;
        challengeData.UserID = PlayerData.GetInstance().getPlayerID();
        challengeData.Score = 0;

        DataManager.Instance.sendChallenge(selectedItem.id, challengeData);

        Clear();
        gameObject.SetActive(false);
    }

    public void SelectFriend(ScrollListChoosableItem listElement)
    {
        selectedItem = listElement;
        chooseButton.enabled = true;
    }

    public void OpenPanel()
    {
        GenerateFriendList();
        gameObject.SetActive(true);
    }
}
