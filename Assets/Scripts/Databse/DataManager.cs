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

    public void UpdateUserData(string userData)
    {
        // Implement logic to update user data in the Main Lobby
    }

    public void SendDataToServer(string data)
    {
        if (serverAPI != null)
        {
            serverAPI.SendDataToServer(data);
        }
        else
        {
            Debug.LogError("ServerAPI reference not found!");
        }
    }
}