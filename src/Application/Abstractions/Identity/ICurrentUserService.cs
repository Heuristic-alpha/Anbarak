namespace Anbarak.Application.Abstractions.Identity;

public interface ICurrentUserService
{
    public string? UserId { get; }

    public List<string> GetRoleNames();
}
