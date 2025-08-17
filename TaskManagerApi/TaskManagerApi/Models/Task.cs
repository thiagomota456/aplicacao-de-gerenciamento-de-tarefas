namespace TaskManagerApi.Models;

public record Task
{
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required Guid Title { get; set; }
    public required string Description {get; set;}
    public required bool IsCompleted {get; set;}
    public int CategoryId { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime UpdatedAt { get; set; }
    
};