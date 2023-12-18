using UnityEngine;
//using UnityEngine.Debug;
using Firebase;
using Firebase.Auth;
using System.Collections;
//using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class DataManager : MonoBehaviour, IDataManager
{
    public DataManager()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        dataManager = new DataManager();
        firebaseInitialized = false;
        loggedIn = false;
    }

    public FirebaseAuth auth;
    public FirebaseUser user;
    private IDataManager dataManager;
   // private ServerAPI serverAPI;

    private bool firebaseInitialized = false;
    private bool loggedIn = false;
    public DependencyStatus dependencyStatus;

    private ServerAPI serverAPI = new ServerAPI();

    public ServerLogInError Login(string email, string password)
    {
        return serverAPI.Login(email, password);
    }

    public ServerRegisterError Register(string email, string password, string nickname)
    {
        return serverAPI.Register(email, password, nickname);
    }

    public bool Logout()
    {
        return serverAPI.Logout();
    }

    public ServerUserUpdateError updateUser(UserData newUserData)
    {
        var result = serverAPI.GetLoggedUserData();

        if (result.Item1 != ServerSearchError.None)
        {
            return ServerUserUpdateError.UserNotLoggedIn;
        }

        UserData currentUserData = result.Item2.Value;


        return serverAPI.UpdateUserData(currentUserData);
    }

    public List<int> fetchMiniGamesList()
    {
        return serverAPI.GetMinigamesIDs();
    }

     public ServerUserUpdateError changeNickname(string newNickname)
{
    // Wywołanie metody UpdateUserNicknameAuth
    bool result = serverAPI.UpdateUserNicknameAuth(newNickname);

    // Przekształcenie wyniku na ServerUserUpdateError
    return result ? ServerUserUpdateError.None : ServerUserUpdateError.NicknameUpdateFailed;
}


    public async Task<AuthError?> changePassword(string newPassword)
    {
        if (auth.CurrentUser == null)
        {
            return AuthError.UserNotFound;
        }

        var result = await auth.CurrentUser.UpdatePasswordAsync(newPassword).ContinueWith(task =>
        {
            
            if (task.IsCanceled)
            {
                UnityEngine.Debug.LogError("UpdatePasswordAsync was canceled.");
                return AuthError.Cancelled;
            }
            if (task.IsFaulted)
            {
                UnityEngine.Debug.LogError("UpdatePasswordAsync encountered an error: " + task.Exception);
                return AuthError.Cancelled;
            }

            UnityEngine.Debug.Log("Password successfully changed.");
            return AuthError.None;
        });

        return result;
    }

    public bool SendFriendRequest(string friendId)
    {
        // Wysy�amy zaproszenie do znajomych do bazy danych
        bool sendRequestResult = serverAPI.SendFriendRequestDatabase(friendId, true);
        if (!sendRequestResult)
        {
            Debug.LogWarning("Failed to send friend request");
            return false;
        }

        // Dodajemy zaproszenie do listy zaprosze� u�ytkownika
        bool addUserFriendInvitesResult = serverAPI.AddUserFriendInvitesDatabase(friendId);
        if (!addUserFriendInvitesResult)
        {
            Debug.LogWarning("Failed to add friend invite to user's list");
            return false;
        }

        return true;
    }

    public bool CancelFriendRequest(string friendId)
    {
        // Usuwamy zaproszenie z listy zaprosze� u�ytkownika
        bool deleteUserFriendInvitesResult = serverAPI.DeleteUserFriendInvitesDatabase(friendId);
        if (!deleteUserFriendInvitesResult)
        {
            Debug.LogWarning("Failed to delete friend invite from user's list");
            return false;
        }

        // Usuwamy zaproszenie z bazy danych
        bool deleteFriendRequestResult = serverAPI.DeleteFriendRequestDatabase(friendId); 
        if (!deleteFriendRequestResult)
        {
            Debug.LogWarning("Failed to delete friend request from database");
            return false;
        }

        return true;
    }

    public bool SaveScore(int minigameId, float score)
    {
        // Pobieramy aktualny najlepszy wynik u�ytkownika
        float currentHighscore = 0;
        if (minigameId < serverAPI.GetLoggedUser().Value.Highscores.Count)
        {
            currentHighscore = serverAPI.GetLoggedUser().Value.Highscores[minigameId];
        }

        // Je�li nowy wynik jest wy�szy ni� aktualny najlepszy wynik, aktualizujemy go
        if (score > currentHighscore)
        {
            return serverAPI.UpdateUserHighscoreDatabase(minigameId, score);
        }

        // Je�li nowy wynik nie jest wy�szy, nie robimy nic
        return true;
    }

    public async Task<(ServerSearchError, UserData?)> fetchUserData()
    {
        var result = serverAPI.GetLoggedUserDatabase();

        if (result.Item1 != ServerSearchError.None)
        {
            Debug.LogWarning($"Failed to fetch user data with {result.Item1}");
        }

        return result;
    }

    public bool sendChallenge(string friendId, ChallengeData challenge)
    {
        return serverAPI.SendChallangeDatabase(friendId, challenge);
    }

    public bool cancelChallenge(ChallengeData challenge)
    {
        return serverAPI.DeleteChallangeDatabase(challenge);
    }

    public bool RespondFriendRequest(string friendId, bool accept)
    {
        if (accept)
        {
            // 1. Usu� zaproszenie do znajomych z listy zaprosze� u�ytkownika
            if (!serverAPI.DeleteUserFriendInvitesDatabase(friendId))
            {
                Debug.LogWarning("Failed to delete friend invite from user's list");
                return false;
            }

            // 2. Dodaj nowego znajomego do listy znajomych u�ytkownika
            if (!serverAPI.UpdateUserFriendsListDatabase(friendId))
            {
                Debug.LogWarning("Failed to add friend to user's list");
                return false;
            }

            // 3. Usu� wys�ane zaproszenie do znajomych z listy zaprosze� znajomego
            if (!serverAPI.DeleteFriendRequestDatabase(friendId))
            {
                Debug.LogWarning("Failed to delete friend request from friend's list");
                return false;
            }

        }
        else
        {
            // Je�li u�ytkownik nie akceptuje zaproszenia, usu� zaproszenie z listy zaprosze� u�ytkownika
            if (!serverAPI.DeleteUserFriendInvitesDatabase(friendId))
            {
                Debug.LogWarning("Failed to delete friend invite from user's list");
                return false;
            }

            // i usu� wys�ane zaproszenie do znajomych z listy zaprosze� znajomego
            if (!serverAPI.DeleteFriendRequestDatabase(friendId))
            {
                Debug.LogWarning("Failed to delete friend request from friend's list");
                return false;
            }
        }

        return true;
    }

    public bool AcceptChallenge(string friendId, ChallengeData challengeResponse)
    {
        // 1. Wy�lij odpowied� na wyzwanie do bazy danych
        if (!serverAPI.SendChallangeDatabase(friendId, challengeResponse))
        {
            Debug.LogWarning("Failed to send challenge response");
            return false;
        }

        // 2. Usu� otrzymane wyzwanie z listy wyzwa� u�ytkownika
        if (!serverAPI.DeleteChallangeDatabase(challengeResponse))
        {
            Debug.LogWarning("Failed to delete received challenge");
            return false;
        }

        return true;
    }

    public (ServerSearchError, UserData?) GetUserByNickname(string nickname)
    {
        return serverAPI.GetUserDataByNickname(nickname);
    }

    public (ServerSearchError, UserData?) GetUserByEmail(string email)
    {
        return serverAPI.GetUserDataByEmail(email);
    }

    public (ServerSearchError, UserData?) GetUserID(string id)
    {
        return serverAPI.GetUserDataByID(id);
    }
}