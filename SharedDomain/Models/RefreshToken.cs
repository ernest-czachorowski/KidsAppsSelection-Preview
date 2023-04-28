namespace SharedDomain.Models;

[Serializable]
public class RefreshToken
{
    // RefreshToken Id is same as user Id
    public Guid Id { get; set; }
    public string Token { get; set; }
}