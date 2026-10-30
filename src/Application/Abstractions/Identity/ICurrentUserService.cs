namespace Anbarak.Application.Abstractions.Identity;

public interface ICurrentUserService
{
    public string? UserId { get; }

    public string? UserName { get; }

    public string? UserEmail { get; }

    public List<string> GetRoleNames();
}
