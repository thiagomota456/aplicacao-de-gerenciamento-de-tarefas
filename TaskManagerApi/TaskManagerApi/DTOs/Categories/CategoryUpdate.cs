using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.DTOs.Categories;

public record CategoryUpdate(
    [StringLength(200)] string? Description
);