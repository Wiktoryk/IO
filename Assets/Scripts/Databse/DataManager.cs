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
    private IServerAPI serverAPI;

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

    private IEnumerator Login(string _email, string _password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to login with {loginTask.Exception}");
            // Handle login errors here
        }
        else
        {
            user = loginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
            Debug.Log("Logged In");

            // After successful login, fetch necessary data or perform actions
            FetchDataFromServer();
        }
    }

    private IEnumerator Register(string _email, string _password, string _verifyPassword, string _username)
    {
        // Your registration logic here
        // Similar to your existing Register method

        yield return null;
    }

    private void FetchDataFromServer()
    {
        // Implement fetching data from the server here
        // Use Server API to get user-related data, invitations, etc.
        // Example: StartCoroutine(GetUserData());
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
    bool login(string email, string password)
    {
        return ServerAPI.login(email, password);
    }
    bool register(string email, string nickname, string password)
    {
        return ServerAPI.register(email, nickname, password);
    }
    public List<int> fetchMiniGamesList()
    {  
        return new List<int>();
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
    public bool changePassword(string password)
    {
        return false;
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
    public bool respondFriendRequest(bool accepted, string nickname)
    {
        return false;
    }
    public bool respondChallenge(string miniGameName)
    {
        return false;
    }
}