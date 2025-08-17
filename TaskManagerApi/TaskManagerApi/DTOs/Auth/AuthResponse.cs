namespace TaskManagerApi.DTOs.Auth;

public record AuthResponse(
    Guid UserId,
    string Username,
    string AccessToken,
    DateTime ExpiresAt,
    string? RefreshToken = null,
    DateTime? RefreshTokenExpiresAt = null
);