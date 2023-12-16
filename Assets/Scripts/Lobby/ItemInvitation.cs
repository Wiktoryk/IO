using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemInvitation : MonoBehaviour
{
    private int invitationId = 0;
    private int invitingPlayerId = 0;

    public Text InvitingPlayerNicknameText;


    public void setInvitationData(int InvitationID, int InvitingPlayerID, string InvitingPlayerNickname)
    {
        invitationId = InvitationID;
        invitingPlayerId = InvitingPlayerID;

        InvitingPlayerNicknameText.text = InvitingPlayerNickname;
    }

    public void AcceptInvitation()
    {


        Destroy(gameObject);
    }

    public void DeclineInvitation()
    {


        Destroy(gameObject);
    }
}
