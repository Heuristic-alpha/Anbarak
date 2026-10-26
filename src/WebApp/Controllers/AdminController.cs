using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Anbarak.WebApp.Controllers
{
    [Authorize(Roles =$"{IdentityRoles.Administrator}")]
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
