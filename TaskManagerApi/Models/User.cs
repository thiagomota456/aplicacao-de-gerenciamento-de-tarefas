namespace TaskManagerApi.Models;

public record User
{
    public required Guid Id { get; set; }
    public required string Username  { get; set; }
    public required string PasswordHash { get; set; }
    public required DateTime CreatedAt { get; set; }
};