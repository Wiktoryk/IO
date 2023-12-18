using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
public interface IDataManager
{   
    ServerLogInError Login(string email, string password);
    ServerRegisterError Register(string email, string password, string nickname);
	bool Logout();
	ServerUserUpdateError updateUser(UserData newUserData);
    List<int> fetchMiniGamesList();
    ServerUserUpdateError changeNickname(string newNickname);
    Task<AuthError?> changePassword(string newPassword);
    bool SendFriendRequest(string friendId);
    bool CancelFriendRequest(string friendId);
    bool SaveScore(int minigameId, float score);
    (ServerSearchError, UserData?) fetchUserData();
    bool sendChallenge(string friendId, ChallengeData challenge);
    bool cancelChallenge(ChallengeData challenge);
    bool RespondFriendRequest(string friendId, bool accept);
    bool AcceptChallenge(string friendId, ChallengeData challengeResponse);
    (ServerSearchError, UserData?) GetUserByNickname(string nickname);
    (ServerSearchError, UserData?) GetUserByEmail(string email);
    (ServerSearchError, UserData?) GetUserID(string id);
}