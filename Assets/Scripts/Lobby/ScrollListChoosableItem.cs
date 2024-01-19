using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScrollListChoosableItem : MonoBehaviour
{
    private SendChallengePanelController parentController;
    public string friendName;
    public int id;

    public void Init(string friendName, int id, SendChallengePanelController parentController)
    {
        this.friendName = friendName;
        this.id = id;
        this.parentController = parentController;

        GetComponentInChildren<TextMeshProUGUI>().text = friendName;
    }

    public void Choose()
    {
        parentController.SelectFriend(this);
    }
}
