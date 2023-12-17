using System;

public struct ChallengeData : IEquatable<ChallengeData>
{
    public int MinigameID;
    public float Score;
    public string UserID;

    public bool Equals(ChallengeData other)
    {
        return other.MinigameID == MinigameID && other.Score == Score && other.UserID == UserID;
    }
}
