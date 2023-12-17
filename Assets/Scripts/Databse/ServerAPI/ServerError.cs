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
    Other = 6
}

public enum ServerUserUpdateError
{
    None = 0,
    UserNotLoggedIn = 1,
    NicknameUpdateFailed = 2
}

public enum ServerSearchError
{
    None = 0,
    UserNotLogged = 1,
    NoUserFound = 2
}