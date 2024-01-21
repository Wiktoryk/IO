using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginRegistrationManager : MonoBehaviour
{
    public InputField LoginEmail;
    public InputField LoginPassword;

    public InputField RegistrationEmail;
    public InputField RegistrationNickname;
    public InputField RegistrationPassword;

    public GameObject LoginPanel;
    public GameObject RegistrationPanel;

    async void Start()
    {
        await DataManager.Instance.Init();
    }

    public async void Login()
    {
        await DataManager.Instance.Login(LoginEmail.text, LoginPassword.text);

        SceneManager.LoadScene("LobbyScene");
    }

    public void ToRegistration()
    {
        LoginPanel.SetActive(false);
        RegistrationPanel.SetActive(true);
    }

    public void ToLogin()
    {
        LoginPanel.SetActive(true);
        RegistrationPanel.SetActive(false);
    }

    public async void Register()
    {
        await DataManager.Instance.Register(RegistrationEmail.text, RegistrationPassword.text, RegistrationNickname.text);

        SceneManager.LoadScene("LobbyScene");
    }
}
