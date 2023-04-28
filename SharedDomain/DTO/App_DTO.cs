namespace SharedDomain.DTO;

public class App_DTO
{
    public Guid Id { get; set; }

    public int DailyRandom { get; set; }

    public AppStatus Status { get; set; }

    public AppPlatform Platform { get; set; }

    public string Title { get; set; }

    public DateTime AddDate_UTC { get; set; }

    public DateTime UpdateDate_UTC { get; set; }

    public string Url { get; set; }

    public string Icon { get; set; }

    public string[] Images { get; set; }

    public string AddedBy { get; set; }
}