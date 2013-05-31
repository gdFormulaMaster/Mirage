namespace Mirage.Common.Network {
    /// <summary>
    /// Login Status Codes Enumeration
    /// </summary>
    public enum LoginStatus : int {
        Failed = 0,
        Successful,
        FailedUserNotExists,
        FailedPasswordInvalid,
        FailedUserLoggedIn,
        FailedClientLoggedIn,
        FailedUsernameInvalid,
    }

    /// <summary>
    /// Registration Status Codes Enumeration
    /// </summary>
    public enum RegistrationStatus : int {
        Failed = 0,
        Successful,
        FailedUsernameInvalid,
        FailedUserExists,
        FailedPasswordInvalid,
    }
}