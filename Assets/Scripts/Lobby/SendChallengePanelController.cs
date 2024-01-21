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

    private async void GenerateFriendList()
    {
        Object[] friendList = new Object[3];// = GetFriendList();

        List<string> friends = ((UserData)DataManager.Instance.GetLoggedUser().Item2).Friends;

        foreach (string friendID in friends)
        {
            UserData data = (UserData)(await DataManager.Instance.GetUserByID(friendID)).Item2;
            GameObject friend = Instantiate(listItemPrefab, content.transform);
            friend.GetComponent<ScrollListChoosableItem>().Init(data.Nickname, friendID, this);
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
        challengeData.MinigameID = MiniGameStatus.Instance.MinigameId;
        challengeData.UserID = PlayerData.GetInstance().getPlayerID();
        challengeData.Score = MiniGameStatus.Instance.Score;

        DataManager.Instance.SendChallenge(selectedItem.id, challengeData);

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
