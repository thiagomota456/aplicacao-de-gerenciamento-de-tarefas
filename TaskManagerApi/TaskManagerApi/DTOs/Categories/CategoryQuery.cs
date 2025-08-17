namespace TaskManagerApi.DTOs.Categories;

public record CategoryQuery(
    string? Search = null,
    int Page = 1,
    int PageSize = 50,
    string? SortBy = null,   // "id" | "description"
    string? SortDir = null   // "asc" | "desc"
);