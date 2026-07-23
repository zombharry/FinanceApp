namespace Client.App.DTO.AuthDtos;

public class AuthResponse
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
}
