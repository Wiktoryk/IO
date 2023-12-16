using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemFriend : MonoBehaviour
{
    public Text PlayerNickname;


    public void setPlayerNickname(string nickName)
    {
        PlayerNickname.text = nickName;
    }
}
