using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.DTOs.Tasks;

public record TaskCreate(
    [Required] Guid UserId,
    [Required, StringLength(160, MinimumLength = 1)] string Title,
    [Required, StringLength(10_000, MinimumLength = 1)] string Description,
    [Range(1, int.MaxValue)] int? CategoryId,
    bool IsCompleted = false
);