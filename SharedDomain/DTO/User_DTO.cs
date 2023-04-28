namespace SharedDomain.DTO;

public class User_DTO
{
    public Guid Id { get; set; }

    [NoWhitespace]
    [StringLength(16, MinimumLength = 4)]
    [LettersDigitsAndDashes]
    public string Username { get; set; }

    [NoWhitespace]
    [StringLength(64, MinimumLength = 8)]
    [ValidEmail]
    public string Email { get; set; }

    public DateTime CreateDate_UTC { get; set; }

    public DateTime UpdateDate_UTC { get; set; }

    public UserRole Role { get; set; }

    public List<App_DTO> Apps { get; set; } = new List<App_DTO>();
}

