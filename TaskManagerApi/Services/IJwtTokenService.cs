namespace TaskManagerApi.Services;

public interface IJwtTokenService
{
    TokenResult Create(Guid userId, string username);
}