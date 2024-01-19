using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndLevelPanelController : MonoBehaviour
{
    [SerializeField]
    private GameObject successLabel;
    [SerializeField]
    private GameObject defeatLabel;
    [SerializeField]
    private TextMeshProUGUI miniGameName;
    [SerializeField]
    private TextMeshProUGUI resultScore;
    [SerializeField]
    private SendChallengePanelController sendChallengePanel;

    public void OpenPanel(MiniGameStatus miniGameStatus)
    {
        gameObject.SetActive(true);

        if (miniGameStatus.Success)
        {
            successLabel.SetActive(true);
            defeatLabel.SetActive(false);
        }
        else
        {
            defeatLabel.SetActive(true);
            successLabel.SetActive(false);
        }

        miniGameName.text = miniGameStatus.MiniGameName;
        resultScore.text = miniGameStatus.Score.ToString();
    }

    public void ToLobby()
    {
        gameObject.SetActive(false);
    }

    public void Challenge()
    {
        gameObject.SetActive(false);
        sendChallengePanel.OpenPanel();
    }
}
