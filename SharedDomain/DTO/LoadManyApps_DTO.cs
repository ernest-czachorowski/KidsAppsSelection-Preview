namespace SharedDomain.DTO;

public class LoadManyApps_DTO
{
    [NoWhitespace]
    [Range(0, GLOBAL.MAX_ITEMS_TO_LOAD-1)]
    public int Start { get; set; } = 0;

    [NoWhitespace]
    [Range(1, GLOBAL.MAX_ITEMS_TO_LOAD)]
    public int ItemsToLoad { get; set; } = 10;

    public string SearchedText { get; set; } = string.Empty;

    public AppStatus Status { get; set; } = AppStatus.Any;

    public AppPlatform Platform { get; set; } = AppPlatform.Any;
}