using System;
using System.Collections.Generic;
using System.Threading.Tasks;
public interface IDataManager
{   
    Task<ServerLogInError> Login(string email, string password);
    Task<ServerRegisterError> Register(string email, string password, string nickname);
	bool Logout();
	Task<ServerUserUpdateError> UpdateUser(UserData newUserData);
    Task<List<int>> FetchMiniGamesList();
    Task<ServerUserUpdateError> ChangeNickname(string newNickname);
    Task<bool> ChangePassword(string newPassword);
    Task<bool> SendFriendRequest(string friendId);
    Task<bool> CancelFriendRequest(string friendId);
    Task<bool> SaveScore(int minigameId, float score);
    Task<bool> UpdateUserXP(uint xp);
    Tuple<ServerSearchError, UserData?> FetchUserData();
    Task<bool> SendChallenge(string friendId, ChallengeData challenge);
    Task<bool> CancelChallenge(ChallengeData challenge);
    Task<bool> RespondFriendRequest(string friendId, bool accept);
    Task<bool> AcceptChallenge(string friendId, ChallengeData challengeResponse);
    Task<Tuple<ServerSearchError, UserData?>> GetUserByNickname(string nickname);
    Task<Tuple<ServerSearchError, UserData?>> GetUserByEmail(string email);
    Task<Tuple<ServerSearchError, UserData?>> GetUserByID(string id);
}