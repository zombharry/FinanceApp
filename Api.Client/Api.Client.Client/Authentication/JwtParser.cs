using System.Security.Claims;
using System.Text.Json;

namespace Api.Client.Client.Authentication;

public class JwtParser
{
    public static IEnumerable<Claim> ParseClaims(string jwt)
    {
        var payload = jwt.Split('.')[1];

        payload = Pad(payload);

        var jsonBytes = Convert.FromBase64String(payload);

        var values = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes)!;

        return values.Select(v => new Claim(v.Key, v.Value.ToString()!));
    }

    private static string Pad(string value)
    {
        switch (value.Length % 4)
        {
            case 2: value += "=="; break;
            case 3: value += "="; break;
        }

        return value.Replace('-', '+')
                    .Replace('_', '/');
    }
}
