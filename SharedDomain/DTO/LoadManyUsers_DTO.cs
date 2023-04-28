namespace SharedDomain.DTO;

public class LoadManyUsers_DTO
{
    [NoWhitespace]
    [Range(0, GLOBAL.MAX_ITEMS_TO_LOAD-1)]
    public int Start { get; set; } = 0;

    [NoWhitespace]
    [Range(1, GLOBAL.MAX_ITEMS_TO_LOAD)]
    public int ItemsToLoad { get; set; } = 10;

    public string SearchedText { get; set; } = string.Empty;

    public UserRole Role { get; set; }
}