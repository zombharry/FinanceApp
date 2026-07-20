namespace Client.App.DTO;

public class UserInfo
{
    public UserInfo()
    {
        Claims = new Dictionary<string, string>();
    }

    public string? Username { get; set; }
    public string? Email { get; set; }
    public Dictionary<string, string> Claims { get; set; }
}
