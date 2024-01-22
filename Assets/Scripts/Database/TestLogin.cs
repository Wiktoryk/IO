using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLogin : MonoBehaviour
{
    public InputField email;
    public InputField password;

    public Canvas loginCanvas;

    public Canvas infoCanvas;

    public Text nickname;

    public Text emailLabel;

    public Text score0;
    public Text score1;
    public Text score2;
    public Text score3;
    public Text score4;
    public Text score5;
    public Text score6;
    public Text score7;

    public async void login()
    {   
        if (await DataManager.Instance.Login(email.text, password.text) == ServerLogInError.None)
        {
            loginCanvas.enabled = false;
            infoCanvas.enabled = true;

            var userData = await DataManager.Instance.FetchUserData();

            if (userData.Item1 == ServerSearchError.None) 
            {
                nickname.text = userData.Item2.Value.Nickname;
                emailLabel.text = userData.Item2.Value.Email;

                for(int i = 0; i < 8; ++i) {
                    if (i == 0)
                    {
                        score0.text = userData.Item2.Value.Highscores[i].ToString();
                    }
                    if (i == 1)
                    {
                        score1.text = userData.Item2.Value.Highscores[i].ToString();
                    }
                    if (i == 2)
                    {
                        score2.text = userData.Item2.Value.Highscores[i].ToString();
                    }
                    if (i == 3)
                    {
                        score3.text = userData.Item2.Value.Highscores[i].ToString();
                    }
                    if (i == 4)
                    {
                        score4.text = userData.Item2.Value.Highscores[i].ToString();
                    }
                    if (i == 5)
                    {
                        score5.text = userData.Item2.Value.Highscores[i].ToString();
                    }
                    if (i == 6)
                    {
                        score6.text = userData.Item2.Value.Highscores[i].ToString();
                    }
                    if (i == 7)
                    {
                        score7.text = userData.Item2.Value.Highscores[i].ToString();
                    }
                }
            }
            else 
            {
                Debug.LogWarning("Error fetching user data");

                nickname.text = 0.ToString();
                emailLabel.text = 0.ToString();

                for(int i = 0; i < 8; ++i) {
                    if (i == 0)
                    {
                        score0.text = 0.ToString();
                    }
                    if (i == 1)
                    {
                        score1.text = 0.ToString();
                    }
                    if (i == 2)
                    {
                        score2.text = 0.ToString();
                    }
                    if (i == 3)
                    {
                        score3.text = 0.ToString();
                    }
                    if (i == 4)
                    {
                        score4.text = 0.ToString();
                    }
                    if (i == 5)
                    {
                        score5.text = 0.ToString();
                    }
                    if (i == 6)
                    {
                        score6.text = 0.ToString();
                    }
                    if (i == 7)
                    {
                        score7.text = 0.ToString();
                    }
                }
            }
        }
    }

    public void logout()
    {
        DataManager.Instance.Logout();

    }
}
