using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInvitation : MonoBehaviour
{
    private int invitationId = 0;
    private string invitingPlayerId = "";

    public Text InvitingPlayerNicknameText;


    public void setInvitationData(int InvitationID, string InvitingPlayerID, string InvitingPlayerNickname)
    {
        invitationId = InvitationID;
        invitingPlayerId = InvitingPlayerID;

        InvitingPlayerNicknameText.text = InvitingPlayerNickname;
    }

    public void AcceptInvitation()
    {
        UserData user = (UserData)ServerAPI.Instance.GetLoggedUserData().Item2;
        UserData otherUser = (UserData)ServerAPI.Instance.GetUserDataByID(invitingPlayerId).Item2;

        user.Friends.Add(invitingPlayerId);
        otherUser.Friends.Add(user.ID);

        user.FriendRequests.Remove(invitingPlayerId);

        Destroy(gameObject);
    }

    public void DeclineInvitation()
    {
        UserData user = (UserData)ServerAPI.Instance.GetLoggedUserData().Item2;

        user.FriendRequests.Remove(invitingPlayerId);

        Destroy(gameObject);
    }
}
