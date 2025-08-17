using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.DTOs.Auth;

public record RegisterRequest(
    [Required, StringLength(64, MinimumLength = 3)] string Username,
    [Required, StringLength(128, MinimumLength = 8)] string Password
);

public record LoginRequest(
    [Required] string Username,
    [Required] string Password
);