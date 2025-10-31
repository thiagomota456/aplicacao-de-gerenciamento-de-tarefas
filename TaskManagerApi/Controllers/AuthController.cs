using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Data;
using TaskManagerApi.DTOs.Auth;
using TaskManagerApi.Models;
using TaskManagerApi.Services;

namespace TaskManagerApi.Controllers;

[ApiController]
[Route("auth")]
[EnableCors("AllowAllHeaders")]
public class AuthController(TaskDbContext db, IJwtTokenService jwt) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest input, CancellationToken ct)
    {
        var exists = await db.Users.AnyAsync(u => u.Username == input.Username, ct);
        if (exists) return Conflict("Username already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = input.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(input.Password),
            CreatedAt = DateTime.UtcNow
        };
        db.Users.Add(user);
        await db.SaveChangesAsync(ct);

        var tkn = jwt.Create(user.Id, user.Username);
        return Ok(new AuthResponse(user.Id, user.Username, tkn.Token, tkn.ExpiresAt));
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest input, CancellationToken ct)
    {
        var user = await db.Users.FirstOrDefaultAsync(u => u.Username == input.Username, ct);
        if (user is null) return Unauthorized("Invalid credentials.");

        var ok = BCrypt.Net.BCrypt.Verify(input.Password, user.PasswordHash);
        if (!ok) return Unauthorized("Invalid credentials.");

        var tkn = jwt.Create(user.Id, user.Username);
        return Ok(new AuthResponse(user.Id, user.Username, tkn.Token, tkn.ExpiresAt));
    }
}