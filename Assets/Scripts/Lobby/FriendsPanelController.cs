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

    async void downloadFriendData()
    {
        var result = ServerAPI.Instance.GetLoggedUserData();
        UserData user = (UserData)result.Item2;
        List<string> friends = user.Friends;
        Dictionary<string, bool> friendsInvitations = user.FriendRequests;
        Debug.Log("NZ" + friends.Count);

        foreach (string freindId in friends)
        {
            //addToFriendsList(((UserData)ServerAPI.Instance.GetUserDataByID(freindId).Result.Item2).Nickname);
            addToFriendsList((await DataManager.Instance.GetUserByID(freindId)).Item2.Value.Nickname);
        }

        //for (int i = 0; i < friendsInvitations.Count; i++)
        int i = 1;
        foreach (string friendInvt in friendsInvitations.Keys)
        {
            //addToInvitationsList(i++, friendInvt, ((UserData)ServerAPI.Instance.GetUserDataByID(friendInvt).Result.Item2).Nickname);
            addToInvitationsList(i++, friendInvt, (await DataManager.Instance.GetUserByID(friendInvt)).Item2.Value.Nickname);
        }
    }

    void addToFriendsList(string nickname)
    {
        ItemFriend itemFriend = Instantiate(ItemFriendPrefab, FriendContent.transform);
        itemFriend.setPlayerNickname(nickname);

        //itemFriend.transform.parent = FriendContent.transform;
    }

    void addToInvitationsList(int InvitationID, string InvitingPlayerID, string InvitingPlayerNickname)
    {
        ItemInvitation itemInvitation = Instantiate(ItemInvitationPrefab, InvitationContent.transform);
        itemInvitation.setInvitationData(InvitationID, InvitingPlayerID, InvitingPlayerNickname);

        //itemInvitation.transform.parent = InvitationContent.transform;
    }


    public async void SendInvitation()
    {
        string invitedPlayerId = inputField.text;

        if (invitedPlayerId != PlayerData.GetInstance().getPlayerID())
        {
            Debug.Log("Inviting " + invitedPlayerId);
            await DataManager.Instance.SendFriendRequest(invitedPlayerId);
        }

        /*try
        {
        }
        catch (FormatException)
        {
            Debug.Log("Wyst¹pi³ b³¹d parsowania!");
        }*/

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
