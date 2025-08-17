using System.ComponentModel.DataAnnotations;

namespace TaskManagerApi.Services;

public record class JwtOptions
{
    [Required] public string Issuer { get; init; } = default!;
    [Required] public string Audience { get; init; } = default!;
    [Required, MinLength(32)] public string Key { get; init; } = default!;
    public int AccessTokenMinutes { get; init; } = 120;
}