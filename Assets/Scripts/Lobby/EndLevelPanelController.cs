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

    [SerializeField]
    private GameObject challengePart;
    [SerializeField]
    private TextMeshProUGUI challengedPlayername;
    [SerializeField]
    private TextMeshProUGUI challengedPlayerScore;

    public async void OpenPanel(MiniGameStatus miniGameStatus)
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

        //je�li rozpocz�t� minigr� wyzwanie to zako�czy si� ono tutaj
        if (ChalangeController.Instance() != null)
        {
            ChallengeData challengeData = new ChallengeData();

            challengeData.Score = miniGameStatus.Score;
            challengeData.UserID = PlayerData.GetInstance().getPlayerID();
            challengeData.MinigameID = ChalangeController.Instance().minigameTypeId;

            challengePart.SetActive(true);
            if (miniGameStatus.Score >= ChalangeController.Instance().chalangingPlayerScore)
            {
                successLabel.SetActive(true);
                defeatLabel.SetActive(false);
            }
            else
            {
                defeatLabel.SetActive(true);
                successLabel.SetActive(false);
            }

            UserData data = (UserData) (await DataManager.Instance.GetUserByID(ChalangeController.Instance().chalangingPlayerId)).Item2;
            challengedPlayername.text = data.Nickname;
            challengedPlayerScore.text = ChalangeController.Instance().chalangingPlayerScore.ToString();

            DataManager.Instance.AcceptChallenge(ChalangeController.Instance().chalangingPlayerId, challengeData);
            Destroy(ChalangeController.Instance().gameObject);
        }/**/

        //Sprawdzanie nowego najlepszego wyniku      //typ minigry
        if (miniGameStatus.Score > PlayerData.GetInstance().getMinigameHighScore((PlayerData.MinigameType)MiniGameStatus.Instance.MinigameId))
        {
            PlayerData.GetInstance().setMinigameHighScore((PlayerData.MinigameType)MiniGameStatus.Instance.MinigameId, miniGameStatus.Score);
            //PlayerData.GetInstance().UploadPlayerData();
        }/**/

        miniGameName.text = miniGameStatus.MiniGameName;
        resultScore.text = miniGameStatus.Score.ToString();
    }

    public void ToLobby()
    {
        challengePart.SetActive(false);
        gameObject.SetActive(false);
    }

    public void Challenge()
    {
        challengePart.SetActive(false);
        gameObject.SetActive(false);
        sendChallengePanel.OpenPanel();
    }
}
