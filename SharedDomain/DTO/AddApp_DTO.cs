namespace SharedDomain.DTO;

public class AddApp_DTO
{
    [NoWhitespace]
    [StringLength(255, MinimumLength = 16)]
    [PlayStoreUrl(ErrorMessage = "Invalid Play Store URL")]
    public string Url { get; set; } = string.Empty;
}