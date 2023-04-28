namespace SharedDomain.DTO;

public class UserChangePassword_DTO
{
    [NoWhitespace]
    [StringLength(64, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [NoWhitespace]
    [StringLength(64, MinimumLength = 8)]
    public string NewPassword { get; set; } = string.Empty;

    [NoWhitespace]
    [StringLength(64, MinimumLength = 8)]
    [Compare("NewPassword")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

