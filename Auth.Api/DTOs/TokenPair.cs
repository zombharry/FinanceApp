namespace Auth.Api.DTOs;

public record TokenPair(
    string AccessToken,
    string RefreshToken
    );
