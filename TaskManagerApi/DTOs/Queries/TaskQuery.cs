namespace TaskManagerApi.DTOs.Queries;

public record TaskQuery(
    Guid? UserId = null,
    int? CategoryId = null,
    bool? IsCompleted = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20,
    string? SortBy = null,   // "created", "updatedAt", "title"
    string? SortDir = null   // "asc" | "desc"
);