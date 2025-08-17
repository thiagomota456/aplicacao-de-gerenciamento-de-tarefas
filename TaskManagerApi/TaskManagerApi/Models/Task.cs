namespace TaskManagerApi.Models;

public record Task
{
    //Id, Title, Description, IsCompleted, Category, CreatedAt, UpdatedAt, UserId (chave estrangeira)
    public required Guid Id { get; set; }
    public required Guid UserId { get; set; }
    public required string Description {get; set;}
    public required bool IsCompleted {get; set;}
    public int CategoryIds { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime UpdatedAt { get; set; }
    
};