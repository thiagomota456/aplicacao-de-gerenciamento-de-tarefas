using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.DTOs.Categories;

public record CategoryCreate(
    [StringLength(200)] string? Description
);