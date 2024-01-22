using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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


    public void Logout()
    {
        DataManager.Instance.Logout();

        SceneManager.LoadScene("LoginRegistration");
    }

    void setMinigameHighScoreText(PlayerData.MinigameType minigameType)
    {
        MinigamesScores[(int)minigameType].text = playerData.getMinigameHighScore(minigameType).ToString();
    }


    public async Task downloadPlayerData()
    {
        if (await playerData.DownloadPlayerData())
        {
            ID.text = playerData.getPlayerID().ToString();
            NicknameText.text = playerData.getNickname();
            LevelText.text = (playerData.getXp() / 1000 + 1).ToString();

            setMinigameHighScoreText(PlayerData.MinigameType.RECYCLING);
            setMinigameHighScoreText(PlayerData.MinigameType.WATER_SAVING);
            setMinigameHighScoreText(PlayerData.MinigameType.ECO_PRODS);
            setMinigameHighScoreText(PlayerData.MinigameType.ENERGY_SAVING);
            setMinigameHighScoreText(PlayerData.MinigameType.REUSING);
            setMinigameHighScoreText(PlayerData.MinigameType.PUBLIC_TRANSP);
            setMinigameHighScoreText(PlayerData.MinigameType.WASTES);
            setMinigameHighScoreText(PlayerData.MinigameType.PAPPER_SAVING);
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
        string newPassword = ChangingPasswordPanel.GetComponentInChildren<InputField>().text;

        DataManager.Instance.ChangePassword(newPassword);

        ChangingPasswordPanel.SetActive(false);
        BlindingPanel.SetActive(false);
    }

    public async void ChangeNickname()
    {
        string newNickname = ChangingNicknamePanel.GetComponentInChildren<InputField>().text;

        await DataManager.Instance.ChangeNickname(newNickname);
        playerData.setNickname(newNickname);
        playerData.UploadPlayerData();

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
