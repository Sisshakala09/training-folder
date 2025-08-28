using Humanizer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HRISApp.Controllers
{

    public class UserManagementController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public UserManagementController(RoleManager<IdentityRole> role, UserManager<IdentityUser> user)
        {
            _roleManager = role;
            _userManager = user;
        }
        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }
    }
