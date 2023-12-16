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

    }

    void addToFriendsList(string nickname)
    {
        ItemFriend itemFriend = Instantiate(ItemFriendPrefab);
        itemFriend.setPlayerNickname(nickname);

        itemFriend.transform.parent = FriendContent.transform;
    }

    void addToInvitationsList(int InvitationID, int InvitingPlayerID, string InvitingPlayerNickname)
    {
        ItemInvitation itemInvitation = Instantiate(ItemInvitationPrefab);
        itemInvitation.setInvitationData(InvitationID, InvitingPlayerID, InvitingPlayerNickname);

        itemInvitation.transform.parent = InvitationContent.transform;
    }


    public void SendInvitation()
    {
        try
        {
            int invitedPlayerId = Int32.Parse(inputField.text);

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

        gameObject.SetActive(v);
    }

    public void closePanel()
    {
        show(false);
    }
}
