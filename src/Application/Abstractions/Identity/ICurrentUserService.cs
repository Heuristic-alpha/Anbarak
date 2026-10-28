namespace Anbarak.Application.Abstractions.Identity;

public interface ICurrentUserService
{
    public string? UserId { get; }

    public Task<List<string>> GetRoleNames();
}
