namespace Domain.Models;

public class User
{
    [Required]
    [MaxLength(36)]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(16)]
    public string Username { get; set; }

    [Required]
    [MaxLength(64)]
    public string Email { get; set; }

    [Required]
    public byte[] PasswordHash { get; set; }

    [Required]
    public byte[] PasswordSalt { get; set; }

    [Required]
    [MaxLength(32)]
    public DateTime CreateDate_UTC { get; set; }

    [Required]
    [MaxLength(32)]
    public DateTime UpdateDate_UTC { get; set; }

    [Required]
    public UserRole Role { get; set; }

    [Required]
    public List<App> Apps { get; set; } = new();

    [Required]
    [MaxLength(1024)]
    public string RefreshToken { get; set; }

    [Required]
    [MaxLength(32)]
    public DateTime RefreshTokenExpires_UTC { get; set; }
}
