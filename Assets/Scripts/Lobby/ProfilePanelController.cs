using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfilePanelController : MonoBehaviour
{
    private PlayerData playerData = PlayerData.GetInstance();

    [Header("Texts containing player's data")]
    public Text ID;
    public Text NicknameText;
    public Text LevelText;
    public Text[] MinigamesScores = new Text[8];

    [Header("Changings panels")]
    public GameObject BlindingPanel;
    public GameObject ChangingNicknamePanel;
    public GameObject ChangingPasswordPanel;


    void setMinigameHighScoreText(PlayerData.MinigameType minigameType)
    {
        MinigamesScores[(int)minigameType].text = playerData.getMinigameHighScore(minigameType).ToString();
    }


    public void downloadPlayerData()
    {
        if (playerData.DownloadPlayerData())
        {
            ID.text = playerData.getPlayerID().ToString();
            NicknameText.text = playerData.getNickname();
            LevelText.text = playerData.getLevel().ToString();

            setMinigameHighScoreText(PlayerData.MinigameType.MG_A);
            setMinigameHighScoreText(PlayerData.MinigameType.MG_B);
            setMinigameHighScoreText(PlayerData.MinigameType.MG_C);
            setMinigameHighScoreText(PlayerData.MinigameType.MG_D);
            setMinigameHighScoreText(PlayerData.MinigameType.MG_E);
            setMinigameHighScoreText(PlayerData.MinigameType.MG_F);
            setMinigameHighScoreText(PlayerData.MinigameType.MG_G);
            setMinigameHighScoreText(PlayerData.MinigameType.MG_H);
        }
    }

    public void uploadPlayerData()
    {
        playerData.UploadPlayerData();
    }

    public void DisplayChangePasswordPopup()
    {
        BlindingPanel.SetActive(true);
        ChangingPasswordPanel.SetActive(true);
    }

    public void DisplayChangeNicknamePopup()
    {
        BlindingPanel.SetActive(true);
        ChangingNicknamePanel.SetActive(true);
    }

    public void ChangePassword()
    {
        string newPassword = ChangingPasswordPanel.GetComponentInChildren<TMP_InputField>().text;

        ChangingPasswordPanel.SetActive(false);
        BlindingPanel.SetActive(false);
    }

    public void ChangeNickname()
    {
        string newNickname = ChangingNicknamePanel.GetComponentInChildren<TMP_InputField>().text;

        ChangingNicknamePanel.SetActive(false);
        BlindingPanel.SetActive(false);
    }

    public void show(bool v)
    {
        if (v)
        {
            downloadPlayerData();
        }

        gameObject.SetActive(v);
    }

    public void closePanel()
    {
        show(false);
    }
}
