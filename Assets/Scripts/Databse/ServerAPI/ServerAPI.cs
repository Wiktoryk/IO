using Firebase;
using Firebase.Auth;
using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ServerAPI : MonoBehaviour
{
    private static bool firebaseInizialized = false;
    private static DependencyStatus dependencyStatus;
    private static FirebaseAuth auth;
    private static DatabaseReference dbReference;

    private static FirebaseUser loggedUser = null;
    private static UserData? LoggedUser = null;

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

                dbReference = FirebaseDatabase.DefaultInstance.RootReference;

                firebaseInizialized = true;
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    public static ServerLogInError Login(string email, string password)
    {
        InitFirebase();

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
            ServerLogInError error = ServerLogInError.Other;
            switch (errorCode)
            {
                case AuthError.None:
                    message = "None";
                    error = ServerLogInError.None;
                    break;
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    error = ServerLogInError.MissingEmail;
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    error = ServerLogInError.MissingPassword;
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    error = ServerLogInError.WrongPassword;
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    error = ServerLogInError.WrongEmail;
                    break;
                case AuthError.UserNotFound:
                    message = "Account dose not exist";
                    error = ServerLogInError.UserNotFound;
                    break;
                default:
                    message = $"Login Failed! Code {(int)errorCode}";
                    error = ServerLogInError.Other;
                    break;
            }
            Debug.LogWarning(message);
            return error;
        }

        loggedUser = LoginTask.Result.User;
        LoggedUser = new UserData(loggedUser, new List<string>(), new List<float>(), new Dictionary<string, bool>(), new List<string>(), new List<ChallengeData>());
        (ServerSearchError, UserData?) user = GetLoggedUserDatabase();
        if (user.Item1 == ServerSearchError.None)
        {
            LoggedUser = user.Item2;
        }
        Debug.LogFormat("User signed in successfully: {0} ({1})", LoggedUser.Value.Nickname, LoggedUser.Value.Email);
        Debug.Log("Logged In");
        return ServerLogInError.None;
    }

    public static ServerRegisterError Register(string email, string password, string nickname)
    {
        InitFirebase();

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
            ServerRegisterError error = ServerRegisterError.Other;
            switch (errorCode)
            {
                case AuthError.None:
                    message = "None";
                    error = ServerRegisterError.Other;
                    break;
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    error = ServerRegisterError.MissingEmail;
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    error = ServerRegisterError.MissingPassword;
                    break;
                case AuthError.WeakPassword:
                    message = "Weak Password";
                    error = ServerRegisterError.WeakPassword;
                    break;
                case AuthError.EmailAlreadyInUse:
                    message = "Email Already In Use";
                    error = ServerRegisterError.EmailAlreadyInUse;
                    break;
                default:
                    message = $"Register Failde! Error {(int)errorCode}";
                    error = ServerRegisterError.Other;
                    break;

            }
            Debug.LogWarning(message);
            return error;
        }

        loggedUser = RegisterTask.Result.User;
        if (loggedUser == null)
        {
            LoggedUser = null;
            return ServerRegisterError.Other;
        }

        LoggedUser = new UserData(loggedUser, new List<string>(), new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, new Dictionary<string, bool>(), new List<string>(), new List<ChallengeData>());

        if (!UpdateUserNicknameAuth(nickname))
        {
            return ServerRegisterError.NicknameSetupFailed;
        }

        if (!RegisterUserDatabase())
        {
            return ServerRegisterError.DatabaseUserRegisterFailed;
        }

        return ServerRegisterError.None;
    }

    public static ServerUserUpdateError UpdateUserData(UserData userData)
    {
        // U¿ytkownik nie zalogowany
        if (LoggedUser == null)
        {
            return ServerUserUpdateError.UserNotLoggedIn;
        }

        // Niektóre dane mog¹ byæ aktualizowane jeœli nale¿¹ do zalogowanego u¿ytkownika
        if (userData.ID == LoggedUser.Value.ID)
        {
            // Update Nickname
            if (userData.Nickname != LoggedUser.Value.Nickname)
            {
                if (!UpdateUserNicknameAuth(userData.Nickname))
                {
                    if (!UpdateUserNicknameDatabase(userData.Nickname))
                    {
                        return ServerUserUpdateError.NicknameUpdateFailed;
                    }
                }
            }
        }

        return ServerUserUpdateError.None;
    }

    private static bool UpdateUserNicknameAuth(string nickname)
    {
        UserProfile profile = new() { DisplayName = nickname };
        var ProfileTask = loggedUser.UpdateUserProfileAsync(profile);

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

        // Uda³o siê wszystko
        UserData user = LoggedUser.Value;
        user.Nickname = nickname;
        LoggedUser = user;
        return true;
    }

    private static bool RegisterUserDatabase()
    {
        if (LoggedUser == null)
        {
            return false;
        }

        UserData user = LoggedUser.Value;

        if (!UpdateUserNicknameDatabase(user.Nickname))
        {
            return false;
        }

        if (!UpdateUserEmailDatabase(user.Email))
        {
            return false;
        }

        int i = 0;
        foreach (float score in user.Highscores)
        {
            if (!UpdateUserHighscoreDatabase(i, score))
            {
                return false;
            }
            i++;
        }

        return true;
    }

    private static bool UpdateUserNicknameDatabase(string nickname)
    {
        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).Child("nickname").SetValueAsync(nickname);

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            UserData user = LoggedUser.Value;
            user.Nickname = nickname;
            LoggedUser = user;
            return true;
        }
    }

    private static bool UpdateUserEmailDatabase(string email)
    {
        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).Child("email").SetValueAsync(email);

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            UserData user = LoggedUser.Value;
            user.Email = email;
            LoggedUser = user;
            return true;
        }
    }

    private static bool UpdateUserHighscoreDatabase(int minigameId, float score)
    {
        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).Child("highscores").Child(minigameId.ToString()).SetValueAsync(score);

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            UserData user = LoggedUser.Value;

            while (minigameId > user.Highscores.Count)
            {
                user.Highscores.Add(0.0f);
            }

            if (minigameId == user.Highscores.Count)
            {
                user.Highscores.Add(score);
            }
            else
            {
                user.Highscores[minigameId] = score;
            }

            LoggedUser = user;

            return true;
        }
    }

    private static bool UpdateUserFriendsListDatabase(string friendId)
    {
        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).Child("friends").Child(friendId).SetValueAsync(true);

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            UserData user = LoggedUser.Value;
            user.Friends.Add(friendId);
            LoggedUser = user;
            return true;
        }
    }

    private static bool AddUserFriendInvitesDatabase(string friendId)
    {
        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).Child("friendInvites").Child(friendId).SetValueAsync(true);

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            UserData user = LoggedUser.Value;
            user.FriendInvites.Add(friendId);
            LoggedUser = user;
            return true;
        }
    }

    private static bool DeleteUserFriendInvitesDatabase(string friendId)
    {
        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).Child("friendInvites").Child(friendId).RemoveValueAsync();

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            UserData user = LoggedUser.Value;
            user.FriendInvites.Remove(friendId);
            LoggedUser = user;
            return true;
        }
    }

    private static bool SendFriendRequestDatabase(string friendId, bool accept)
    {
        var DBTask = dbReference.Child("users").Child(friendId).Child("friendRequests").Child(loggedUser.UserId).SetValueAsync(accept);

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            return true;
        }
    }

    private static bool DeleteFriendRequestDatabase(string friendId)
    {
        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).Child("friendRequests").Child(friendId).RemoveValueAsync();

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            UserData user = LoggedUser.Value;
            user.FriendRequests.Remove(friendId);
            LoggedUser = user;
            return true;
        }
    }

    private static bool SendChallangeDatabase(string friendId, ChallengeData challenge)
    {
        var DBTask = dbReference.Child("users").Child(friendId).Child("challanges").Child(loggedUser.UserId + "_" + challenge.MinigameID.ToString()).Child("userId").SetValueAsync(challenge.UserID);

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            DBTask = dbReference.Child("users").Child(friendId).Child("challanges").Child(loggedUser.UserId + "_" + challenge.MinigameID.ToString()).Child("minigameId").SetValueAsync(challenge.MinigameID);

            while (!DBTask.IsCompleted) { }
            //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                return false;
            }
            else
            {
                DBTask = dbReference.Child("users").Child(friendId).Child("challanges").Child(loggedUser.UserId + "_" + challenge.MinigameID.ToString()).Child("score").SetValueAsync(challenge.Score);

                while (!DBTask.IsCompleted) { }
                //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

                if (DBTask.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                    return false;
                }
            }

            return true;
        }
    }

    private static bool DeleteChallangeDatabase(ChallengeData challenge)
    {
        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).Child("challanges").Child(challenge.UserID + "_" + challenge.MinigameID.ToString()).RemoveValueAsync();

        while (!DBTask.IsCompleted) { }
        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
            return false;
        }
        else
        {
            UserData user = LoggedUser.Value;
            user.ChallengeData.Remove(challenge);
            LoggedUser = user;

            return true;
        }
    }

    public static (ServerSearchError, UserData?) GetLoggedUserData()
    {
        if (LoggedUser == null)
        {
            return (ServerSearchError.UserNotLogged, null);
        }
        return (ServerSearchError.None, LoggedUser.Value);
    }

    public static List<int> GetMinigamesIDs()
    {
        return new List<int>();
    }

    public static bool Logout()
    {
        if (LoggedUser == null)
        {
            return false;
        }

        auth.SignOut();
        loggedUser = null;
        LoggedUser = null;
        return true;
    }

    public static (ServerSearchError, UserData?) GetLoggedUserDatabase()
    {
        if (LoggedUser == null)
        {
            return (ServerSearchError.UserNotLogged, null);
        }

        var DBTask = dbReference.Child("users").Child(loggedUser.UserId).GetValueAsync();

        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        while (!DBTask.IsCompleted) { }

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

            return (ServerSearchError.Other, null);
        }
        else if (DBTask.Result.Value == null)
        {
            UserData user = LoggedUser.Value;
            user.ChallengeData = new();
            user.FriendRequests = new();
            user.Friends = new();
            user.Highscores = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

            return (ServerSearchError.None, user);
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            UserData user = LoggedUser.Value;

            List<ChallengeData> challengesData = new();

            DataSnapshot challanges = snapshot.Child("challanges");

            foreach (DataSnapshot challange in challanges.Children)
            {
                ChallengeData challangeData;
                challangeData.MinigameID = int.Parse(challange.Child("minigameId").Value.ToString());
                challangeData.Score = float.Parse(challange.Child("score").Value.ToString());
                challangeData.UserID = challange.Child("userId").Value.ToString();

                challengesData.Add(challangeData);
            }

            user.ChallengeData = challengesData;

            Dictionary<string, bool> friendsReqData = new();

            DataSnapshot friendsReq = snapshot.Child("friendRequests");

            foreach (DataSnapshot req in friendsReq.Children)
            {
                friendsReqData.Add(req.Key.ToString(), bool.Parse(req.Value.ToString()));
            }

            user.FriendRequests = friendsReqData;

            List<string> friendsInvData = new();

            DataSnapshot friendsInv = snapshot.Child("friendInvites");

            foreach (DataSnapshot inv in friendsInv.Children)
            {
                friendsInvData.Add(inv.Key.ToString());
            }

            user.FriendInvites = friendsInvData;

            List<string> friendsData = new();

            DataSnapshot friends = snapshot.Child("friends");

            foreach (DataSnapshot friend in friends.Children)
            {
                friendsData.Add(friend.Key.ToString());
            }

            user.Friends = friendsData;

            List<float> highscoresData = new();

            DataSnapshot highscores = snapshot.Child("highscores");

            foreach (DataSnapshot highscore in highscores.Children)
            {
                highscoresData.Add(float.Parse(highscore.Value.ToString()));
            }

            user.Highscores = highscoresData;

            return (ServerSearchError.None, user);
        }
    }

    public static (ServerSearchError, UserData?) GetUserDataByNickname(string nickname)
    {
        if (LoggedUser == null)
        {
            return (ServerSearchError.UserNotLogged, null);
        }

        var DBTask = dbReference.Child("users").OrderByChild("nickname").GetValueAsync();

        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        while (!DBTask.IsCompleted) { }

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

            return (ServerSearchError.Other, null);
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            UserData user = new();

            bool found = false;

            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse())
            {
                if (childSnapshot.Child("nickname").Value.ToString().Equals(nickname))
                {
                    user.ID = childSnapshot.Key;
                    user.Email = childSnapshot.Child("email").Value.ToString();
                    user.Nickname = childSnapshot.Child("nickname").Value.ToString();

                    List<ChallengeData> challengesData = new(); 

                    DataSnapshot challanges = childSnapshot.Child("challanges");

                    foreach (DataSnapshot challange in challanges.Children)
                    {
                        ChallengeData challangeData;
                        challangeData.MinigameID = int.Parse(challange.Child("minigameId").Value.ToString());
                        challangeData.Score = float.Parse(challange.Child("score").Value.ToString());
                        challangeData.UserID = challange.Child("userId").Value.ToString();

                        challengesData.Add(challangeData);
                    }

                    user.ChallengeData = challengesData;

                    Dictionary<string, bool> friendsReqData = new();

                    DataSnapshot friendsReq = snapshot.Child("friendRequests");

                    foreach (DataSnapshot req in friendsReq.Children)
                    {
                        friendsReqData.Add(req.Key.ToString(), bool.Parse(req.Value.ToString()));
                    }

                    user.FriendRequests = friendsReqData;

                    List<string> friendsInvData = new();

                    DataSnapshot friendsInv = snapshot.Child("friendInvites");

                    foreach (DataSnapshot inv in friendsInv.Children)
                    {
                        friendsInvData.Add(inv.Key.ToString());
                    }

                    user.FriendInvites = friendsInvData;

                    List<string> friendsData = new();

                    DataSnapshot friends = childSnapshot.Child("friends");

                    foreach (DataSnapshot friend in friends.Children)
                    {
                        friendsData.Add(friend.Key.ToString());
                    }

                    user.Friends = friendsData;

                    List<float> highscoresData = new();

                    DataSnapshot highscores = childSnapshot.Child("highscores");

                    foreach (DataSnapshot highscore in highscores.Children)
                    {
                        highscoresData.Add(float.Parse(highscore.Value.ToString()));
                    }

                    user.Highscores = highscoresData;

                    found = true;
                    break;
                }
            }

            if (found)
            {
                return (ServerSearchError.None, user);
            }
            else
            {
                return (ServerSearchError.NoUserFound, null);
            }
        }
    }

    public static (ServerSearchError, UserData?) GetUserDataByEmail(string email)
    {
        if (LoggedUser == null)
        {
            return (ServerSearchError.UserNotLogged, null);
        }

        var DBTask = dbReference.Child("users").OrderByChild("email").GetValueAsync();

        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        while (!DBTask.IsCompleted) { }

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

            return (ServerSearchError.Other, null);
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            UserData user = new();

            bool found = false;

            foreach (DataSnapshot childSnapshot in snapshot.Children.Reverse())
            {
                if (childSnapshot.Child("email").Value.ToString().Equals(email))
                {
                    user.ID = childSnapshot.Key;
                    user.Email = childSnapshot.Child("email").Value.ToString();
                    user.Nickname = childSnapshot.Child("nickname").Value.ToString();

                    List<ChallengeData> challengesData = new();

                    DataSnapshot challanges = childSnapshot.Child("challanges");

                    foreach (DataSnapshot challange in challanges.Children)
                    {
                        ChallengeData challangeData;
                        challangeData.MinigameID = int.Parse(challange.Child("minigameId").Value.ToString());
                        challangeData.Score = float.Parse(challange.Child("score").Value.ToString());
                        challangeData.UserID = challange.Child("userId").Value.ToString();

                        challengesData.Add(challangeData);
                    }

                    user.ChallengeData = challengesData;

                    Dictionary<string, bool> friendsReqData = new();

                    DataSnapshot friendsReq = snapshot.Child("friendRequests");

                    foreach (DataSnapshot req in friendsReq.Children)
                    {
                        friendsReqData.Add(req.Key.ToString(), bool.Parse(req.Value.ToString()));
                    }

                    user.FriendRequests = friendsReqData;

                    List<string> friendsInvData = new();

                    DataSnapshot friendsInv = snapshot.Child("friendInvites");

                    foreach (DataSnapshot inv in friendsInv.Children)
                    {
                        friendsInvData.Add(inv.Key.ToString());
                    }

                    user.FriendInvites = friendsInvData;

                    List<string> friendsData = new();

                    DataSnapshot friends = childSnapshot.Child("friends");

                    foreach (DataSnapshot friend in friends.Children)
                    {
                        friendsData.Add(friend.Key.ToString());
                    }

                    user.Friends = friendsData;

                    List<float> highscoresData = new();

                    DataSnapshot highscores = childSnapshot.Child("highscores");

                    foreach (DataSnapshot highscore in highscores.Children)
                    {
                        highscoresData.Add(float.Parse(highscore.Value.ToString()));
                    }

                    user.Highscores = highscoresData;

                    found = true;
                    break;
                }
            }

            if (found)
            {
                return (ServerSearchError.None, user);
            }
            else
            {
                return (ServerSearchError.NoUserFound, null);
            }
        }
    }

    public static (ServerSearchError, UserData?) GetUserDataByID(string id)
    {
        if (LoggedUser == null)
        {
            return (ServerSearchError.UserNotLogged, null);
        }

        var DBTask = dbReference.Child("users").Child(id).GetValueAsync();

        //yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
        while (!DBTask.IsCompleted) { }

        if (DBTask.Exception != null)
        {
            Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

            return (ServerSearchError.Other, null);
        }
        else if (DBTask.Result.Value == null)
        {
            return (ServerSearchError.NoUserFound, null);
        }
        else
        {
            DataSnapshot snapshot = DBTask.Result;

            UserData user;

            user.ID = snapshot.Key.ToString();
            user.Nickname = snapshot.Child("nickname").Value.ToString();
            user.Email = snapshot.Child("email").Value.ToString();

            List<ChallengeData> challengesData = new();

            DataSnapshot challanges = snapshot.Child("challanges");

            foreach (DataSnapshot challange in challanges.Children)
            {
                ChallengeData challangeData;
                challangeData.MinigameID = int.Parse(challange.Child("minigameId").Value.ToString());
                challangeData.Score = float.Parse(challange.Child("score").Value.ToString());
                challangeData.UserID = challange.Child("userId").Value.ToString();

                challengesData.Add(challangeData);
            }

            user.ChallengeData = challengesData;

            Dictionary<string, bool> friendsReqData = new();

            DataSnapshot friendsReq = snapshot.Child("friendRequests");

            foreach (DataSnapshot req in friendsReq.Children)
            {
                friendsReqData.Add(req.Key.ToString(), bool.Parse(req.Value.ToString()));
            }

            user.FriendRequests = friendsReqData;

            List<string> friendsInvData = new();

            DataSnapshot friendsInv = snapshot.Child("friendInvites");

            foreach (DataSnapshot inv in friendsInv.Children)
            {
                friendsInvData.Add(inv.Key.ToString());
            }

            user.FriendInvites = friendsInvData;

            List<string> friendsData = new();

            DataSnapshot friends = snapshot.Child("friends");

            foreach (DataSnapshot friend in friends.Children)
            {
                friendsData.Add(friend.Key.ToString());
            }

            user.Friends = friendsData;

            List<float> highscoresData = new();

            DataSnapshot highscores = snapshot.Child("highscores");

            foreach (DataSnapshot highscore in highscores.Children)
            {
                highscoresData.Add(float.Parse(highscore.Value.ToString()));
            }

            user.Highscores = highscoresData;

            return (ServerSearchError.None, user);
        }
    }
}