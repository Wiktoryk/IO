using Codice.Client.BaseCommands;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using UnityEngine;

public class ServerAPI : MonoBehaviour
{
    private static bool firebaseInizialized = false;
    private static DependencyStatus dependencyStatus;
    private static FirebaseAuth auth;
    private static FirebaseDatabase database;

    private static ServerAPI instance = null;
    public static ServerAPI Instance {

        get
        {
            if (instance == null)
            {
                InitFirebase();
                instance = new ServerAPI();
            }
            return instance;
        }

        private set
        {
            if (instance != value)
            {
                instance = value;
            }
        }
    }

    public UserData? LoggedUser = null;
    private FirebaseUser user;

    private static void InitFirebase()
    {
        if (firebaseInizialized)
        {
            return;
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                Debug.Log("Setting up Firebase Auth");

                auth = FirebaseAuth.DefaultInstance;

                Debug.Log("Setting up Firebase Database");

                database = FirebaseDatabase.DefaultInstance;

                firebaseInizialized = true;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public bool Login(string email, string password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        while (!LoginTask.IsCompleted)
        {
        }
        //yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account dose not exist";
                    break;
                default:
                    message = $"Login Failed! Code {(int)errorCode}";
                    break;
            }
            Debug.LogWarning(message);
            return false;
        }

        user = LoginTask.Result.User;
        Debug.LogFormat("User signed in successfully: {0} ({1})", user.DisplayName, user.Email);
        Debug.Log("Logged In");
        LoggedUser = new UserData
        {
            ID = 0,
            Nickname = user.DisplayName,
            Friends = new List<int>(),
            Highscores = new List<float>(),
            FriendRequests = new List<int>(),
            ChallengeData = new List<ChallengeData>()
        };
        return true;
    }

    public bool Register(string email, string password, string nickname)
    {
        var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);

        while (!RegisterTask.IsCompleted)
        {
        }
        //yield return new WaitUntil(() => RegisterTask.IsCompleted);

        if (RegisterTask.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {RegisterTask.Exception}");
            FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    break;
                default:
                    message = $"Register Failde! Error {(int)errorCode}";
                    break;

            }
            Debug.LogWarning(message);
            return false;
        }

        user = RegisterTask.Result.User;

        if (user != null)
        {
            UserProfile profile = new() { DisplayName = user.DisplayName };

            var ProfileTask = user.UpdateUserProfileAsync(profile);
            
            while (!ProfileTask.IsCompleted) { }
            //yield return new WaitUntil(() => ProfileTask.IsCompleted);

            if (ProfileTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
                FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                Debug.LogWarning("Username Set Failed!");
                return false;
            }
            else
            {
                // Uda³o siê wszystko
                LoggedUser = new UserData
                {
                    ID = 0,
                    Nickname = user.DisplayName,
                    Friends = new List<int>(),
                    Highscores = new List<float>(),
                    FriendRequests = new List<int>(),
                    ChallengeData = new List<ChallengeData>()
                };
                return true;
            }
        }
        return false;
    }

    public bool UpdateUserData(UserData userData)
    {
        if (LoggedUser == null)
        {
            return false;
        }
        return true;
    }

    public UserData GetLoggedUserData()
    {
        return LoggedUser.Value;
    }

    public List<int> GetMinigamesIDs()
    {
        return new List<int>();
    }

    public bool Logout()
    {
        return false;
    }

    public UserData GetUserDataByNickname(string nickname)
    {
        return LoggedUser.Value;
    }

    public UserData GetUserDataByEmail(string email)
    {
        return LoggedUser.Value;
    }

    public UserData GetUserDataByID(int id)
    {
        return LoggedUser.Value;
    }
}
