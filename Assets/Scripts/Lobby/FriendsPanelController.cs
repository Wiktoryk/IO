using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Windows;

public class FriendsPanelController : MonoBehaviour
{
    public TMP_InputField inputField;

    [Header("Friend list elements")]
    public ItemFriend ItemFriendPrefab;
    public GameObject FriendContent;

    [Header("Friend list elements")]
    public ItemInvitation ItemInvitationPrefab;
    public GameObject InvitationContent;

    void downloadFriendData()
    {
        var result = ServerAPI.Instance.GetLoggedUserData();
        UserData user = (UserData)result.Item2;
        List<string> friends = user.Friends;
        Dictionary<string, bool> friendsInvitations = user.FriendRequests;

        foreach (string freindId in friends)
        {
            addToFriendsList(((UserData)ServerAPI.Instance.GetUserDataByID(freindId).Result.Item2).Nickname);
        }

        //for (int i = 0; i < friendsInvitations.Count; i++)
        int i = 1;
        foreach (string friendInvt in friendsInvitations.Keys)
        {
            addToInvitationsList(i++, friendInvt, ((UserData)ServerAPI.Instance.GetUserDataByID(friendInvt).Result.Item2).Nickname);
        }
    }

    void addToFriendsList(string nickname)
    {
        ItemFriend itemFriend = Instantiate(ItemFriendPrefab);
        itemFriend.setPlayerNickname(nickname);

        itemFriend.transform.parent = FriendContent.transform;
    }

    void addToInvitationsList(int InvitationID, string InvitingPlayerID, string InvitingPlayerNickname)
    {
        ItemInvitation itemInvitation = Instantiate(ItemInvitationPrefab);
        itemInvitation.setInvitationData(InvitationID, InvitingPlayerID, InvitingPlayerNickname);

        itemInvitation.transform.parent = InvitationContent.transform;
    }


    public void SendInvitation()
    {
        try
        {
            string invitedPlayerId = inputField.text;

            if (invitedPlayerId != PlayerData.GetInstance().getPlayerID())
            {
                Debug.Log(string.Format("Wys³ano zaproszenie do {0}", invitedPlayerId));
            }
        }
        catch (FormatException)
        {
            Debug.Log("Wyst¹pi³ b³¹d parsowania!");
        }

        inputField.text = "";
    }

    public void show(bool v)
    {
        if (v)
        {
            downloadFriendData();
        }
        else
        {
            ItemFriend[] friends = FriendContent.GetComponentsInChildren<ItemFriend>();
            ItemInvitation[] invitations = FriendContent.GetComponentsInChildren<ItemInvitation>();
            foreach (ItemFriend f in friends)
            {
                Destroy(f.gameObject);
            }
            foreach (ItemInvitation f in invitations)
            {
                Destroy(f.gameObject);
            }
        }

        gameObject.SetActive(v);
    }

    public void closePanel()
    {
        show(false);
    }
}
