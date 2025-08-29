using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureMvcAuth.Models;

namespace SecureMvcAuth.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<Test> _userManager;

        public AdminController(UserManager<Test> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Dashboard()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }
    }
}
