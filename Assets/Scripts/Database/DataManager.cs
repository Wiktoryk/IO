using UnityEngine;
//using UnityEngine.Debug;
using Firebase;
using Firebase.Auth;
using System.Collections;
//using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Firebase.Database;

public class DataManager : IDataManager
{

    private static DataManager instance;
    public static DataManager Instance { 
        get {
            return instance ??= new DataManager();
        }
    }

    public DataManager()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;
        //dataManager = new DataManager();
        firebaseInitialized = false;
        loggedIn = false;
    }

    public FirebaseAuth auth;
    public FirebaseUser user;
    //private IDataManager dataManager;
   // private ServerAPI serverAPI;

    private bool firebaseInitialized = false;
    private bool loggedIn = false;
    public DependencyStatus dependencyStatus;

    public async Task Init()
    {
        await ServerAPI.Instance.Init();
    }

    public async Task<ServerLogInError> Login(string email, string password)
    {
        return await ServerAPI.Instance.Login(email, password);
    }

    public async Task<ServerRegisterError> Register(string email, string password, string nickname)
    {
        return await ServerAPI.Instance.Register(email, password, nickname);
    }

    public bool Logout()
    {
        return ServerAPI.Instance.Logout();
    }

    public async Task<ServerUserUpdateError> updateUser(UserData newUserData)
    {
        var result = ServerAPI.Instance.GetLoggedUserData();

        if (result.Item1 != ServerSearchError.None)
        {
            return ServerUserUpdateError.UserNotLoggedIn;
        }

        UserData currentUserData = result.Item2.Value;


        return await ServerAPI.Instance.UpdateUserData(currentUserData);
    }

    public async Task<List<int>> fetchMiniGamesList()
    {
        return await ServerAPI.Instance.GetMinigamesIDs();
    }

    public async Task<ServerUserUpdateError> changeNickname(string newNickname)
     {
         // Wywołanie metody UpdateUserNicknameAuth
         bool result = await ServerAPI.Instance.UpdateUserNicknameAuth(newNickname);

         // Przekształcenie wyniku na ServerUserUpdateError
         return result ? ServerUserUpdateError.None : ServerUserUpdateError.NicknameUpdateFailed;
     }

    public async Task<bool> changePassword(string newPassword)
    {
        return await ServerAPI.Instance.UpdateUserPasswordAuth(newPassword);
    }

    public async Task<bool> SendFriendRequest(string friendId)
    {
        // Wysy�amy zaproszenie do znajomych do bazy danych
        bool sendRequestResult = await ServerAPI.Instance.SendFriendRequestDatabase(friendId, true);
        if (!sendRequestResult)
        {
            Debug.LogWarning("Failed to send friend request");
            return false;
        }

        // Dodajemy zaproszenie do listy zaprosze� u�ytkownika
        bool addUserFriendInvitesResult = await ServerAPI.Instance.AddUserFriendInvitesDatabase(friendId);
        if (!addUserFriendInvitesResult)
        {
            Debug.LogWarning("Failed to add friend invite to user's list");
            return false;
        }

        return true;
    }

    public async Task<bool> CancelFriendRequest(string friendId)
    {
        // Usuwamy zaproszenie z listy zaprosze� u�ytkownika
        bool deleteUserFriendInvitesResult = await ServerAPI.Instance.DeleteUserFriendInvitesDatabase(friendId);
        if (!deleteUserFriendInvitesResult)
        {
            Debug.LogWarning("Failed to delete friend invite from user's list");
            return false;
        }

        // Usuwamy zaproszenie z bazy danych
        bool deleteFriendRequestResult = await ServerAPI.Instance.DeleteFriendRequestDatabase(friendId);
        if (!deleteFriendRequestResult)
        {
            Debug.LogWarning("Failed to delete friend request from database");
            return false;
        }

        return true;
    }

    public async Task<bool> SaveScore(int minigameId, float score)
    {
        // Pobieramy aktualny najlepszy wynik u�ytkownika
        float currentHighscore = 0;
        if (minigameId < ServerAPI.Instance.GetLoggedUserData().Item2.Value.Highscores.Count)
        {
            currentHighscore = ServerAPI.Instance.GetLoggedUserData().Item2.Value.Highscores[minigameId];
        }

        // Je�li nowy wynik jest wy�szy ni� aktualny najlepszy wynik, aktualizujemy go
        if (score > currentHighscore)
        {
            return await ServerAPI.Instance.UpdateUserHighscoreDatabase(minigameId, score);
        }

        // Je�li nowy wynik nie jest wy�szy, nie robimy nic
        return true;
    }

    public Tuple<ServerSearchError, UserData?> fetchUserData()
    {
        var result = ServerAPI.Instance.GetLoggedUserData();

        if (result.Item1 != ServerSearchError.None)
        {
            Debug.LogWarning($"Failed to fetch user data with {result.Item1}");
        }

        return result;
    }

    public async Task<bool> sendChallenge(string friendId, ChallengeData challenge)
    {
        return await ServerAPI.Instance.SendChallangeDatabase(friendId, challenge);
    }

    public async Task<bool> cancelChallenge(ChallengeData challenge)
    {
        return await ServerAPI.Instance.DeleteChallangeDatabase(challenge);
    }

    public async Task<bool> RespondFriendRequest(string friendId, bool accept)
    {
        if (accept)
        {
            // 1. Usu� zaproszenie do znajomych z listy zaprosze� u�ytkownika
            if (!(await ServerAPI.Instance.DeleteUserFriendInvitesDatabase(friendId)))
            {
                Debug.LogWarning("Failed to delete friend invite from user's list");
                return false;
            }

            // 2. Dodaj nowego znajomego do listy znajomych u�ytkownika
            if (!(await ServerAPI.Instance.UpdateUserFriendsListDatabase(friendId)))
            {
                Debug.LogWarning("Failed to add friend to user's list");
                return false;
            }

            // 3. Usu� wys�ane zaproszenie do znajomych z listy zaprosze� znajomego
            if (!(await ServerAPI.Instance.DeleteFriendRequestDatabase(friendId)))
            {
                Debug.LogWarning("Failed to delete friend request from friend's list");
                return false;
            }

        }
        else
        {
            // Je�li u�ytkownik nie akceptuje zaproszenia, usu� zaproszenie z listy zaprosze� u�ytkownika
            if (!(await ServerAPI.Instance.DeleteUserFriendInvitesDatabase(friendId)))
            {
                Debug.LogWarning("Failed to delete friend invite from user's list");
                return false;
            }

            // i usu� wys�ane zaproszenie do znajomych z listy zaprosze� znajomego
            if (!(await ServerAPI.Instance.DeleteFriendRequestDatabase(friendId)))
            {
                Debug.LogWarning("Failed to delete friend request from friend's list");
                return false;
            }
        }

        return true;
    }

    public async Task<bool> AcceptChallenge(string friendId, ChallengeData challengeResponse)
    {
        // 1. Wy�lij odpowied� na wyzwanie do bazy danych
        if (!(await ServerAPI.Instance.SendChallangeDatabase(friendId, challengeResponse)))
        {
            Debug.LogWarning("Failed to send challenge response");
            return false;
        }

        // 2. Usu� otrzymane wyzwanie z listy wyzwa� u�ytkownika
        if (!(await ServerAPI.Instance.DeleteChallangeDatabase(challengeResponse)))
        {
            Debug.LogWarning("Failed to delete received challenge");
            return false;
        }

        return true;
    }

    public async Task<Tuple<ServerSearchError, UserData?>> GetUserByNickname(string nickname)
    {
        return await ServerAPI.Instance.GetUserDataByNickname(nickname);
    }

    public async Task<Tuple<ServerSearchError, UserData?>> GetUserByEmail(string email)
    {
        return await ServerAPI.Instance.GetUserDataByEmail(email);
    }

    public async Task<Tuple<ServerSearchError, UserData?>> GetUserByID(string id)
    {
        return await ServerAPI.Instance.GetUserDataByID(id);
    }

    public Tuple<ServerSearchError, UserData?> GetLoggedUser()
    {
        return ServerAPI.Instance.GetLoggedUserData();
    }

    public async Task<List<int>> GetMinigamesIDs()
    {
        return await ServerAPI.Instance.GetMinigamesIDs();
    }
}