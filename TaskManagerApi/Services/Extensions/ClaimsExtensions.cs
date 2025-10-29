using System.Security.Claims;

namespace TaskManagerApi.Services.Extensions;

public static class ClaimsExtensions
{
    public static Guid RequireUserId(this ClaimsPrincipal user)
    {
        var val = user.FindFirstValue("uid") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(val, out var id)
            ? id
            : throw new UnauthorizedAccessException("Invalid or missing user id claim.");
    }
}