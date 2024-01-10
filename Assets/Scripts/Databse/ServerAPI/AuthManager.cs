using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using UnityEngine.UIElements;

public class AuthManager : MonoBehaviour
{
    [Header("Firebase")]
    private bool firebaseInizialized = false;
    private bool login = false;
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

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
    }

    private void Update()
    {
        if (firebaseInizialized && !login)
        {
            login = true;
            LoginButton();
        }
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");

        auth = FirebaseAuth.DefaultInstance;

        firebaseInizialized = true;
    }

    public void LoginButton()
    {
        StartCoroutine(Login("test@test.com", "123456"));
        //StartCoroutine(Login("cos@email.com", "password"));
    }

    public void RegisterButton()
    {
        StartCoroutine(Register("cos@email.com", "password", "password", "nickname"));
    }

    private IEnumerator Login(string _email, string _password)
    {
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);

        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

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
        }
        else
        {
            User = LoginTask.Result.User;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            Debug.Log("Logged In");
        }
    }

    private IEnumerator Register(string _email, string _password, string _verifyPassword, string _username)
    {
        if (_username == "")
        {
            Debug.LogWarning("Missing Username!");
        }
        else if (_password != _verifyPassword)
        {
            Debug.LogWarning("Passwords Dose Not Match!");
        }
        else
        {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);

            yield return new WaitUntil(() => RegisterTask.IsCompleted);

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
            }
            else
            {
                User = RegisterTask.Result.User;

                if (User != null)
                {
                    UserProfile profile = new() { DisplayName = User.DisplayName };

                    var ProfileTask = User.UpdateUserProfileAsync(profile);

                    yield return new WaitUntil(() => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        Debug.LogWarning("Username Set Failed!");
                    }
                    else
                    {
                        // Uda�o si� wszystko
                    }
                }
            }
        }
    }
}
