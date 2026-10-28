using Anbarak.Application.Abstractions.Identity;
using System.Security.Claims;

namespace Anbarak.WebApp.Services;

public sealed class CurrentUserService(
    IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    private ClaimsPrincipal? CurrentUser =>
        httpContextAccessor.HttpContext?.User;

    public string? UserId =>
        CurrentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    public List<string> GetRoleNames()
    {
        var roles = CurrentUser?.Identities
            .Where(identity => identity.IsAuthenticated)
            .SelectMany(identity =>
                identity.FindAll(identity.RoleClaimType))
            .Select(claim => claim.Value)
            .Distinct()
            .ToList() ?? new List<string>();

        return roles;
    }
}