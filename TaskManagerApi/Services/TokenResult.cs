namespace TaskManagerApi.Services;

public class TokenResult(string token, DateTime expiresAt)
{
    public string Token { get; } = token;
    public DateTime ExpiresAt { get; } = expiresAt;
}