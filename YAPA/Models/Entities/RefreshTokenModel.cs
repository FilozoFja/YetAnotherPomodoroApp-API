namespace YAPA.Models.Entities;
public class RefreshTokenModel
{
    public int Id { get; set; }
    public string Token { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime Expires { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Revoked { get; set; }
}
