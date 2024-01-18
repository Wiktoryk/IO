using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Auth;
public interface IDataManager
{   
    Task<ServerLogInError> Login(string email, string password);
    Task<ServerRegisterError> Register(string email, string password, string nickname);
	bool Logout();
	Task<ServerUserUpdateError> updateUser(UserData newUserData);
    Task<List<int>> fetchMiniGamesList();
    Task<ServerUserUpdateError> changeNickname(string newNickname);
    Task<bool> changePassword(string newPassword);
    Task<bool> SendFriendRequest(string friendId);
    Task<bool> CancelFriendRequest(string friendId);
    Task<bool> SaveScore(int minigameId, float score);
    Tuple<ServerSearchError, UserData?> fetchUserData();
    Task<bool> sendChallenge(string friendId, ChallengeData challenge);
    Task<bool> cancelChallenge(ChallengeData challenge);
    Task<bool> RespondFriendRequest(string friendId, bool accept);
    Task<bool> AcceptChallenge(string friendId, ChallengeData challengeResponse);
    Task<Tuple<ServerSearchError, UserData?>> GetUserByNickname(string nickname);
    Task<Tuple<ServerSearchError, UserData?>> GetUserByEmail(string email);
    Task<Tuple<ServerSearchError, UserData?>> GetUserByID(string id);
}