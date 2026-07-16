namespace Api.Client.Client.DTOs;

public record TokenPair(
    string AccessToken,
    string RefreshToken
    );
