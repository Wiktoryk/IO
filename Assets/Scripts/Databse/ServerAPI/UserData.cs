using Firebase.Auth;
using System.Collections.Generic;

public struct UserData
{
    public string ID;
    public string Email;
    public string Nickname;

    public List<string> Friends;
    public List<float> Highscores;
    public Dictionary<string, bool> FriendRequests;
    public List<string> FriendInvites;
    public List<ChallengeData> ChallengeData;

    public UserData(FirebaseUser user, List<string> friends, List<float> highscores, Dictionary<string, bool> friendRequests, List<string> friendInvites, List<ChallengeData> challengeData)
    {
        ID = user.UserId;
        Email = user.Email;
        Nickname = user.DisplayName;

        Friends = friends;
        Highscores = highscores;
        FriendRequests = friendRequests;
        FriendInvites = friendInvites;
        ChallengeData = challengeData;
    }
}
