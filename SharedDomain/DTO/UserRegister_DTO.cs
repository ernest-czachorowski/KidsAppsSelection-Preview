namespace SharedDomain.DTO;

public class UserRegister_DTO
{
    [NoWhitespace]
    [StringLength(16, MinimumLength = 4)]
    [LettersDigitsAndDashes]
    public string Username { get; set; } = string.Empty;

    [NoWhitespace]
    [PopularValidEmail]
    [StringLength(64, MinimumLength = 8)]
    public string Email { get; set; } = string.Empty;

    [NoWhitespace]
    [StringLength(64, MinimumLength = 8)]
    public string Password { get; set; } = string.Empty;

    [NoWhitespace]
    [StringLength(64, MinimumLength = 8)]
    [Compare("Password")]
    public string ConfirmPassword { get; set; } = string.Empty;
}