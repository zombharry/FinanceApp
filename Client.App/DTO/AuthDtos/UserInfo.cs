namespace Client.App.DTO.AuthDtos;

public class UserInfo
{
    public UserInfo()
    {
        Claims = new Dictionary<string, string>();
    }

    public Guid UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}
