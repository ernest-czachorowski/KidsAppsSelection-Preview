namespace SharedDomain.Enums;

public enum AppStatus
{
    Any,
    Visible,
    Hidden,
    Deleted
}

public enum AppPlatform
{
    Any,
    Android,
    iOS,
    Mac,
    Windows
}

public enum UserRole
{
    Admin,
    User
}

public enum PageStatus
{
    Init,
    Loading,
    DataLoaded,
    NoMoreData,
    Error
}
