public enum ServerLogInError
{
    None = 0,
    WrongPassword = 1,
    WrongEmail = 2,
    MissingEmail = 3,
    MissingPassword = 4,
    UserNotFound = 5,
    Other = 6
}

public enum ServerRegisterError
{
    None = 0,
    WeakPassword = 1,
    EmailAlreadyInUse = 2,
    MissingEmail = 3,
    MissingPassword = 4,
    NicknameSetupFailed = 5,
    DatabaseUserRegisterFailed = 6,
    Other = 7
}

public enum ServerUserUpdateError
{
    None = 0,
    UserNotLoggedIn = 1,
    NicknameUpdateFailed = 2,
    HighscoresUpdateFailed = 3,
    FriendsListUpdateFailed = 4,
    FriendsInvitesAddFailed = 5,
    FriendsInvitesRemoveFailed = 6,
    FreindsRequestsAddFailed = 7,
    FriendsRequestsRemoveFailed = 8,
    ChallangesSendFailed = 9,
    ChallangesRemoveFailed = 10,
    GetCurrentUserData = 11,
    XPUpdateFailed = 12
}

public enum ServerSearchError
{
    None = 0,
    Other = 1,
    UserNotLogged = 2,
    NoUserFound = 3
}