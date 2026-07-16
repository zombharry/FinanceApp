namespace Api.Client.Client.Services;

public class TokenService
{
    public string? Token { get; private set; }

    public void Set(string token)
        => Token = token;

    public void Clear()
        => Token = null;
}
