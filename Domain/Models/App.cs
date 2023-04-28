namespace Domain.Models;

public class App
{
    [Required]
    [MaxLength(36)]
    public Guid Id { get; set; }

    [Required]
    public int DailyRandom { get; set; }

    [Required]
    public AppStatus Status { get; set; }

    [Required]
    public AppPlatform Platform { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; }

    [Required]
    [MaxLength(32)]
    public DateTime AddDate_UTC { get; set; }

    [Required]
    [MaxLength(32)]
    public DateTime UpdateDate_UTC { get; set; }

    [Required]
    [MaxLength(255)]
    public string Url { get; set; }

    [Required]
    [MaxLength(255)]
    public string Icon { get; set; }

    [Required]
    [MaxLength(2048)]
    public string[] Images { get; set; }

    public User? AddedBy { get; set; }
}