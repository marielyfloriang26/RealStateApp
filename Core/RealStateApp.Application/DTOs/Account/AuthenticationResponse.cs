namespace RealStateApp.Application.DTOs.Account;

public class AuthenticationResponse
{
    public string Token { get; set; } = null!;
    public string Usuario { get; set; } = null!;
    public List<string> Roles { get; set; } = new();
    public DateTime Expiracion { get; set; }
}