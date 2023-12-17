using Firebase.Auth;
using System.Collections.Generic;

public struct UserData
{
    public readonly string ID;
    public string Email;
    public string Nickname;

    public List<string> Friends;
    public List<float> Highscores;
    public List<string> FriendRequests;
    public List<ChallengeData> ChallengeData;

    public UserData(FirebaseUser user, List<string> friends, List<float> highscores, List<string> friendRequests, List<ChallengeData> challengeData)
    {
        ID = user.UserId;
        Email = user.Email;
        Nickname = user.DisplayName;

        Friends = friends;
        Highscores = highscores;
        FriendRequests = friendRequests;
        ChallengeData = challengeData;
    }
}
