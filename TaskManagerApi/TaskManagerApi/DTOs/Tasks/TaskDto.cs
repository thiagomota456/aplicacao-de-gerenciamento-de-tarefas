namespace TaskManagerApi.DTOs.Tasks;

public record TaskDto(
    Guid Id,
    Guid UserId,
    string Title,
    string Description,
    int? CategoryId,
    bool IsCompleted,
    DateTime Created,
    DateTime UpdatedAt
);