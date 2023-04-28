namespace SharedDomain.DTO;

public class UserLogin_DTO
{
    [NoWhitespace]
    [StringLength(64, MinimumLength = 8)]
    [ValidEmail]
    public string Email { get; set; } = string.Empty;

    [NoWhitespace]
    [StringLength(64, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;
}