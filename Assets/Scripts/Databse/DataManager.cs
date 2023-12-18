using UnityEngine;
using Firebase;
using Firebase.Auth;
using System.Collections;
using System.Diagnostics;

public class DataManager : MonoBehaviour, IDataManager
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    private IDataManager dataManager;
    private ServerAPI.ServerAPI serverAPI;

    private bool firebaseInitialized = false;
    private bool loggedIn = false;
    public DependencyStatus dependencyStatus;

    private void Awake()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        dataManager = FindObjectOfType<DataManager>();
        serverAPI = FindObjectOfType<ServerAPI>();
    }

    private void Update()
    {
        if (firebaseInitialized && !loggedIn)
        {
            loggedIn = true;
            LoginButton();
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        auth = FirebaseAuth.DefaultInstance;

        firebaseInitialized = true;
    }

    public void LoginButton()
    {
        StartCoroutine(Login("test@test.com", "123456"));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register("cos@email.com", "password", "password", "nickname"));
    }

    public static ServerLogInError Login(string email, string password)
    {
        return ServerAPI.Login(email, password);
    }

    public static ServerRegisterError Register(string email, string password, string nickname)
    {
        return ServerAPI.Register(email, password, nickname);
    }

    public static bool Logout()
    {
        return ServerAPI.Logout();
    }

    public ServerUserUpdateError updateUser(UserData newUserData)
    {
        var result = ServerAPI.GetLoggedUserData();

        if (result.Item1 != ServerSearchError.None)
        {
            return ServerUserUpdateError.UserNotLoggedIn;
        }

        UserData currentUserData = result.Item2.Value;

        // Here you can update the fields of currentUserData with the values from newUserData
        // For example:
        // currentUserData.Nickname = newUserData.Nickname;

        return ServerAPI.UpdateUserData(currentUserData);
    }

    public static List<int> fetchMiniGamesList()
    {
        return ServerAPI.GetMinigamesIDs();
    }

    public ServerUserUpdateError changeNickname(string newNickname)
    {
        if (ServerAPI.LoggedUser == null)
        {
            return ServerUserUpdateError.UserNotLoggedIn;
        }

        UserData userData = ServerAPI.LoggedUser.Value;
        userData.Nickname = newNickname;

        return ServerAPI.UpdateUserData(userData);
    }

    public async Task<AuthError?> changePassword(string newPassword)
    {
        if (auth.CurrentUser == null)
        {
            return AuthError.UserNotFound;
        }

        var result = await auth.CurrentUser.UpdatePasswordAsync(newPassword).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                return (AuthError)task.Exception.InnerException.ErrorCode;
            }

            return null;
        });

        return result;
    }

    public static bool SendFriendRequest(string friendId)
    {
        // Wysy³amy zaproszenie do znajomych do bazy danych
        bool sendRequestResult = SendFriendRequestDatabase(friendId, true);
        if (!sendRequestResult)
        {
            Debug.LogWarning("Failed to send friend request");
            return false;
        }

        // Dodajemy zaproszenie do listy zaproszeñ u¿ytkownika
        bool addUserFriendInvitesResult = AddUserFriendInvitesDatabase(friendId);
        if (!addUserFriendInvitesResult)
        {
            Debug.LogWarning("Failed to add friend invite to user's list");
            return false;
        }

        return true;
    }

    public static bool CancelFriendRequest(string friendId)
    {
        // Usuwamy zaproszenie z listy zaproszeñ u¿ytkownika
        bool deleteUserFriendInvitesResult = DeleteUserFriendInvitesDatabase(friendId);
        if (!deleteUserFriendInvitesResult)
        {
            Debug.LogWarning("Failed to delete friend invite from user's list");
            return false;
        }

        // Usuwamy zaproszenie z bazy danych
        bool deleteFriendRequestResult = DeleteFriendRequestDatabase(friendId);
        if (!deleteFriendRequestResult)
        {
            Debug.LogWarning("Failed to delete friend request from database");
            return false;
        }

        return true;
    }

    public static bool SaveScore(int minigameId, float score)
    {
        // Pobieramy aktualny najlepszy wynik u¿ytkownika
        float currentHighscore = 0;
        if (minigameId < LoggedUser.Value.Highscores.Count)
        {
            currentHighscore = LoggedUser.Value.Highscores[minigameId];
        }

        // Jeœli nowy wynik jest wy¿szy ni¿ aktualny najlepszy wynik, aktualizujemy go
        if (score > currentHighscore)
        {
            return UpdateUserHighscoreDatabase(minigameId, score);
        }

        // Jeœli nowy wynik nie jest wy¿szy, nie robimy nic
        return true;
    }

    public async Task<(ServerSearchError, UserData?)> fetchUserData()
    {
        var result = ServerAPI.GetLoggedUserDatabase();

        if (result.Item1 != ServerSearchError.None)
        {
            Debug.LogWarning($"Failed to fetch user data with {result.Item1}");
        }

        return result;
    }

    public bool sendChallenge(string friendId, ChallengeData challenge)
    {
        return ServerAPI.SendChallangeDatabase(friendId, challenge);
    }

    public bool cancelChallenge(ChallengeData challenge)
    {
        return ServerAPI.DeleteChallangeDatabase(challenge);
    }

    public static bool RespondFriendRequest(string friendId, bool accept)
    {
        if (accept)
        {
            // 1. Usuñ zaproszenie do znajomych z listy zaproszeñ u¿ytkownika
            if (!DeleteUserFriendInvitesDatabase(friendId))
            {
                Debug.LogWarning("Failed to delete friend invite from user's list");
                return false;
            }

            // 2. Dodaj nowego znajomego do listy znajomych u¿ytkownika
            if (!UpdateUserFriendsListDatabase(friendId))
            {
                Debug.LogWarning("Failed to add friend to user's list");
                return false;
            }

            // 3. Usuñ wys³ane zaproszenie do znajomych z listy zaproszeñ znajomego
            if (!DeleteFriendRequestDatabase(friendId))
            {
                Debug.LogWarning("Failed to delete friend request from friend's list");
                return false;
            }

            // 4. Dodaj u¿ytkownika do listy znajomych znajomego
            // W tym celu musisz mieæ dostêp do metody podobnej do UpdateUserFriendsListDatabase(friendId), ale dla innego u¿ytkownika.
            // Przyk³ad: if (!UpdateFriendFriendsListDatabase(friendId)) { ... }
        }
        else
        {
            // Jeœli u¿ytkownik nie akceptuje zaproszenia, usuñ zaproszenie z listy zaproszeñ u¿ytkownika
            if (!DeleteUserFriendInvitesDatabase(friendId))
            {
                Debug.LogWarning("Failed to delete friend invite from user's list");
                return false;
            }

            // i usuñ wys³ane zaproszenie do znajomych z listy zaproszeñ znajomego
            if (!DeleteFriendRequestDatabase(friendId))
            {
                Debug.LogWarning("Failed to delete friend request from friend's list");
                return false;
            }
        }

        return true;
    }

    public static bool AcceptChallenge(string friendId, ChallengeData challengeResponse)
    {
        // 1. Wyœlij odpowiedŸ na wyzwanie do bazy danych
        if (!SendChallangeDatabase(friendId, challengeResponse))
        {
            Debug.LogWarning("Failed to send challenge response");
            return false;
        }

        // 2. Usuñ otrzymane wyzwanie z listy wyzwañ u¿ytkownika
        if (!DeleteChallangeDatabase(challengeResponse))
        {
            Debug.LogWarning("Failed to delete received challenge");
            return false;
        }

        return true;
    }

    public static (ServerSearchError, UserData?) GetUserByNickname(string nickname)
    {
        return ServerAPI.GetUserDataByNickname(nickname);
    }

    public static (ServerSearchError, UserData?) GetUserByEmail(string email)
    {
        return ServerAPI.GetUserDataByEmail(email);
    }

    public static (ServerSearchError, UserData?) GetUserID(string id)
    {
        return ServerAPI.GetUserDataByID(id);
    }
}