namespace Api.Shared.Authentication.DTOs;

public record TokenPair(
    string AccessToken,
    string RefreshToken
    );
