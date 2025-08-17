namespace TaskManagerApi.Models;

public record Category
{
    public required int Id { get; set; }
    public required Guid UserId { get; set; }
    public string? Description { get; set; }
};