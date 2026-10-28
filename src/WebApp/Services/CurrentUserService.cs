using Anbarak.Application.Abstractions.Identity;
using Anbarak.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Anbarak.WebApp.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ClaimsPrincipal? _user;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor,                       
                                 UserManager<ApplicationUser> userManager)
    {
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;

        _user = _httpContextAccessor.HttpContext?.User;
    }

    public string? UserId => _user?.FindFirstValue(ClaimTypes.NameIdentifier);

    public async Task<List<string>> GetRoleNames()
    {
        ApplicationUser? applicationUser = null;
        if (_user != null) applicationUser = await _userManager.GetUserAsync(_user);
        
        if (applicationUser != null)
        {
            var roles=  await _userManager.GetRolesAsync(applicationUser);
            return roles.ToList() ?? new List<string>();
        }
        return new List<string>();
    }
}
