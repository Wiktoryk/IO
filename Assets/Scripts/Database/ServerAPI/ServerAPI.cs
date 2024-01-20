using System;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using UnityEngine;

using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class ServerAPI
{
    private bool firebaseInizialized = false;
    public bool IsReady => firebaseInizialized;
    private DependencyStatus dependencyStatus;
    private FirebaseAuth auth;
    private DatabaseReference dbReference;

    private FirebaseUser firebaseLoggedUser = null;
    private UserData? LoggedUser = null;

    private static ServerAPI _instance = null;
    public static ServerAPI Instance { 
        get
        {
            _instance ??= new ServerAPI();
            return _instance;
        }
        private set
        {
            if (_instance != value)
            {
                _instance = value;
            }
        }
    }

    public ServerAPI() {}

    public async Task Init()
    {
        if (firebaseInizialized)
        {
            return;
        }

        var initTask = FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + task.Exception);
                return;
            }

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

        await initTask;
    }

    public async Task<ServerLogInError> Login(string email, string password)
    {
        return await Task.Run(async () =>
        {
            if (auth == null || dbReference == null)
            {
                Debug.Log("Not Inizialized");
                return ServerLogInError.Other;
            }

            if (LoggedUser != null)
            {
                Logout();
            }

            ServerLogInError error = ServerLogInError.None;

            var LoginTask = auth.SignInWithEmailAndPasswordAsync(email, password);
            await LoginTask;

            if (LoginTask.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return error;
            }
            if (LoginTask.IsFaulted)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync encountered an error: " + LoginTask.Exception);
                return error;
            }

            if (LoginTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {LoginTask.Exception}");
                FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "";
                error = ServerLogInError.Other;
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

            firebaseLoggedUser = LoginTask.Result.User;
            LoggedUser = new UserData(firebaseLoggedUser, new List<string>(), new List<float>(), new Dictionary<string, bool>(), new List<string>(), new List<ChallengeData>());
            Tuple<ServerSearchError, UserData?> user = await GetLoggedUserDatabase();
            if (user.Item1 == ServerSearchError.None)
            {
                LoggedUser = user.Item2;
            }
            Debug.LogFormat("User signed in successfully: {0} ({1})", LoggedUser.Value.Nickname, LoggedUser.Value.Email);
            Debug.Log("Logged In");

            return ServerLogInError.None;
        });
    }

    public async Task<ServerRegisterError> Register(string email, string password, string nickname)
    {
        return await Task.Run(async () => {
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
            await RegisterTask;

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

            firebaseLoggedUser = RegisterTask.Result.User;
            if (firebaseLoggedUser == null)
            {
                LoggedUser = null;
                return ServerRegisterError.Other;
            }

            LoggedUser = new UserData(firebaseLoggedUser, new List<string>(), new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f }, new Dictionary<string, bool>(), new List<string>(), new List<ChallengeData>());

            if (!await UpdateUserNicknameAuth(nickname))
            {
                return ServerRegisterError.NicknameSetupFailed;
            }

            if (!await RegisterUserDatabase())
            {
                return ServerRegisterError.DatabaseUserRegisterFailed;
            }

            return ServerRegisterError.None;
        });
    }

    public async Task<ServerUserUpdateError> UpdateUserData(UserData userData)
    {
        return await Task.Run(async () =>
        {
            // U�ytkownik nie zalogowany
            if (LoggedUser == null)
            {
                return ServerUserUpdateError.UserNotLoggedIn;
            }

            // Niekt�re dane mog� by� aktualizowane je�li nale�� do zalogowanego u�ytkownika
            if (userData.ID == LoggedUser.Value.ID)
            {
                // Update Nickname
                if (userData.Nickname != LoggedUser.Value.Nickname)
                {
                    if (!await UpdateUserNicknameAuth(userData.Nickname))
                    {
                        return ServerUserUpdateError.NicknameUpdateFailed;
                    }
                }

                // Update Score
                List<float> high = userData.Highscores.ToList();

                for (int i = 0; i < high.Count; i++)
                {
                    if (high[i] != LoggedUser.Value.Highscores[i])
                    {
                        if (!await UpdateUserHighscoreDatabase(i, high[i]))
                        {
                            return ServerUserUpdateError.HighscoresUpdateFailed;
                        }
                    }
                }

                // Update Friends List
                foreach (string friendID in userData.Friends)
                {
                    if (!LoggedUser.Value.Friends.Contains(friendID))
                    {
                        if (!await UpdateUserFriendsListDatabase(friendID))
                        {
                            return ServerUserUpdateError.FriendsListUpdateFailed;
                        }
                    }
                }

                // Update Friends Invites
                foreach (string friendID in userData.FriendInvites)
                {
                    if (!LoggedUser.Value.FriendInvites.Contains(friendID))
                    {
                        if (!await AddUserFriendInvitesDatabase(friendID))
                        {
                            return ServerUserUpdateError.FriendsInvitesAddFailed;
                        }
                    }
                }
                foreach (string friendID in LoggedUser.Value.FriendInvites)
                {
                    if (!userData.FriendInvites.Contains(friendID))
                    {
                        if (!await DeleteUserFriendInvitesDatabase(friendID))
                        {
                            return ServerUserUpdateError.FriendsInvitesRemoveFailed;
                        }
                    }
                }

                // Remove Friend Requests
                foreach (var request in LoggedUser.Value.FriendRequests)
                {
                    if (!userData.FriendRequests.Contains(request))
                    {
                        if (!await DeleteFriendRequestDatabase(request.Key))
                        {
                            return ServerUserUpdateError.FriendsRequestsRemoveFailed;
                        }
                    }
                }

                // Remove Challange
                foreach (ChallengeData challange in LoggedUser.Value.ChallengeData)
                {
                    if (!userData.ChallengeData.Contains(challange))
                    {
                        if (!await DeleteChallangeDatabase(challange))
                        {
                            return ServerUserUpdateError.ChallangesRemoveFailed;
                        }
                    }
                }
            }

            (ServerSearchError error, UserData? currUser) = await GetUserDataByID(userData.ID);
            if (error != ServerSearchError.None)
            {
                return ServerUserUpdateError.GetCurrentUserData;
            }

            // Send Friends Requests
            foreach (var request in userData.FriendRequests)
            {
                if (!currUser.Value.FriendRequests.Contains(request))
                {
                    if (!await SendFriendRequestDatabase(userData.ID, request.Value))
                    {
                        return ServerUserUpdateError.FriendsInvitesAddFailed;
                    }
                }
            }

            // Send Challanges
            foreach (ChallengeData challange in userData.ChallengeData)
            {
                if (!currUser.Value.ChallengeData.Contains(challange))
                {
                    if (!await SendChallangeDatabase(userData.ID, challange))
                    {
                        return ServerUserUpdateError.ChallangesSendFailed;
                    }
                }
            }


            if (userData.ID == LoggedUser.Value.ID)
            {
                UserData updatedUser = userData;
                updatedUser.Email = LoggedUser.Value.Email;
                LoggedUser = updatedUser;
            }
            return ServerUserUpdateError.None;
        });
    }

    public async Task<bool> UpdateUserNicknameAuth(string nickname)
    {
        return await Task.Run(async () =>
        {
            UserProfile profile = new() { DisplayName = nickname };
            var ProfileTask = firebaseLoggedUser.UpdateUserProfileAsync(profile);
            await ProfileTask;

            if (ProfileTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {ProfileTask.Exception}");
                FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                Debug.LogWarning($"Username Set Failed! Error: {errorCode}");
                return false;
            }

            // Uda�o si� wszystko
            UserData user = LoggedUser.Value;
            user.Nickname = nickname;
            LoggedUser = user;
            return await UpdateUserNicknameDatabase(nickname);
        });
    }

    public async Task<bool> UpdateUserPasswordAuth(string password) 
    {
        return await Task.Run(async () =>
        {
            var PasswordTask = auth.CurrentUser.UpdatePasswordAsync(password);
            await PasswordTask;

            if (PasswordTask.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {PasswordTask.Exception}");
                FirebaseException firebaseEx = PasswordTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                Debug.LogWarning($"Password Set Failed! Error: {errorCode}");
                return false;
            }

            // Uda�o sie wszystko
            Debug.Log("Password successfully changed.");
            return true;
        });
    }

    private async Task<bool> RegisterUserDatabase()
    {
        return await Task.Run(async () =>
        {
            if (LoggedUser == null)
            {
                return false;
            }

            UserData user = LoggedUser.Value;

            if (!await UpdateUserNicknameDatabase(user.Nickname))
            {
                return false;
            }

            if (!await UpdateUserEmailDatabase(user.Email))
            {
                return false;
            }

            // Update Score
            List<float> high = user.Highscores.ToList();

            for (int i = 0; i < high.Count; i++)
            {
                if (!await UpdateUserHighscoreDatabase(i, high[i]))
                {
                    return false;
                }
            }

            return true;
        });
    }

    private async Task<bool> UpdateUserNicknameDatabase(string nickname)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).Child("nickname").SetValueAsync(nickname);
            await DBTask;

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
        });
    }

    private async Task<bool> UpdateUserEmailDatabase(string email)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).Child("email").SetValueAsync(email);
            await DBTask;

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
        });
    }

    public async Task<bool> UpdateUserHighscoreDatabase(int minigameId, float score)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).Child("highscores").Child(minigameId.ToString()).SetValueAsync(score);
            await DBTask;

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
        });
    }

    public async Task<bool> UpdateUserFriendsListDatabase(string friendId)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).Child("friends").Child(friendId).SetValueAsync(true);
            await DBTask;

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
        });
    }

    public async Task<bool> AddUserFriendInvitesDatabase(string friendId)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).Child("friendInvites").Child(friendId).SetValueAsync(true);
            await DBTask;

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
        });
    }

    public async Task<bool> DeleteUserFriendInvitesDatabase(string friendId)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).Child("friendInvites").Child(friendId).RemoveValueAsync();
            await DBTask;

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
        });
    }

    public async Task<bool> SendFriendRequestDatabase(string friendId, bool accept)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(friendId).Child("friendRequests").Child(firebaseLoggedUser.UserId).SetValueAsync(accept);
            await DBTask;

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                return false;
            }
            else
            {
                return true;
            }
        });
    }

    public async Task<bool> DeleteFriendRequestDatabase(string friendId)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).Child("friendRequests").Child(friendId).RemoveValueAsync();
            await DBTask;

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
        });
    }

    public async Task<bool> SendChallangeDatabase(string friendId, ChallengeData challenge)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(friendId).Child("challanges").Child(firebaseLoggedUser.UserId + "_" + challenge.MinigameID.ToString()).Child("userId").SetValueAsync(challenge.UserID);
            await DBTask;

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                return false;
            }
            else
            {
                DBTask = dbReference.Child("users").Child(friendId).Child("challanges").Child(firebaseLoggedUser.UserId + "_" + challenge.MinigameID.ToString()).Child("minigameId").SetValueAsync(challenge.MinigameID);
                await DBTask;

                if (DBTask.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                    return false;
                }
                else
                {
                    DBTask = dbReference.Child("users").Child(friendId).Child("challanges").Child(firebaseLoggedUser.UserId + "_" + challenge.MinigameID.ToString()).Child("score").SetValueAsync(challenge.Score);
                    await DBTask;

                    if (DBTask.Exception != null)
                    {
                        Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                        return false;
                    }
                }

                return true;
            }
        });
    }

    public async Task<bool> DeleteChallangeDatabase(ChallengeData challenge)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).Child("challanges").Child(challenge.UserID + "_" + challenge.MinigameID.ToString()).RemoveValueAsync();
            await DBTask;

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
        });        
    }

    public Tuple<ServerSearchError, UserData?> GetLoggedUserData()
    {
        if (LoggedUser == null)
        {
            return new(ServerSearchError.UserNotLogged, null);
        }
        return new(ServerSearchError.None, LoggedUser.Value);
    }

    public async Task<List<int>> GetMinigamesIDs()
    {
        return await Task.Run(async () =>
        {
            (bool minigamesIDsGenerated, List<int> minigamesIDs) = await GetMinigamesIDsDatabase();
            if (minigamesIDsGenerated)
            {
                return minigamesIDs;
            }

            // Losowanie
            // Pozyskanie daty
            DateTime timeNow = DateTime.Now;
            int currentDay = timeNow.Year * 10000 + timeNow.Month * 100 + timeNow.Day;

            // hashowanie daty
            int seed = currentDay.GetHashCode();

            // stworzenie objektu random
            System.Random rng = new(seed);

            // stworzenie losowo pomieszaniej listy
            List<int> allNumbers = new() { 0, 1, 2, 3, 4, 5, 6, 7 };

            int n = allNumbers.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                int value = allNumbers[k];
                allNumbers[k] = allNumbers[n];
                allNumbers[n] = value;
            }

            // Wybranie pierwszych 4 liczb
            minigamesIDs = allNumbers.GetRange(0, 4);

            // Data wylosowania w Stringu
            // 2024.1.18
            string data = new StringBuilder("")
                            .Append(timeNow.Year.ToString())
                            .Append(".")
                            .Append(timeNow.Month.ToString())
                            .Append(".")
                            .Append(timeNow.Day.ToString())
                            .ToString();

            await SaveMinigamesIDsDatabase(data, minigamesIDs);

            return minigamesIDs;
        });
    }

    private async Task<bool> SaveMinigamesIDsDatabase(string data, List<int> ids)
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("todayMinigames").Child("value").SetValueAsync(string.Join(";", ids));
            await DBTask;

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");
                return false;
            }
            else
            {
                DBTask = dbReference.Child("todayMinigames").Child("data").SetValueAsync(data);
                await DBTask;

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
        });
    }

    private async Task<Tuple<bool, List<int>>> GetMinigamesIDsDatabase()
    {
        return await Task.Run(async () =>
        {
            var DBTask = dbReference.Child("todayMinigames").Child("data").GetValueAsync();
            await DBTask;

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

                return new Tuple<bool, List<int>>(false, null);
            }
            else
            {
                DateTime time = DateTime.Now;

                if (DBTask.Result.Value.ToString().Equals("") || 
                    !DBTask.Result.Value.ToString().Equals(
                        new StringBuilder("")
                        .Append(time.Year.ToString())
                        .Append(".")
                        .Append(time.Month.ToString())
                        .Append(".")
                        .Append(time.Day.ToString())
                        .ToString()
                    )
                )
                {
                    return new Tuple<bool, List<int>>(false, null);
                }

                DBTask = dbReference.Child("todayMinigames").Child("value").GetValueAsync();
                await DBTask;

                if (DBTask.Exception != null)
                {
                    Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

                    return new Tuple<bool, List<int>>(false, null);
                }
                else
                {
                    return new Tuple<bool, List<int>>(true, DBTask.Result.Value.ToString().Split(';').ToList().Select(int.Parse).ToList());
                }
            }
        });
    }

    public bool Logout()
    {
        if (LoggedUser == null)
        {
            return false;
        }

        auth.SignOut();
        firebaseLoggedUser = null;
        LoggedUser = null;
        return true;
    }

    private async Task<Tuple<ServerSearchError, UserData?>> GetLoggedUserDatabase()
    {
        return await Task.Run(async () =>
        {
            if (LoggedUser == null)
            {
                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.UserNotLogged, null);
            }

            var DBTask = dbReference.Child("users").Child(firebaseLoggedUser.UserId).GetValueAsync();
            await DBTask;

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.Other, null);
            }
            else if (DBTask.Result.Value == null)
            {
                UserData tempUser = LoggedUser.Value;
                tempUser.ChallengeData = new();
                tempUser.FriendRequests = new();
                tempUser.Friends = new();
                tempUser.Highscores = new List<float> { 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f };

                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.None, tempUser);
            }

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

            return new Tuple<ServerSearchError, UserData?>(ServerSearchError.None, user);
        });
    }

    public async Task<Tuple<ServerSearchError, UserData?>> GetUserDataByNickname(string nickname)
    {
        return await Task.Run(async () =>
        {
            if (LoggedUser == null)
            {
                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.UserNotLogged, null);
            }

            var DBTask = dbReference.Child("users").OrderByChild("nickname").GetValueAsync();
            await DBTask;

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.Other, null);
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
                    return new Tuple<ServerSearchError, UserData?>(ServerSearchError.None, user);
                }
                else
                {
                    return new Tuple<ServerSearchError, UserData?>(ServerSearchError.NoUserFound, null);
                }
            }
        });
    }

    public async Task<Tuple<ServerSearchError, UserData?>> GetUserDataByEmail(string email)
    {
        return await Task.Run(async () =>
        {
            if (LoggedUser == null)
            {
                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.UserNotLogged, null);
            }

            var DBTask = dbReference.Child("users").OrderByChild("email").GetValueAsync();
            await DBTask;

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.Other, null);
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
                    return new Tuple<ServerSearchError, UserData?>(ServerSearchError.None, user);
                }
                else
                {
                    return new Tuple<ServerSearchError, UserData?>(ServerSearchError.NoUserFound, null);
                }
            }
        });
    }

    public async Task<Tuple<ServerSearchError, UserData?>> GetUserDataByID(string id)
    {
        return await Task.Run(async () =>
        {
            if (LoggedUser == null)
            {
                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.UserNotLogged, null);
            }

            var DBTask = dbReference.Child("users").Child(id).GetValueAsync();
            await DBTask;

            if (DBTask.Exception != null)
            {
                Debug.LogWarning(message: $"Failed to register task with {DBTask.Exception}");

                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.Other, null);
            }
            else if (DBTask.Result.Value == null)
            {
                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.NoUserFound, null);
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

                return new Tuple<ServerSearchError, UserData?>(ServerSearchError.None, user);
            }
        });
    }
}